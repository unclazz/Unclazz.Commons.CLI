using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unclazz.Commons.CLI;

namespace Test.Unclazz.Commons.CLI
{
	[TestFixture()]
	public class CommandLineTest
	{
		IEnumerable<string> Arguments(params string[] args)
		{
			return args;
		}

		[Test()]
		public void Parse_Flag_DelegateHasnotArgument()
		{
			// Arrange
			bool v0 = false;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
			                     .SetterDelegate(ss => vs = ss.ToList())
			                     .AddOption("/f")
			                     .SetterDelegate(()=> v0 = true)
			                     .Build();

			// Act
			cl0.Parse(Arguments("/f", "/b", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo(true));
			Assert.That(vs.Count, Is.EqualTo(2));
			Assert.That(vs[0], Is.EqualTo("/b"));
			Assert.That(vs[1], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Flag_DelegateHasAnArgument()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("/f", "/b", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo(string.Empty));
			Assert.That(vs.Count, Is.EqualTo(2));
			Assert.That(vs[0], Is.EqualTo("/b"));
			Assert.That(vs[1], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Flag_DotDependsOnOrder()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("/b", "/f", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo(string.Empty));
			Assert.That(vs.Count, Is.EqualTo(2));
			Assert.That(vs[0], Is.EqualTo("/b"));
			Assert.That(vs[1], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Flag_DelegateHasAnArgument_TypeMismatch()
		{
			// Arrange
			int v0 = 0;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .SetterDelegate((int s) => v0 = s)
								 .Build();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				cl0.Parse(Arguments("/f", "/b", "baz"));
			});
		}

		[Test()]
		public void Parse_HasArg_DelegateHasnotArgument()
		{
			// Arrange
			bool v0 = false;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
			                     .HasArgument()
								 .SetterDelegate(() => v0 = true)
								 .Build();

			// Act
			cl0.Parse(Arguments("/f", "bar", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo(true));
			Assert.That(vs.Count, Is.EqualTo(1));
			Assert.That(vs[0], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_HasArg_DelegateHasAnArgument()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
			                     .HasArgument()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("/f", "bar", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo("bar"));
			Assert.That(vs.Count, Is.EqualTo(1));
			Assert.That(vs[0], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Required_FoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .HasArgument()
								 .Required()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("/f", "bar", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo("bar"));
			Assert.That(vs.Count, Is.EqualTo(1));
			Assert.That(vs[0], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Required_NotFoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .HasArgument()
			                     .Required()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				cl0.Parse(Arguments("foo", "bar", "baz"));
			});
		}

		[Test()]
		public void Parse_Multiple_FoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .HasArgument()
			                     .Multiple()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("abc", "/f", "bar", "/f", "baz", "123"));

			// Assert
			Assert.That(v0, Is.EqualTo("baz"));
			Assert.That(vs.Count, Is.EqualTo(2));
		}

		[Test()]
		public void Parse_Multiple_NotFoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .HasArgument()
								 .Multiple()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			cl0.Parse(Arguments("foo", "bar", "baz"));

			// Assert
			Assert.That(v0, Is.Null);
			Assert.That(vs.Count, Is.EqualTo(3));
		}

		[Test()]
		public void Parse_RequiedAndMultiple_NotFoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder("test.exe")
								 .SetterDelegate(ss => vs = ss.ToList())
								 .AddOption("/f")
								 .HasArgument()
			                     .Required()
								 .Multiple()
								 .SetterDelegate((string s) => v0 = s)
								 .Build();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				cl0.Parse(Arguments("foo", "bar", "baz"));
			});
		}
	}
}
