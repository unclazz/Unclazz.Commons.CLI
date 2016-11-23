using System;
using System.Collections.Generic;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドラインの定義情報を表わすインターフェースです。
	/// </summary>
	public interface ICommandLine
	{
		/// <summary>
		/// コマンド名です。
		/// </summary>
		/// <value>コマンド名</value>
		string CommandName { get; }
		/// <summary>
		/// コマンドの説明です。
		/// </summary>
		/// <value>コマンドの説明</value>
		string Description { get; }
		/// <summary>
		/// コマンドライン引数のパースのとき大文字小文字を区別する場合<c>true</c>を返します。
		/// </summary>
		/// <value>大文字小文字を区別する場合<c>true</c></value>
		bool CaseSensitive { get; }
		/// <summary>
		/// コマンドライン・オプションの定義情報です。
		/// </summary>
		/// <value>コマンドライン・オプションの定義情報</value>
		OptionCollection Options { get; }
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」を処理するためのデリゲートです。
		/// デフォルト値として利用されているデリゲートは受け取った値を単に破棄します。
		/// </summary>
		/// <value>「残りの引数」のためのデリゲート</value>
		Action<IEnumerable<string>> SetterDelegate { get; }
		/// <summary>
		/// コマンドライン引数をパースします。
		/// アプリケーション構成ファイルの設定情報がフォールバックとして利用されます。
		/// </summary>
		/// <param name="arguments">コマンドライン引数</param>
		/// <exception cref="ParseException">パース中にエラーが発生した場合</exception>
		void Parse(IEnumerable<string> arguments);
		/// <summary>
		/// コマンドライン引数をパースします。
		/// 第2引数で指定された設定情報がフォールバックとして利用されます。
		/// </summary>
		/// <param name="arguments">コマンドライン引数</param>
		/// <param name="settings">設定情報を格納した辞書</param>
		/// <exception cref="ParseException">パース中にエラーが発生した場合</exception>
		void Parse(IEnumerable<string> arguments, IDictionary<string, string> settings);
	}
}
