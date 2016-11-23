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
	}
}
