using System;
using System.Collections.Generic;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドラインの定義情報を表わすインターフェースです。
	/// </summary>
	public interface ICommandLine<T>
	{
		/// <summary>
		/// コマンド名です。
		/// </summary>
		/// <value>コマンド名</value>
		string CommandName { get; }
		/// <summary>
		/// コマンドの説明です。
		/// 説明はヘルプの出力に使用されます。
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
		OptionCollection<T> Options { get; }
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」を処理するためのデリゲートです。
		/// デフォルト値として利用されているデリゲートは受け取った値を単に破棄します。
		/// </summary>
		/// <value>「残りの引数」のためのデリゲート</value>
		Action<T, IEnumerable<string>> SetterDelegate { get; }
		/// <summary>
		/// コマンドライン引数のうちコマンドライン・オプションの定義情報に
		/// 含まれなかった「残りの引数」の名前のシーケンス。
		/// これらの名前はヘルプの出力に使用されます。
		/// </summary>
		/// <value>「残りの引数」の名前のシーケンス</value>
		IEnumerable<string> ArgumentNames { get; }
		/// <summary>
		/// パーサ・インスタンスを取得します。
		/// </summary>
		/// <returns>パーサ</returns>
		/// <param name="valueObject">パースした結果の値が設定されるバリュー・オブジェクト</param>
		IParser<T> GetParser(T valueObject);
		IParser<T> GetParser(Func<T> valueObjectSupplier);
		IParser<T> GetParser();
	}
}
