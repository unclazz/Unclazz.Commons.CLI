using System;
using System.Collections.Generic;

namespace Unclazz.Commons.CLI
{
	public class CommandLineBuilder
	{
		private static readonly Action<IEnumerable<string>> noop = (ss) => { };

		private readonly string commandName = string.Empty;
		private string description = string.Empty;
		private bool caseSensitive = true;
		private ISet<IOption> options = new HashSet<IOption>();
		private Action<IEnumerable<string>> setterDelegate = noop;

		internal CommandLineBuilder(string cn)
		{
			if (cn == null)
			{
				throw new ArgumentNullException(nameof(cn));
			}
			commandName = cn;
		}

		public CommandLineBuilder Description(string d)
		{
			description = d == null ? string.Empty : d;
			return this;
		}
		public CommandLineBuilder CaseSensitive(bool cs)
		{
			caseSensitive = cs;
			return this;
		}
		public CommandLineBuilder CaseSensitive()
		{
			return CaseSensitive(true);
		}
		public CommandLineBuilder AddOption(IOption o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(nameof(o));
			}
			options.Add(o);
			return this;
		}
		public OptionBuilder AddOption(string n)
		{
			return new OptionBuilder(this, n);
		}
		public CommandLineBuilder SetterDelegate(Action<IEnumerable<string>> setter)
		{
			setterDelegate = setter == null ? noop : setter;
			return this;
		}
		public ICommandLine Build()
		{
			if (options.Count == 0)
			{
				throw new ApplicationException("No option specified.");
			}
			return new CommandLine(commandName, description, caseSensitive, 
			                       options, setterDelegate);
		}
	}


}
