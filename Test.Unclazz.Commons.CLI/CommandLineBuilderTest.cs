﻿using NUnit.Framework;
using System;
using Unclazz.Commons.CLI;
using System.Linq;

namespace Test.Unclazz.Commons.CLI
{
	[TestFixture()]
	public class CommandLineBuilderTest
	{
		[Test()]
		public void Build_ReturnsNewInstanceOfICommandLine()
		{
			// Arrange
			var b = CommandLine.Builder<object>("test.exe")
							   .CaseSensitive()
			                   .AddOption(Option.Builder<object>("foo")
							   .HasArgument()
			                              .Required().Build())
							   .AddOption(Option.Builder<object>("bar")
			                              .HasArgument().Build())
			                   .AddOption(Option.Builder<object>("baz").Build());

			// Act
			ICommandLine<object> cl = b.Build();

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
			var b = CommandLine.Builder<object>("test.exe")
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
			var b0 = CommandLine.Builder<object>("test.exe")
							   .CaseSensitive()
			                    .AddOption(Option.Builder<object>("foo").Build());
			var b1 = CommandLine.Builder<object>("test.exe")
			                    .Description("test command")
							   .CaseSensitive()
			                    .AddOption(Option.Builder<object>("foo").Build());

			// Act
			ICommandLine<object> cl0 = b0.Build();
			ICommandLine<object> cl1 = b1.Build();

			// Assert
			Assert.That(cl0.Description, Is.EqualTo(string.Empty));
			Assert.That(cl1.Description, Is.EqualTo("test command"));
		}

		[Test()]
		public void CaseSensitive_SetCaseSensitiveOfCommandLine()
		{
			// Arrange
			var b0 = CommandLine.Builder<object>("test.exe")
							   .CaseSensitive(false)
			                    .AddOption(Option.Builder<object>("foo"));
			var b1 = CommandLine.Builder<object>("test.exe")
							   .CaseSensitive(true)
			                    .AddOption(Option.Builder<object>("foo"));
			var b2 = CommandLine.Builder<object>("test.exe")
							   .CaseSensitive()
			                    .AddOption(Option.Builder<object>("foo"));

			// Act
			ICommandLine<object> cl0 = b0.Build();
			ICommandLine<object> cl1 = b1.Build();
			ICommandLine<object> cl2 = b2.Build();

			// Assert
			Assert.False(cl0.CaseSensitive);
			Assert.True(cl1.CaseSensitive);
			Assert.True(cl2.CaseSensitive);
		}
	}
}
