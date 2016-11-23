using System;

namespace Unclazz.Commons.CLI.Sample
{
	class MainClass
	{
		public static void Main(string[] args)
		{
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

			// ヘルプを表示
			Console.WriteLine(new HelpFormatter().Format(cmdln));

			// コマンドラインをパース
			cmdln.Parse(new string[] { "/j", "path/to/json", "/d", "/p", "8888" });

			Console.WriteLine("ps.RequestJson = {0}", ps.RequestJson);
			Console.WriteLine("ps.RemotePort  = {0}", ps.RemotePort);
			Console.WriteLine("ps.ShowDump    = {0}", ps.ShowDump);
		}
	}

	class Parameters
	{
		public string RemoteHost { get; set; }
		public string RemotePath { get; set; }
		public int RemotePort { get; set; }
		public int RequestTimeout { get; set; }
		public string RequestJson { get; set; }
		public bool ShowHelp { get; set; }
		public bool ShowDump { get; set; }
	}
}
