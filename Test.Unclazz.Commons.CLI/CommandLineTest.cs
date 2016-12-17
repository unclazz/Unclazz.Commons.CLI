using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unclazz.Commons.CLI;

namespace Test.Unclazz.Commons.CLI
{
	[TestFixture()]
	public class CommandLineTest
	{
		[Test()]
		public void GetParser_WhenCalledWithNull_ThrowsException()
		{
			// Arrange
			var cl0 = CommandLine.Builder<object>("foo.exe")
			                     .AddOption(Option.Builder<object>("/f"))
								 .Build();

			// Act
			// Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				cl0.GetParser(null);
			});
		}
	}
}
