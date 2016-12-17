using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// <see cref="ICommandLine&lt;T&gt;"/>インターフェースのためのユーティリティです。
	/// <see cref="Builder(string)"/>メソッドを通じてビルダー・オブジェクトを取得することができます。
	/// </summary>
	public class CommandLine
	{
		/// <summary>
		/// 新しいビルダーを生成します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="commandName">コマンド名</param>
		public static CommandLineBuilder<T> Builder<T>(string commandName)
		{
			return new CommandLineBuilder<T>(commandName);
		}
	}

	/// <summary>
	/// <see cref="ICommandLine&lt;T&gt;"/>インターフェースを実装した具象クラスです。
	/// </summary>
	internal class CommandLine<T> : ICommandLine<T>
	{
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
		/// パーサを取得します。
		/// このメソッドにより生成されたパーサは、
		/// <paramref name="valueObject"/>で指定されたオブジェクトに
		/// コマンドラインから読み取った値を設定します。
		/// </summary>
		/// <returns>パーサ</returns>
		/// <param name="valueObject">バリュー・オブジェクト</param>
		public IParser<T> GetParser(T valueObject)
		{
			if (valueObject == null)
			{
				throw new ArgumentNullException(nameof(valueObject));
			}
			return new Parser<T>(this, valueObject);
		}
		/// <summary>
		/// パーサを取得します。
		/// このメソッドにより生成されたパーサは、
		/// <paramref name="valueObjectSupplier"/>で指定されたデリゲートが返すオブジェクトに
		/// コマンドラインから読み取った値を設定します。
		/// </summary>
		/// <returns>パーサ</returns>
		/// <param name="valueObjectSupplier">バリュー・オブジェクトを返すデリゲート</param>
		public IParser<T> GetParser(Func<T> valueObjectSupplier)
		{
			if (valueObjectSupplier == null)
			{
				throw new ArgumentNullException(nameof(valueObjectSupplier));
			}
			return new Parser<T>(this, valueObjectSupplier);
		}
		/// <summary>
		/// パーサを取得します。
		/// このメソッドにより生成されたパーサは、
		/// <typeparamref name="T"/>のデフォルト・コンストラクタを使用して
		/// バリュー・オブジェクトのインスタンスを初期化して、
		/// そのインスタンスにコマンドラインから読み取った値を設定します。
		/// </summary>
		/// <returns>パーサ</returns>
		public IParser<T> GetParser()
		{
			return new Parser<T>(this);
		}
	}
}
