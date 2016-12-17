using System;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドライン・パース中に発生したエラーを表わす例外クラスです。
	/// </summary>
	public class ParseException : Exception
	{
		private static readonly string message = 
			"An error has occurred while parsing command line.";

		/// <summary>
		/// この例外オブジェクトが表わす問題のカテゴリです。
		/// </summary>
		/// <value>カテゴリ</value>
		public ParseExceptionCategory Category { get; }
		/// <summary>
		/// エラー発生時に処理中だった<see cref="IOption"/>です。
		/// </summary>
		/// <value>オプション</value>
		public IOption TargetOption { get; }
		/// <summary>
		/// エラー発生時に処理中だったコマンドライン引数の値です。
		/// </summary>
		/// <value>引数の値</value>
		public string TargetValue { get; }

		internal ParseException
		(ParseExceptionCategory c, IOption o, string v) : base(message)
		{
			Category = c;
			TargetOption = o;
			TargetValue = v;
		}

		internal ParseException
		(ParseExceptionCategory c, IOption o, string v, Exception cause) : base(message, cause)
		{
			Category = c;
			TargetOption = o;
			TargetValue = v;
		}
	}
}
