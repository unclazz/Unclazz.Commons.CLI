using NUnit.Framework;
using System;
using Unclazz.Commons.CLI;
using System.Linq;

namespace Test.Unclazz.Commons.CLI
{
	[TestFixture()]
	public class OptionBuilderTest
	{
		[Test()]
		public void Build_ReturnsNewInstanceOfICommandLine()
		{
			// Arrange
			var b = CommandLine.Builder("test.exe")
							   .CaseSensitive()
							   .AddOption("foo")
							   .HasArgument()
							   .Required()
							   .AndOption("bar")
							   .HasArgument()
							   .AndOption("baz");

			// Act
			ICommandLine cl = b.Build();

			// Assert
			Assert.True(cl.Options.All(o =>
			{
				return new string[] { "foo", "bar", "baz" }.Contains(o.Name);
			}));
		}

		[Test()]
		public void Build_WhenNoOptionSpecified_ReturnsNewInstanceOfICommandLine()
		{
			// Arrange
			var b = CommandLine.Builder("test.exe")
							   .CaseSensitive();

			// Act
			// Assert
			Assert.Throws<ApplicationException>(() =>
			{
				b.Build();
			});
		}

		[Test()]
		public void Description_SetDescriptionOfCommandLine()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
							   .CaseSensitive()
								.AddOption("foo");
			var b1 = CommandLine.Builder("test.exe")
			                    .Description("test command")
							   .CaseSensitive()
								.AddOption("foo");

			// Act
			ICommandLine cl0 = b0.Build();
			ICommandLine cl1 = b1.Build();

			// Assert
			Assert.That(cl0.Description, Is.EqualTo(string.Empty));
			Assert.That(cl1.Description, Is.EqualTo("test command"));
		}

		[Test()]
		public void CaseSensitive_SetCaseSensitiveOfCommandLine()
		{
			// Arrange
			var b0 = CommandLine.Builder("test.exe")
							   .CaseSensitive(false)
								.AddOption("foo");
			var b1 = CommandLine.Builder("test.exe")
							   .CaseSensitive(true)
								.AddOption("foo");
			var b2 = CommandLine.Builder("test.exe")
							   .CaseSensitive()
								.AddOption("foo");

			// Act
			ICommandLine cl0 = b0.Build();
			ICommandLine cl1 = b1.Build();
			ICommandLine cl2 = b2.Build();

			// Assert
			Assert.False(cl0.CaseSensitive);
			Assert.True(cl1.CaseSensitive);
			Assert.True(cl2.CaseSensitive);
		}
	}
}
