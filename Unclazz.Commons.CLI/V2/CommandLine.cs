using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// <see cref="ICommandLine&lt;T&gt;"/>インターフェースを実装した具象クラスです。
	/// <see cref="Builder(string)"/>メソッドを通じてビルダー・オブジェクトを取得することができます。
	/// </summary>
	public class CommandLine<T> : ICommandLine<T>
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

		public string CommandName { get; }
		public string Description { get; }
		public bool CaseSensitive { get; }
		public OptionCollection<T> Options { get; }
		public IEnumerable<string> ArgumentNames { get; }
		public Action<T, IEnumerable<string>> SetterDelegate { get; }

		internal CommandLine(string cn, string d, bool cs,
							 IEnumerable<IOption<T>> os,
							 IEnumerable<string> ags,
							 Action<T, IEnumerable<string>> sd)
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
			Options = new OptionCollection<T>(os);
			ArgumentNames = ags;
			SetterDelegate = sd;
		}

		/// <summary>
		/// Gets the parser.
		/// </summary>
		/// <returns>The parser.</returns>
		/// <param name="valueObject">Value object.</param>
		public IParser<T> GetParser(T valueObject)
		{
			if (valueObject == null)
			{
				throw new ArgumentNullException(nameof(valueObject));
			}
			return new Parser<T>(this, valueObject);
		}
	}
}
