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
		/// 例外のカテゴリです。
		/// </summary>
		public enum ExceptionCategory
		{
			/// <summary>
			/// コマンドラインのパース中に予期せぬエラーが発生しました。
			/// </summary>
			UnexpectedErrorHasOccurred = 0,
			/// <summary>
			/// <see cref="IOption.SetterDelegate"/>が参照するデリゲート内でエラーが発生しました。
			/// </summary>
			SetterErrorHasOccurred = 1,
			/// <summary>
			/// 必須のオプションが設定されていません。
			/// </summary>
			RequiredOptionNotFound = 2,
			/// <summary>
			/// 単一値をとるべきオプションで重複が見つかりました。
			/// </summary>
			DuplicatedOption = 3
		}

		/// <summary>
		/// この例外オブジェクトが表わす問題のカテゴリです。
		/// </summary>
		/// <value>カテゴリ</value>
		public ExceptionCategory Category { get; }
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
		(ExceptionCategory c, IOption o, string v) : base(message)
		{
			Category = c;
			TargetOption = o;
			TargetValue = v;
		}

		internal ParseException
		(ExceptionCategory c, IOption o, string v, Exception cause) : base(message, cause)
		{
			Category = c;
			TargetOption = o;
			TargetValue = v;
		}
	}
}
