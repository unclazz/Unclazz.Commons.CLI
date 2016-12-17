using System;
namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// <see cref="IOption&lt;T&gt;"/>を実装したクラスです。
	/// </summary>
	public class Option<T> : IOption<T>
	{
		/// <summary>
		/// 新しいビルダーを生成します。
		/// </summary>
		/// <param name="name">コマンドライン・オプションの名前</param>
		public static OptionBuilder<T> Builder(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			return new OptionBuilder<T>(name);
		}

		public string Name { get; }
		public string AlternativeName { get; }
		public string SettingName { get; }
		public bool Required { get; }
		public bool HasArgument { get; }
		public string ArgumentName { get; }
		public bool Multiple { get; }
		public string Description { get; }
		public Action<T, string> SetterDelegate { get; }

		internal Option(string n, string an, string sn, bool r, bool ha, string agn,
						bool m, string d, Action<T, string> sd)
		{
			if (IsNullOrEmpty(n) && IsNullOrEmpty(an))
			{
				throw new ArgumentException(string
				.Format("Name = {0}, AlternativeName = {1}.", n, an));
			}
			if (sd == null)
			{
				throw new ArgumentNullException(nameof(sd));
			}
			Name = n == null ? string.Empty : n;
			AlternativeName = an == null ? string.Empty : an;
			SettingName = sn == null ? string.Empty : sn;
			Required = r;
			HasArgument = ha;
			ArgumentName = agn == null ? string.Empty : agn;
			Multiple = m;
			Description = d;
			SetterDelegate = sd;
		}

		bool IsNullOrEmpty(string s)
		{
			return s == null || s.Length == 0;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			var other = obj as IOption;
			if (other == null)
			{
				return false;
			}
			return Name.Equals(other.Name);
		}
	}
}
