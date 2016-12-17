using System;
namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドライン・オプションの定義を組み立てるビルダーです。
	/// </summary>
	public class OptionBuilder<T>
	{
		private static readonly Action<T, string> noop = (o, s) => { };

		private readonly string name;
		private string alternativeName = string.Empty;
		private string settingName = string.Empty;
		private bool required = false;
		private bool hasArgument = false;
		private string argumentName = string.Empty;
		private bool multiple = false;
		private string description = string.Empty;
		private Action<T, string> setterDelegate = noop;

		internal OptionBuilder(string n)
		{
			if (n == null)
			{
				throw new ArgumentNullException(nameof(n));
			}
			if (n.Length == 0)
			{
				throw new ArgumentException("Option name must not be empty.");
			}
			name = n;
		}

		/// <summary>
		/// コマンドライン・オプションの別名を設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="an">コマンドライン・オプションの別名</param>
		public OptionBuilder<T> AlternativeName(string an)
		{
			alternativeName = an == null ? string.Empty : an;
			return this;
		}
		/// <summary>
		/// アプリケーション構成ファイルの設定情報のキー名を設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="sn">設定情報のキー名</param>
		public OptionBuilder<T> SettingName(string sn)
		{
			settingName = sn == null ? string.Empty : sn;
			return this;
		}
		/// <summary>
		/// オプションが必須かどうかを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="r"><c>true</c>の場合は必須</param>
		public OptionBuilder<T> Required(bool r)
		{
			required = r;
			return this;
		}
		/// <summary>
		/// オプションが必須であるものとして設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		public OptionBuilder<T> Required()
		{
			return Required(true);
		}
		/// <summary>
		/// オプションが引数をとるかどうかを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="ha"><c>true</c>の場合は引数をとる</param>
		public OptionBuilder<T> HasArgument(bool ha)
		{
			hasArgument = ha;
			return this;
		}
		/// <summary>
		/// オプションが引数を取るものとして設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		public OptionBuilder<T> HasArgument()
		{
			return HasArgument(true);
		}
		/// <summary>
		/// オプションの引数名を設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="argName">Argument name.</param>
		public OptionBuilder<T> ArgumentName(string argName)
		{
			argumentName = argName == null ? string.Empty : argName;
			return this;
		}
		/// <summary>
		/// オプションが複数回指定ができるかどうかを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="m"><c>true</c>の場合は複数回指定できる</param>
		public OptionBuilder<T> Multiple(bool m)
		{
			multiple = m;
			return this;
		}
		/// <summary>
		/// オプションが複数回指定できるものとして設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		public OptionBuilder<T> Multiple()
		{
			return Multiple(true);
		}
		/// <summary>
		/// オプションの説明を設定します。
		/// </summary>
		/// <param name="d">オプションの説明</param>
		public OptionBuilder<T> Description(string d)
		{
			description = d == null ? string.Empty : d;
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// デリゲートの第1引数にはコマンドラインから読み取られたオプションの引数が設定されます。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, string> setter)
		{
			setterDelegate = setter == null ? noop : setter;
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// 引数をとらないデリゲートは引数をとらないオプション（いわゆる「フラグ」）のために使用します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o);
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// デリゲートの第1引数にはコマンドラインから読み取られたオプションの引数が設定されます。
		/// 型変換には<see cref="long.Parse(string)"/>が利用されます。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, long> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o, long.Parse(s));
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// デリゲートの第1引数にはコマンドラインから読み取られたオプションの引数が設定されます。
		/// 型変換には<see cref="int.Parse(string)"/>が利用されます。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, int> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o, int.Parse(s));
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// デリゲートの第1引数にはコマンドラインから読み取られたオプションの引数が設定されます。
		/// 型変換には<see cref="double.Parse(string)"/>が利用されます。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, double> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o, double.Parse(s));
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// デリゲートの第1引数にはコマンドラインから読み取られたオプションの引数が設定されます。
		/// 型変換には<see cref="DateTime.Parse(string)"/>が利用されます。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, DateTime> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o, DateTime.Parse(s));
			return this;
		}
		/// <summary>
		/// オプション引数を他の任意のオブジェクトに設定するためのデリゲートを設定します。
		/// </summary>
		/// <returns>ビルダー</returns>
		/// <param name="setter">デリゲート</param>
		public OptionBuilder<T> SetterDelegate(Action<T, bool> setter)
		{
			setterDelegate = setter == null ? noop : (o, s) => setter(o, !false.ToString().Equals(s));
			return this;
		}
		/// <summary>
		/// 新しい<see cref="IOption"/>のインスタンスを構築します。
		/// </summary>
		/// <returns>コマンドライン・オプションの定義情報</returns>
		public IOption<T> Build()
		{
			return new Option<T>(name, alternativeName, settingName, required,
							  hasArgument, argumentName, multiple, description, setterDelegate);
		}
	}
}
