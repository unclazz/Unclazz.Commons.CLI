using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// <see cref="ICommandLine"/>インターフェースを実装した具象クラスです。
	/// <see cref="Builder(string)"/>メソッドを通じてビルダー・オブジェクトを取得することができます。
	/// </summary>
	public class CommandLine : ICommandLine
	{
		/// <summary>
		/// 新しいビルダーを生成します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="commandName">コマンド名</param>
		public static CommandLineBuilder Builder(string commandName)
		{
			return new CommandLineBuilder(commandName);
		}

		public void Parse(IEnumerable<string> arguments)
		{
			Parse(arguments, LoadSettings());
		}

		public void Parse(IEnumerable<string> arguments, IDictionary<string, string> settings)
		{
			try
			{
				ResolveOptions(arguments, settings);
			}
			catch (Exception ex)
			{
				throw new ParseException
				(ParseException.ExceptionCategory.UnexpectedErrorHasOccurred,
				 null, null, ex);
			}
		}

		public string CommandName { get; }
		public string Description { get; }
		public bool CaseSensitive { get; }
		public OptionCollection Options { get; }
		public IEnumerable<string> ArgumentNames { get; }
		public Action<IEnumerable<string>> SetterDelegate { get; }

		internal CommandLine(string cn, string d, bool cs,
		                     IEnumerable<IOption> os,
		                     IEnumerable<string> ags,
		                     Action<IEnumerable<string>> sd)
		{
			if (cn == null || cn.Length == 0)
			{
				throw new ArgumentException("Command name must be not-null and not-empty.");
			}
			if (os == null || os.Count() == 0)
			{
				throw new ArgumentException("Option's sequence must be not-null and not-empty.");
			}

			CommandName = cn;
			Description = d;
			CaseSensitive = cs;
			Options = new OptionCollection(os);
			ArgumentNames = ags;
			SetterDelegate = sd;
		}

		/// <summary>
		/// オプションの名前・別名とコマンドラインで実際に指定された名前とを照合します。
		/// 照合に際して<see cref="CaseSensitive"/>の値が考慮されます。
		/// </summary>
		/// <returns>照合が成功した場合は<c>true</c></returns>
		/// <param name="opt">オプションの定義</param>
		/// <param name="specifiedName">実際に指定されたオプションの名前</param>
		bool HaveSameName(IOption opt, string specifiedName)
		{
			if (CaseSensitive)
			{
				return opt.Name.Equals(specifiedName) ||
						  opt.AlternativeName.Equals(specifiedName);
			}
			else {
				return opt.Name.ToUpper().Equals(specifiedName.ToUpper()) ||
						  opt.Name.ToUpper().Equals(specifiedName.ToUpper());
			}
		}
		/// <summary>
		/// 一連のコマンドライン・オプションの値をその定義情報にしたがい解決します。
		/// このメソッドはコマンドライン引数とアプリケーション構成ファイルの設定情報を入力として、
		/// コマンドライン・オプションの定義情報に規定された情報を読み取ります。
		/// </summary>
		/// <param name="rawArgs">コマンドライン引数</param>
		/// <param name="settings">アプリケーション構成ファイルの設定情報</param>
		void ResolveOptions(IEnumerable<string> rawArgs, IDictionary<string,string> settings)
		{
			// 1. コマンドライン引数の処理（前段階）

			// まず解決待ちの引数と解決できなかった引数を格納するキューを用意する
			// ＊最終的に解決不可となったものは、コマンドライン・オプションやその引数ではなく
			// コマンドそのものの引数であると見なされ、ICommandLine.SetterDelegateに渡される。
			var toArgResolve = new Queue<string>(rawArgs);
			var cantArgResolved = new Queue<string>();

			// 値解決の状況を記録するための辞書を用意する
			// ＊後続の処理では、あるオプションの値の解決に成功する都度、
			// この辞書の当該オプションのエントリーを検索して
			// その値であるリストにオプションの値を追加していく
			var ctx = new Dictionary<IOption, List<string>>();
			foreach (var o in Options)
			{
				ctx.Add(o, new List<string>());
			}

			// 解決待ちキューが空になるまでループ
			while (toArgResolve.Count > 0)
			{
				// 解決待ちキューの先頭から値を1つ取得（削除）
				var former = toArgResolve.Dequeue();
				// 解決待ちキューの先頭から値を1つ取得（非・削除）
				var latter = toArgResolve.Count == 0
							 ? string.Empty : toArgResolve.Peek();

				// 値解決を試みる
				var result = ResolveOptionsWithArguments(former, latter, ctx);
				// 解決の成否をチェック
				if (result)
				{
					// 成功の場合
					// 値解決状況を記録する辞書からオプション定義を検索
					var opt = ctx.First(a => HaveSameName(a.Key, former));

					// オプションがそれ自体の引数をとるものかどうかチェック
					if (opt.Key.HasArgument)
					{
						// 引数をとるものの場合
						// 解決した値を辞書エントリーの値に追加
						opt.Value.Add(latter);
						// さらに解決待ちキューから値を削除
						toArgResolve.Dequeue();
					}
					else {
						// 引数をとるものでない場合
						// 解決した値を辞書エントリーの値にダミー値を追加
						opt.Value.Add(string.Empty);
					}
				}
				else {
					// 失敗の場合
					// 解決不可キューに値を追加
					cantArgResolved.Enqueue(former);
				}
			}

			// 2. アプリケーション構成ファイルの処理

			// アプリケーション構成ファイルの設定情報に基づきループ
			foreach (var item in settings)
			{
				// 値解決を試みる
				var result = ResolveOptionsWithAppSettings(item.Key, item.Value, ctx);
				// 成否をチェック
				if (result)
				{
					// 成功の場合
					// 値解決状況を記録する辞書からオプション定義を検索
					var opt = ctx.First(a => a.Key.SettingName.Equals(item.Key));
					if (opt.Key.HasArgument)
					{
						// 引数をとるものの場合
						// 解決した値を辞書エントリーの値に追加
						opt.Value.Add(settings[item.Key]);
					}
					else {
						// 引数をとるものでない場合
						// 解決した値を辞書エントリーの値にダミー値を追加
						opt.Value.Add(string.Empty);
					}
				}
			}

			// 3. コマンドライン引数の処理（後段階）

			// 解決不可キューの内容を「残りの引数」としてデリゲートに渡す
			SetterDelegate(cantArgResolved);

			// 4. 必死オプションのチェック

			// 必須にもかかわらず一連の処理で解決のできていないオプションを検索
			var missed = ctx.Where(kv => kv.Key.Required && kv.Value.Count() == 0)
			               .Select(kv => kv.Key).FirstOrDefault();
			if (missed != null)
			{
				// 該当するものが1件でもあった場合は例外をスローする
				throw new ParseException
				(ParseException.ExceptionCategory.RequiredOptionNotFound,
				 missed, null);
			}
		}

		/// <summary>
		/// 一連のコマンドライン・オプションの値をその定義情報にしたがい解決します。
		/// このメソッドはコマンドライン引数を入力として、
		/// コマンドライン・オプションの定義情報に規定された情報を読み取ります。
		/// </summary>
		/// <returns>値解決に成功した場合は<c>true</c></returns>
		/// <param name="former">1つ目のコマンドライン引数</param>
		/// <param name="latter">2つ目のコマンドライン引数</param>
		/// <param name="ctx">値解決状況を記録した辞書</param>
		bool ResolveOptionsWithArguments(string former, string latter,
			 Dictionary<IOption, List<string>> ctx)
		{
			// オプション定義うちで名前もしくは別名が一致するものを検索
			var nameMatched = Options
				.FirstOrDefault(a => HaveSameName(a, former));
			// 検索結果をチェック
			if (nameMatched == null)
			{
				// 該当するものがなかった場合はただちに処理を終える
				return false;
			}

			// すでに1回値解決が済んでいるにもかかわらず、
			// 複数回指定が不可のオプションが再度登場したケースをチェック
			if (!nameMatched.Multiple &&
				ctx.Any(b => b.Key.Equals(nameMatched) && b.Value.Count() > 0))
			{
				// 該当する場合は例外をスローする
				throw new ParseException
				(ParseException.ExceptionCategory.DuplicatedOption,
				 nameMatched, latter);
			}

			try
			{
				// オプション定義に登録されたデリゲートを呼び出す
				nameMatched.SetterDelegate(nameMatched.HasArgument ? latter : string.Empty);
				// 値解決は成功したものとしてtrueを返す
				return true;
			}
			catch (Exception ex)
			{
				// 例外がスローされた場合
				// 処理状況を示す情報とともにラップして再スローする
				throw new ParseException
				(ParseException.ExceptionCategory.SetterErrorHasOccurred,
				 nameMatched, latter, ex);
			}
		}

		/// <summary>
		/// 一連のコマンドライン・オプションの値をその定義情報にしたがい解決します。
		/// このメソッドはアプリケーション構成ファイルの設定情報を入力として、
		/// コマンドライン・オプションの定義情報に規定された情報を読み取ります。
		/// </summary>
		/// <returns>値解決に成功した場合は<c>true</c></returns>
		/// <param name="key">設定情報のキー名</param>
		/// <param name="value">設定情報の値</param>
		/// <param name="ctx">値解決状況を記録した辞書</param>
		bool ResolveOptionsWithAppSettings(string key, string value,
			 Dictionary<IOption, List<string>> ctx)
		{
			// オプション定義うちでキー名が該当するものを検索
			var nameMatched = Options
				.FirstOrDefault(a => a.SettingName.Equals(key));
			// 検索結果をチェック
			if (nameMatched == null)
			{
				// 該当するものがなかった場合はただちに処理を終える
				return false;
			}

			// 解決状況を記録した辞書を参照し、すでに解決済みでないかチェックする
			if (ctx.Any(b => b.Key.Equals(nameMatched) && b.Value.Count() > 0))
			{
				// 解決済みの場合はただちに処理を終える
				return false;
			}

			try
			{
				// オプション定義に登録されたデリゲートを呼び出す
				nameMatched.SetterDelegate(nameMatched.HasArgument ? value : string.Empty);
				// 値解決は成功したものとしてtrueを返す
				return true;
			}
			catch (Exception ex)
			{
				// 例外がスローされた場合
				// 処理状況を示す情報とともにラップして再スローする
				throw new ParseException
				(ParseException.ExceptionCategory.SetterErrorHasOccurred,
				 nameMatched, value, ex);
			}
		}

		IDictionary<string, string> LoadSettings()
		{
			var r = new Dictionary<string, string>();
			var settings = System.Configuration.ConfigurationManager.AppSettings;
			foreach (var k in settings.AllKeys)
			{
				r[k] = settings[k];
			}
			return r;
		}
	}
}
