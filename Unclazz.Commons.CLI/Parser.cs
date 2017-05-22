using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	class Parser<T> : IParser<T>
	{
		private readonly ICommandLine<T> cl;
		private readonly Func<T> supplier;

		internal Parser(ICommandLine<T> commandLine, Func<T> valueObjectSupplier)
		{
			cl = commandLine;
			supplier = valueObjectSupplier;
		}

		internal Parser(ICommandLine<T> commandLine)
		{
			cl = commandLine;
			supplier = Activator.CreateInstance<T>;
		}

		internal Parser(ICommandLine<T> commandLine, T valueObject)
		{
			cl = commandLine;
			supplier = () => valueObject;
		}

		public T Parse(IEnumerable<string> arguments)
		{
			return Parse(arguments, LoadSettings());
		}

		public T Parse(IEnumerable<string> arguments, IDictionary<string, string> settings)
		{
			try
			{
				var vo = supplier();
				ResolveOptions(arguments, settings, vo);
				return vo;
			}
			catch (Exception ex)
			{
				throw MakeParseException
				(ParseExceptionCategory.UnexpectedErrorHasOccurred,
				 null, null, ex);
			}
		}

		/// <summary>
		/// オプションの名前・別名とコマンドラインで実際に指定された名前とを照合します。
		/// 照合に際して<see cref="ICommandLine&lt;T&gt;.CaseSensitive"/>の値が考慮されます。
		/// </summary>
		/// <returns>照合が成功した場合は<c>true</c></returns>
		/// <param name="opt">オプションの定義</param>
		/// <param name="specifiedName">実際に指定されたオプションの名前</param>
		bool HaveSameName(IOption<T> opt, string specifiedName)
		{
			if (cl.CaseSensitive)
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
        /// <param name="vo">パース結果を設定するバリュー・オブジェクト</param>
		void ResolveOptions(IEnumerable<string> rawArgs, IDictionary<string, string> settings, T vo)
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
			var ctx = new Dictionary<IOption<T>, List<string>>();
			foreach (var o in cl.Options)
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
				var result = ResolveOptionsWithArguments(former, latter, ctx, vo);
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
				var result = ResolveOptionsWithAppSettings(item.Key, item.Value, ctx, vo);
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
						var upper = item.Value.ToUpper();
						if (upper.Equals("FALSE") || upper.Equals("NO") ||
						upper.Equals("0") || upper.Equals("F") || upper.Equals("N"))
						{
							opt.Value.Add(false.ToString());
						}
						else {
							opt.Value.Add(true.ToString());
						}
					}
				}
			}

			// 3. コマンドライン引数の処理（後段階）

			// 解決不可キューの内容を「残りの引数」としてデリゲートに渡す
			cl.SetterDelegate(vo, cantArgResolved);

			// 4. 必死オプションのチェック

			// 必須にもかかわらず一連の処理で解決のできていないオプションを検索
			var missed = ctx.Where(kv => kv.Key.Required && kv.Value.Count() == 0)
						   .Select(kv => kv.Key).FirstOrDefault();
			if (missed != null)
			{
				// 該当するものが1件でもあった場合は例外をスローする
				throw MakeParseException(
					ParseExceptionCategory.RequiredOptionNotFound,
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
        /// <param name="vo">パース結果を設定するバリュー・オブジェクト</param>
        bool ResolveOptionsWithArguments(string former, string latter,
			 Dictionary<IOption<T>, List<string>> ctx, T vo)
		{
			// オプション定義うちで名前もしくは別名が一致するものを検索
			var nameMatched = cl.Options
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
				throw MakeParseException(ParseExceptionCategory.DuplicatedOption,
				 nameMatched, latter);
			}

			try
			{
				// オプション定義に登録されたデリゲートを呼び出す
				nameMatched.SetterDelegate(vo, nameMatched.HasArgument ? latter : string.Empty);
				// 値解決は成功したものとしてtrueを返す
				return true;
			}
			catch (Exception ex)
			{
				// 例外がスローされた場合
				// 処理状況を示す情報とともにラップして再スローする
				throw MakeParseException
				(ParseExceptionCategory.SetterErrorHasOccurred,
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
        /// <param name="vo">パース結果を設定するバリュー・オブジェクト</param>
        bool ResolveOptionsWithAppSettings(string key, string value,
			 Dictionary<IOption<T>, List<string>> ctx, T vo)
		{
			// オプション定義うちでキー名が該当するものを検索
			var nameMatched = cl.Options
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
				if (nameMatched.HasArgument)
				{
					nameMatched.SetterDelegate(vo, value);
				}
				else {
					var upper = value.ToString().ToUpper();
					if (upper.Equals("FALSE") || upper.Equals("NO") ||
						upper.Equals("0") || upper.Equals("F") || upper.Equals("N"))
					{
						nameMatched.SetterDelegate(vo, false.ToString());
					}
					else {
						nameMatched.SetterDelegate(vo, true.ToString());
					}
				}

				// 値解決は成功したものとしてtrueを返す
				return true;
			}
			catch (Exception ex)
			{
				// 例外がスローされた場合
				// 処理状況を示す情報とともにラップして再スローする
				throw MakeParseException(
					ParseExceptionCategory.SetterErrorHasOccurred,
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

		ParseException MakeParseException(
			ParseExceptionCategory c,
			IOption<T> to, string tv, Exception e)
		{
			return new ParseException(c, to, tv, e);
		}

		ParseException MakeParseException(
			ParseExceptionCategory c,
			IOption<T> to, string tv)
		{
			return new ParseException(c, to, tv);
		}
	}
}
