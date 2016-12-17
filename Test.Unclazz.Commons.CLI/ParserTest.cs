using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unclazz.Commons.CLI;

namespace Test.Unclazz.Commons.CLI
{

	[TestFixture()]
	public class ParserTest
	{
		IEnumerable<string> Arguments(params string[] args)
		{
			return args;
		}

		[Test()]
		public void ParserHasBeenInitializedWithSupplierDelegate_ItsParseMethod_ReturnsVoThatSuppliedByDelegate()
		{
			// Arrange
			var cl0 = CommandLine.Builder<object>("foo.exe")
								 .AddOption(Option.Builder<object>("/f"))
								 .Build();
			var v0 = new object();
			var p0 = cl0.GetParser(() => v0);

			// Act
			var r0 = p0.Parse(new string[] { "/f" });

			// Assert
			Assert.True(object.ReferenceEquals(v0, r0));
		}

		[Test()]
		public void Parse_WhenSupplierDelegateThrowsException_ThrowsTheException()
		{
			// Arrange
			var cl0 = CommandLine.Builder<object>("foo.exe")
								 .AddOption(Option.Builder<object>("/f"))
								 .Build();
			var p0 = cl0.GetParser(() => 
			{
				throw new Exception();
			});

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				p0.Parse(new string[] { "/f" });
			});
		}

		[Test()]
		public void Parse_Flag_DelegateHasnotArgument()
		{
			// Arrange
			bool v0 = false;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder<object>("test.exe")
			                     .SetterDelegate((o, ss) => vs = ss.ToList())
			                     .AddOption(Option.Builder<object>("/f")
			                     	.SetterDelegate(o => v0 = true).Build())
			                     .Build();

			// Act
			cl0.GetParser().Parse(Arguments("/f", "/b", "baz"));

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
			var cl0 = CommandLine.Builder<object>("test.exe")
			                     .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                                .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build();

			// Act
			cl0.GetParser().Parse(Arguments("/f", "/b", "baz"));

			// Assert
			Assert.That(v0, Is.EqualTo(string.Empty));
			Assert.That(vs.Count, Is.EqualTo(2));
			Assert.That(vs[0], Is.EqualTo("/b"));
			Assert.That(vs[1], Is.EqualTo("baz"));
		}

		[Test()]
		public void Parse_Flag_DelegateHasAnArgument_Case2()
		{
			// Arrange
			bool v0 = false;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder<object>("test.exe")
			                     .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                                .SettingName("Foo")
								 			.SetterDelegate((object o, bool v) => v0 = v)
			                                .Required()
											.Build()).Build();
			var dict = new Dictionary<string, string>();

			// Act & Assert #1
			dict["Foo"] = true.ToString();
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(true));

			// Act & Assert #2
			v0 = true;
			dict["Foo"] = false.ToString();
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(false));

			// Act & Assert #3
			v0 = true;
			dict["Foo"] = "NO";
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(false));

			// Act & Assert #4
			v0 = true;
			dict["Foo"] = "0";
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(false));

			// Act & Assert #5
			v0 = true;
			dict["Foo"] = "N";
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(false));

			// Act & Assert #6
			v0 = true;
			dict["Foo"] = "F";
			cl0.GetParser().Parse(Arguments("/b", "baz"), dict);
			Assert.That(v0, Is.EqualTo(false));
		}

		[Test()]
		public void Parse_Flag_DontDependsOnOrder()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder<object>("test.exe")
			                     .SetterDelegate((o, ss) => vs = ss.ToList())
			                     .AddOption(Option.Builder<object>("/f")
								 	.SetterDelegate((o, s) => v0 = s).Build())
			                     .Build();

			// Act
			cl0.GetParser().Parse(Arguments("/b", "/f", "baz"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
			                     .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                                .SetterDelegate((o, s) => v0 = s).Build())
			                     .Build().GetParser();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				p0.Parse(Arguments("/f", "/b", "baz"));
			});
		}

		[Test()]
		public void Parse_HasArg_DelegateHasnotArgument()
		{
			// Arrange
			bool v0 = false;
			IList<string> vs = null;
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                     .HasArgument()
								 .SetterDelegate((o) => v0 = true)
			                                .Build()).Build().GetParser();

			// Act
			p0.Parse(Arguments("/f", "bar", "baz"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                     .HasArgument()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			p0.Parse(Arguments("/f", "bar", "baz"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .HasArgument()
								 .Required()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			p0.Parse(Arguments("/f", "bar", "baz"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .HasArgument()
			                     .Required()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				p0.Parse(Arguments("foo", "bar", "baz"));
			});
		}

		[Test()]
		public void Parse_Multiple_FoundInArguments()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .HasArgument()
			                     .Multiple()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			p0.Parse(Arguments("abc", "/f", "bar", "/f", "baz", "123"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .HasArgument()
								 .Multiple()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			p0.Parse(Arguments("foo", "bar", "baz"));

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
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .HasArgument()
			                     .Required()
								 .Multiple()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				p0.Parse(Arguments("foo", "bar", "baz"));
			});
		}

		[Test()]
		public void Parse_Required_FoundInArguments_WithAppSettings()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
			                     .SettingName("Test.Foo")
								 .HasArgument()
								 .Required()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build();
			var dict = new Dictionary<string, string>();
			dict.Add("Test.Foo", "BAR");

			// Act
			cl0.GetParser().Parse(Arguments("/f", "bar", "baz"), dict);

			// Assert
			Assert.That(v0, Is.EqualTo("bar"));
			Assert.That(vs.Count, Is.EqualTo(1));
		}

		[Test()]
		public void Parse_Required_NotFoundInArguments_WithAppSettings()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var cl0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .SettingName("Test.Foo")
								 .HasArgument()
								 .Required()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build();

			var dict = new Dictionary<string, string>();
			dict.Add("Test.Foo", "BAR");

			// Act
			cl0.GetParser().Parse(Arguments("/b", "bar", "baz"), dict);

			// Assert
			Assert.That(v0, Is.EqualTo("BAR"));
			Assert.That(vs.Count, Is.EqualTo(3));
		}

		[Test()]
		public void Parse_Required_NotFoundInArgumentsAndAppSettings()
		{
			// Arrange
			string v0 = null;
			IList<string> vs = null;
			var p0 = CommandLine.Builder<object>("test.exe")
								 .SetterDelegate((o, ss) => vs = ss.ToList())
								 .AddOption(Option.Builder<object>("/f")
								 .SettingName("Test.Foo")
								 .HasArgument()
								 .Required()
								 .SetterDelegate((object o, string s) => v0 = s)
			                                .Build()).Build().GetParser();

			var dict = new Dictionary<string, string>();
			dict.Add("Test.Bar", "BAR");

			// Act
			// Assert
			Assert.Throws<ParseException>(() =>
			{
				p0.Parse(Arguments("foo", "bar", "baz"));
			});
		}
	}
}
