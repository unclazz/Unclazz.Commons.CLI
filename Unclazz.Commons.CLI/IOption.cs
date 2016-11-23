using System;
namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドライン・オプションの定義情報を表わすインターフェースです。
	/// </summary>
	public interface IOption
	{
		/// <summary>
		/// オプション名です。
		/// </summary>
		/// <value>オプション名</value>
		string Name { get; }
		/// <summary>
		/// オプションの別名です。
		/// </summary>
		/// <value>オプションの別名</value>
		string AlternativeName { get; }
		/// <summary>
		/// アプリケーション構成ファイルの設定情報におけるキーです。
		/// この値が設定されているオプションは、
		/// コマンドライン引数として値が設定されていない場合、
		/// アプリケーション構成ファイルの設定情報をフォールバックとして利用します。
		/// </summary>
		/// <value>設定情報のキー</value>
		string SettingName { get; }
		/// <summary>
		/// このオプションが必須のものかどうかを示す値を返します。
		/// </summary>
		/// <value><c>true</c>の場合は必須</value>
		bool Required { get; }
		/// <summary>
		/// このオプションが引数をとるかどうかを示す値を返します。
		/// </summary>
		/// <value><c>true</c>の場合は引数をとる</value>
		bool HasArgument { get; }
		/// <summary>
		/// このオプションの引数名を返します。
		/// 引数名はヘルプの出力に使用されます。
		/// </summary>
		/// <value>オプションの引数名</value>
		string ArgumentName { get; }
		/// <summary>
		/// このオプションが複数回指定できるかどうかを示す値を返します。
		/// </summary>
		/// <value><c>true</c>の場合は複数回指定ができる</value>
		bool Multiple { get; }
		/// <summary>
		/// このオプションの説明です。
		/// 説明はヘルプの出力に使用されます。
		/// </summary>
		/// <value>オプションの説明</value>
		string Description { get; }
		/// <summary>
		/// オプションの引数を任意のオブジェクトに設定するためのデリゲートです。
		/// デフォルト値として利用されているデリゲートは受け取った値を単に破棄します。
		/// </summary>
		/// <value>デリゲート</value>
		Action<string> SetterDelegate { get; }
	}
}
