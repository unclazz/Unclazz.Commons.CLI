using NUnit.Framework;
using Unclazz.Commons.CLI;

namespace Test.Unclazz.Commons.CLI
{
	[TestFixture()]
	public class OptionBuilderTest
	{
		[Test()]
		public void Description_SetDescriptionOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Description("foo is foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Description(null));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();

			// Assert
			Assert.That(cl0.Options["foo"].Description, Is.EqualTo("foo is foo"));
			Assert.That(cl1.Options["foo"].Description, Is.EqualTo(string.Empty));
		}

		[Test()]
		public void HasArgument_SetHasArgumentOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
			                    .AddOption(Option.Builder("foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .HasArgument(true));
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .HasArgument());
			var b3 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .HasArgument(false));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();
			var cl2 = b2.Build();
			var cl3 = b3.Build();

			// Assert
			Assert.That(cl0.Options["foo"].HasArgument, Is.False);
			Assert.That(cl1.Options["foo"].HasArgument, Is.True);
			Assert.That(cl2.Options["foo"].HasArgument, Is.True);
			Assert.That(cl3.Options["foo"].HasArgument, Is.False);
		}

		[Test()]
		public void Required_SetRequiredOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
			                    .AddOption(Option.Builder("foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Required(true));
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Required());
			var b3 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Required(false));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();
			var cl2 = b2.Build();
			var cl3 = b3.Build();

			// Assert
			Assert.That(cl0.Options["foo"].Required, Is.False);
			Assert.That(cl1.Options["foo"].Required, Is.True);
			Assert.That(cl2.Options["foo"].Required, Is.True);
			Assert.That(cl3.Options["foo"].Required, Is.False);
		}

		[Test()]
		public void Multiple_SetMultipleOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
			                    .AddOption(Option.Builder("foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Multiple(true));
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Multiple());
			var b3 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .Multiple(false));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();
			var cl2 = b2.Build();
			var cl3 = b3.Build();

			// Assert
			Assert.That(cl0.Options["foo"].Multiple, Is.False);
			Assert.That(cl1.Options["foo"].Multiple, Is.True);
			Assert.That(cl2.Options["foo"].Multiple, Is.True);
			Assert.That(cl3.Options["foo"].Multiple, Is.False);
		}

		[Test()]
		public void AlternativeName_SetAlternativeNameOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
			                    .AddOption(Option.Builder("foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .AlternativeName("altFoo"));
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .AlternativeName(null));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();
			var cl2 = b2.Build();

			// Assert
			Assert.That(cl0.Options["foo"].AlternativeName, Is.EqualTo(string.Empty));
			Assert.That(cl1.Options["foo"].AlternativeName, Is.EqualTo("altFoo"));
			Assert.That(cl2.Options["foo"].AlternativeName, Is.EqualTo(string.Empty));
		}

		[Test()]
		public void SettingName_SetSettingNameOfOption()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
			                    .AddOption(Option.Builder("foo"));
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SettingName("altFoo"));
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SettingName(null));

			// Act
			var cl0 = b0.Build();
			var cl1 = b1.Build();
			var cl2 = b2.Build();

			// Assert
			Assert.That(cl0.Options["foo"].SettingName, Is.EqualTo(string.Empty));
			Assert.That(cl1.Options["foo"].SettingName, Is.EqualTo("altFoo"));
			Assert.That(cl2.Options["foo"].SettingName, Is.EqualTo(string.Empty));
		}

		[Test()]
		public void SetterDelegate_SetSetterDelegateOfOption()
		{
			// Arrange
			string s0 = null;
			var b0 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SetterDelegate((s) => s0 = s));
			int i1 = 0;
			var b1 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SetterDelegate((int i) => i1 = i));
			double d2 = 0.0;
			var b2 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SetterDelegate((double d) => d2 = d));
			bool bl3 = false;
			var b3 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
			                               .SetterDelegate(() => bl3 = true));
			bool bl4 = false;
			var b4 = CommandLine.Builder("test.exe")
								.AddOption(Option.Builder("foo")
										   .SetterDelegate((bool b) => bl4 = b));

			// Act
			var cl0 = b0.Build();
			cl0.Options["foo"].SetterDelegate("bar");
			var cl1 = b1.Build();
			cl1.Options["foo"].SetterDelegate("123");
			var cl2 = b2.Build();
			cl2.Options["foo"].SetterDelegate("123.456");
			var cl3 = b3.Build();
			cl3.Options["foo"].SetterDelegate("bar");
			var cl4 = b4.Build();
			cl4.Options["foo"].SetterDelegate(string.Empty);

			// Assert
			Assert.That(s0, Is.EqualTo("bar"));
			Assert.That(i1, Is.EqualTo(123));
			Assert.That(d2, Is.EqualTo(123.456));
			Assert.That(bl3, Is.EqualTo(true));
			Assert.That(bl4, Is.EqualTo(true));

			// Act & Assert #2
			cl4.Options["foo"].SetterDelegate(true.ToString());
			Assert.That(bl4, Is.EqualTo(true));
			cl4.Options["foo"].SetterDelegate(false.ToString());
			Assert.That(bl4, Is.EqualTo(false));
		}
	}
}
