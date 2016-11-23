# Unclazz.Commons.CLI

`Unclazz.Commons.CLI`は.NETランタイム上で実行するコマンドライン・アプリケーションを構築する際、コマンドライン・オプションの定義や定義に基づくパースを行うためのライブラリです。開発開始の当初念頭にあったのは`Apache Commons CLI`ですが結果的にはまったくの別物となっています。アセンブリは[NuGet Gallery](https://www.nuget.org/packages/Unclazz.Commons.CLI/)で公開されているので、NuGetを通じて取得することができます。

## コンポーネント

このライブラリの提供するAPIはいずれも`Unclazz.Commons.CLI`名前空間で提供されています。

`ICommandLine`と`IOption`はそれぞれコマンドラインとそのオプションの定義を表わすものです。このインターフェースが提供するAPIを通じて定義済みのコマンドラインの情報にアクセスできることはもちろん、アプリケーションのユーザがシェルやコマンドプロンプトで指定した引数のパースを行うことができます。

`CommandLine`と`Option`は前述のインターフェースを実装した具象クラスです。`CommandLine.Builder(string)`と`Option.Builder(string)`を通じて得られるビルダーを使うことでそのインスタンスを構築することができます。

`HelpFormatter`は`ICommandLine`のインスタンスをもとにヘルプ表示のフォーマットを行うクラスです。

## サンプルコード

`Unclazz.Commons.CLI.Sample`プロジェクトにサンプル・コードを格納しています。これは[Tac.MetaServlet.Client](https://github.com/unclazz/Tac.MetaServlet.Client)のサンプル・アプリケーションのCLIを模倣したものです：

```cs
var ps = new Parameters();
var b = CommandLine.Builder("TACRPC.EXE");

// コマンドライン全体に関わる設定
b.Description("TAC(Talend Administration Center)のRPCインターフェースである" +
			   "MetaServletに対してコマンドラインからアクセスする手段を提供します.")
 .CaseSensitive(false);

// Jオプション
b.AddOption(Option.Builder("/J")
	.AlternativeName("/JSON")
	.ArgumentName("json-file")
	.Description("RPCリクエストを表わすJSONが記述されたファイルのパス.")
	.HasArgument()
	.Required()
	.SetterDelegate((string s) => ps.RequestJson = s));

// H, P, Qオプション
b.AddOption(Option.Builder("/H")
	.AlternativeName("/HOST")
	.ArgumentName("host")
	.Description("RPCリクエスト先のホスト名. デフォルトは\"localhost\".")
	.HasArgument()
	.SetterDelegate((string s) => ps.RemoteHost = s))
.AddOption(Option.Builder("/P")
	.AlternativeName("/PORT")
	.ArgumentName("port")
	.Description("RPCリクエスト先のポート名. デフォルトは8080.")
	.HasArgument()
	.SetterDelegate((int i) => ps.RemotePort = i))
.AddOption(Option.Builder("/Q")
	.AlternativeName("/PATH")
	.ArgumentName("path")
	.Description("RPCリクエスト先のパス名. デフォルトは" +
	"\"/org.talend.administrator/metaServlet\".")
	.HasArgument()
	.SetterDelegate((string s) => ps.RemotePath = s));

// Tオプション
b.AddOption(Option.Builder("/T")
	.AlternativeName("/TIMEOUT")
	.ArgumentName("timeout")
	.Description("RPCリクエストのタイムアウト時間. 単位はミリ秒. デフォルトは100000.")
	.HasArgument()
	.SetterDelegate((int i) => ps.RemotePort = i));

// Dオプション（フラグ）
b.AddOption(Option.Builder("/D")
	.AlternativeName("/DUMP")
	.Description("リクエストとレスポンスのダンプ出力を行う.")
	.SetterDelegate(() => ps.ShowDump = true));

// コマンドライン定義を構築
var cmdln = b.Build();

// コマンドラインをパース
cmdln.Parse(new string[] { "/j", "path/to/json", "/d", "/p", "8888" });

Console.WriteLine("ps.RequestJson = {0}", ps.RequestJson);
Console.WriteLine("ps.RemotePort  = {0}", ps.RemotePort);
Console.WriteLine("ps.ShowDump    = {0}", ps.ShowDump);
```

コードの末尾でコマンドラインのパースを行っています。標準出力には以下のような出力がなされるでしょう：

```sh
ps.RequestJson = path/to/json
ps.RemotePort  = 8888
ps.ShowDump    = True
```

最後にヘルプ表示を行ってみましょう：

```cs
// デフォルトのオプションでヘルプ表示
Console.WriteLine(new HelpFormatter().Format(cmdln));
```

このコードを実行すると以下のような出力がなされます：

```sh
Syntax:
                    TACRPC.EXE /J <json-file> [/D] [/H <host>] [/P <port>] [/Q <
                    path>] [/T <timeout>]

Description:
                    TAC(Talend Administration Center)のRPCインターフェースであるMetaServletに
                    対してコマンドラインからアクセスする手段を提供します.

Options:
/D, /DUMP           リクエストとレスポンスのダンプ出力を行う.
/H, /HOST           RPCリクエスト先のホスト名. デフォルトは"localhost".
/J, /JSON           RPCリクエストを表わすJSONが記述されたファイルのパス.
/P, /PORT           RPCリクエスト先のポート名. デフォルトは8080.
/Q, /PATH           RPCリクエスト先のパス名. デフォルトは"/org.talend.administrator/metaServlet"
                    .
/T, /TIMEOUT        RPCリクエストのタイムアウト時間. 単位はミリ秒. デフォルトは100000.
```
