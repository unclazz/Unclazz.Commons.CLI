using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドラインの定義を組み立てるビルダーです。
	/// ビルダーのインスタンスは<see cref="CommandLine.Builder(string)"/>メソッドを通じて得られます。
	/// </summary>
	public class CommandLineBuilder
	{
		private static readonly Action<IEnumerable<string>> noop = (ss) => { };

		private readonly string commandName = string.Empty;
		private string description = string.Empty;
		private bool caseSensitive = true;
		private ISet<IOption> options = new HashSet<IOption>();
		private IEnumerable<string> argumentNames = Enumerable.Empty<string>();
		private Action<IEnumerable<string>> setterDelegate = noop;

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
		public CommandLineBuilder Description(string d)
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
		public CommandLineBuilder CaseSensitive(bool cs)
		{
			caseSensitive = cs;
			return this;
		}
		/// <summary>
		/// コマンドラインのパースにおいて大文字小文字を区別するように設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		public CommandLineBuilder CaseSensitive()
		{
			return CaseSensitive(true);
		}
		/// <summary>
		/// コマンドライン・オプションの定義を追加します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="o">コマンドライン・オプションの定義</param>
		public CommandLineBuilder AddOption(IOption o)
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
		public CommandLineBuilder AddOption(OptionBuilder b)
		{
			return AddOption(b.Build());
		}
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」の名前のシーケンスを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="argNames">「残りの引数」の名前</param>
		public CommandLineBuilder ArgumentNames(params string[] argNames)
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
		public CommandLineBuilder SetterDelegate(Action<IEnumerable<string>> setter)
		{
			setterDelegate = setter == null ? noop : setter;
			return this;
		}
		/// <summary>
		/// コマンドラインの定義を組み立てます。
		/// </summary>
		/// <returns>コマンドラインの定義</returns>
		/// <exception cref="ApplicationException">オプションの定義が1つも指定されていない場合</exception>
		public ICommandLine Build()
		{
			if (options.Count == 0)
			{
				throw new ApplicationException("No option specified.");
			}
			return new CommandLine(commandName, description, caseSensitive, 
			                       options, argumentNames, setterDelegate);
		}
	}
}
