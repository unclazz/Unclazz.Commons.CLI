using System;
using System.Collections.Generic;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドライン文字列のパーサを表わすインターフェースです。
	/// </summary>
	public interface IParser<T>
	{
		/// <summary>
		/// コマンドライン引数をパースします。
		/// 第2引数で指定された設定情報がフォールバックとして利用されます。
		/// </summary>
		/// <param name="arguments">コマンドライン引数</param>
		/// <param name="settings">設定情報を格納した辞書</param>
		/// <exception cref="ParseException">パース中にエラーが発生した場合</exception>
		T Parse(IEnumerable<string> arguments, IDictionary<string, string> settings);
		/// <summary>
		/// コマンドライン引数をパースします。
		/// アプリケーション構成ファイルの設定情報がフォールバックとして利用されます。
		/// </summary>
		/// <param name="arguments">コマンドライン引数</param>
		/// <exception cref="ParseException">パース中にエラーが発生した場合</exception>
		T Parse(IEnumerable<string> arguments);
	}
}
