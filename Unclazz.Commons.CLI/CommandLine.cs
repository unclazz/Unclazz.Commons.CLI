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

		public void Parse(IEnumerable<string> rawArgs)
		{
			try
			{
				ResolveOptions(rawArgs, LoadSettings());
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
		public IEnumerable<IOption> Options { get; }
		public Action<IEnumerable<string>> SetterDelegate { get; }

		internal CommandLine(string cn, string d, bool cs,
		                     IEnumerable<IOption> os,
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
			Options = os;
			SetterDelegate = sd;
		}

		bool HaveSameName(IOption opt, string specified)
		{
			if (CaseSensitive)
			{
				return opt.Name.Equals(specified) ||
						  opt.AlternativeName.Equals(specified);
			}
			else {
				return opt.Name.ToUpper().Equals(specified.ToUpper()) ||
						  opt.Name.ToUpper().Equals(specified.ToUpper());
			}
		}

		void ResolveOptions(IEnumerable<string> rawArgs, IDictionary<string,string> settings)
		{
			var toArgResolve = new Queue<string>(rawArgs);
			var notArgResolved = new Queue<string>();
			var ctx = new Dictionary<IOption, List<string>>();

			foreach (var o in Options)
			{
				ctx.Add(o, new List<string>());
			}

			while (toArgResolve.Count > 0)
			{
				var former = toArgResolve.Dequeue();
				var latter = toArgResolve.Count == 0
							 ? string.Empty : toArgResolve.Peek();

				var result = ResolveArguments(former, latter, ctx);
				if (result)
				{
					var opt = ctx.First(a => HaveSameName(a.Key, former));

					if (opt.Key.HasArgument)
					{
						opt.Value.Add(latter);
						toArgResolve.Dequeue();
					}
					else {
						opt.Value.Add(string.Empty);
					}
				}
				else {
					notArgResolved.Enqueue(former);
				}
			}

			foreach (var item in settings)
			{
				var result = ResolveSettings(item.Key, item.Value, ctx);
				if (result)
				{
					var opt = ctx.First(a => a.Key.SettingName.Equals(item.Key));
					if (opt.Key.HasArgument)
					{
						opt.Value.Add(settings[item.Key]);
					}
					else {
						opt.Value.Add(string.Empty);
					}
				}
			}

			SetterDelegate(notArgResolved);

			var found = ctx.Where(kv => kv.Value.Count() > 0).Select(kv => kv.Key).ToList();
			var missed = Options.Where(a => a.Required).FirstOrDefault(a => !found.Contains(a));

			if (missed != null)
			{
				throw new ParseException
				(ParseException.ExceptionCategory.RequiredOptionNotFound,
				 missed, null);
			}
		}

		bool ResolveArguments(string former, string latter,
			 Dictionary<IOption, List<string>> ctx)
		{
			var nameMatched = Options
				.FirstOrDefault(a => HaveSameName(a, former));

			if (nameMatched == null)
			{
				return false;
			}

			if (!nameMatched.Multiple &&
				ctx.Any(b => b.Key.Equals(nameMatched) && b.Value.Count() > 0))
			{
				throw new ParseException
				(ParseException.ExceptionCategory.DuplicatedOption,
				 nameMatched, latter);
			}

			try
			{
				nameMatched.SetterDelegate(latter);
				return true;
			}
			catch (Exception ex)
			{
				throw new ParseException
				(ParseException.ExceptionCategory.SetterErrorHasOccurred,
				 nameMatched, latter, ex);
			}
		}

		bool ResolveSettings(string key, string value,
			 Dictionary<IOption, List<string>> ctx)
		{
			var nameMatched = Options
				.FirstOrDefault(a => a.SettingName.Equals(key));

			if (nameMatched == null)
			{
				return false;
			}

			if (ctx.Any(b => b.Key.Equals(nameMatched) && b.Value.Count() > 0))
			{
				return false;
			}

			try
			{
				nameMatched.SetterDelegate(value);
				return true;
			}
			catch (Exception ex)
			{
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
