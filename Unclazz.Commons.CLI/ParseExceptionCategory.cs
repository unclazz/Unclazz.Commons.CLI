using System;
namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// パース例外のカテゴリです。
	/// </summary>
	public enum ParseExceptionCategory
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
}
