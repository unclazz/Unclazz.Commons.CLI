using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	public class CommandLineBuilder<T>
	{
		private static readonly Action<T, IEnumerable<string>> noop = (o, ss) => { };

		private readonly string commandName = string.Empty;
		private string description = string.Empty;
		private bool caseSensitive = true;
		private ISet<IOption<T>> options = new HashSet<IOption<T>>();
		private IEnumerable<string> argumentNames = Enumerable.Empty<string>();
		private Action<T, IEnumerable<string>> setterDelegate = noop;

		internal CommandLineBuilder(string cn)
		{
			if (cn == null)
			{
				throw new ArgumentNullException(nameof(cn));
			}
			commandName = cn;
		}

		/// <summary>
		/// コマンドの説明を設定します。
		/// </summary>
		/// <param name="d">コマンドの説明</param>
		public CommandLineBuilder<T> Description(string d)
		{
			description = d == null ? string.Empty : d;
			return this;
		}
		/// <summary>
		/// コマンドラインのパースにおいて大文字小文字を区別するかどうかを設定します。
		/// デフォルトは<c>true</c>です。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="cs"><c>true</c>の場合は大文字小文字を区別する</param>
		public CommandLineBuilder<T> CaseSensitive(bool cs)
		{
			caseSensitive = cs;
			return this;
		}
		/// <summary>
		/// コマンドラインのパースにおいて大文字小文字を区別するように設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		public CommandLineBuilder<T> CaseSensitive()
		{
			return CaseSensitive(true);
		}
		/// <summary>
		/// コマンドライン・オプションの定義を追加します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="o">コマンドライン・オプションの定義</param>
		public CommandLineBuilder<T> AddOption(IOption<T> o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(nameof(o));
			}
			options.Add(o);
			return this;
		}
		/// <summary>
		/// コマンドライン・オプションの定義を追加します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="b">コマンドライン・オプションのビルダー</param>
		public CommandLineBuilder<T> AddOption(OptionBuilder<T> b)
		{
			return AddOption(b.Build());
		}
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」の名前のシーケンスを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="argNames">「残りの引数」の名前</param>
		public CommandLineBuilder<T> ArgumentNames(params string[] argNames)
		{
			argumentNames = argNames == null ? Enumerable.Empty<string>() : argNames;
			return this;
		}
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」を処理するためのデリゲートを設定します。
		/// デフォルト値として利用されているデリゲートは受け取った値を単に破棄します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public CommandLineBuilder<T> SetterDelegate(Action<T, IEnumerable<string>> setter)
		{
			setterDelegate = setter == null ? noop : setter;
			return this;
		}
		/// <summary>
		/// コマンドラインの定義を組み立てます。
		/// </summary>
		/// <returns>コマンドラインの定義</returns>
		/// <exception cref="ApplicationException">オプションの定義が1つも指定されていない場合</exception>
		public ICommandLine<T> Build()
		{
			if (options.Count == 0)
			{
				throw new ApplicationException("No option specified.");
			}
			return new CommandLine<T>(commandName, description, caseSensitive,
								   options, argumentNames, setterDelegate);
		}
	}
}
