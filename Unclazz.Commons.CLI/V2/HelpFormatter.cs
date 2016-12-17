using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドラインの定義に基づきヘルプ表示を整形するクラスです。
	/// </summary>
	public class HelpFormatter<T>
	{
		/// <summary>
		/// ヘルプ表示の整形のためのオプションを格納するクラスです。
		/// </summary>
		public class FormatOptions
		{
			/// <summary>
			/// ヘルプ表示の1行の列数（文字数）です。
			/// デフォルトは<c>80</c>です。
			/// </summary>
			/// <value>1行の列数</value>
			public int LineWidth { get; set; }
			/// <summary>
			/// コマンドライン・オプションをインデントする列数（文字数）です。
			/// デフォルトは<c>20</c>です。
			/// </summary>
			/// <value>インデントの列数</value>
			public int IndentWidth { get; set; }
			/// <summary>
			/// コンストラクタです。
			/// </summary>
			public FormatOptions()
			{
				LineWidth = 80;
				IndentWidth = 20;
			}
		}

		private readonly FormatOptions options = new FormatOptions();

		/// <summary>
		/// コンストラクタです。
		/// 整形オプションはデフォルトのものを使用します。
		/// </summary>
		public HelpFormatter()
		{
		}
		/// <summary>
		/// コンストラクタです。
		/// 引数で整形オプションを設定できます。
		/// </summary>
		/// <param name="options">整形オプション</param>
		public HelpFormatter(FormatOptions options)
		{
			this.options = options;
		}
		/// <summary>
		/// ヘルプ表示を整形して返します。
		/// </summary>
		/// <param name="cmdln">コマンドラインの定義情報</param>
		public string Format(ICommandLine<T> cmdln)
		{
			return Render_Help(cmdln).ToString();
		}

		StringBuilder Render_Help(ICommandLine<T> cmdln)
		{
			var b = new StringBuilder();
			b.Append(Render_Section("Syntax", Render_Syntax(cmdln).ToString()))
			 .Append(Render_Section("Description", cmdln.Description))
			 .AppendLine("Options:");

			foreach (var o in cmdln.Options)
			{
				b.Append(Render_Option(o));
			}

			return b;
		}

		IEnumerable<StringBuilder> Split(IEnumerable<char> chars)
		{
			var w = options.LineWidth - options.IndentWidth;
			var l = new List<StringBuilder>();
			var q = new Queue<char>(chars);
			var b = new StringBuilder();

			l.Add(b);
			while (q.Count > 0)
			{
				b.Append(q.Dequeue());
				if (b.Length >= w)
				{
					b = new StringBuilder();
					l.Add(b);
				}
			}

			return l;
		}

		StringBuilder Render_Syntax(ICommandLine<T> cmdln)
		{
			var b = new StringBuilder();

			b.Append(cmdln.CommandName);

			foreach (var opt in cmdln.Options.Where(o => o.Required))
			{
				b.Append(' ').Append(opt.Name);
				if (opt.HasArgument)
				{
					b.Append(' ').Append('<').Append(opt.ArgumentName).Append('>');
				}
			}

			foreach (var opt in cmdln.Options.Where(o => !o.Required))
			{
				b.Append(' ').Append('[').Append(opt.Name);
				if (opt.HasArgument)
				{
					b.Append(' ').Append('<').Append(opt.ArgumentName).Append('>');
				}
				b.Append(']');
			}

			foreach (var arg in cmdln.ArgumentNames)
			{
				b.Append(' ').Append('<').Append(arg).Append('>');
			}

			return b;
		}

		StringBuilder Spaces(int n)
		{
			return Enumerable
				.Repeat(' ', n)
				.Aggregate(new StringBuilder(), (b, ch) => b.Append(ch));
		}

		StringBuilder Render_OptionNameAndArgName(IOption<T> opt)
		{
			var w = options.IndentWidth;
			var b = new StringBuilder();

			b.Append(opt.Name);
			if (opt.AlternativeName.Length > 0)
			{
				b.Append(", ").Append(opt.AlternativeName);
			}

			if (b.Length > w)
			{
				b.AppendLine().Append(Spaces(w));
			}
			else {
				b.Append(Spaces(w - b.Length));
			}

			return b;
		}

		StringBuilder Render_Option(IOption<T> opt)
		{
			var sp = Spaces(options.IndentWidth);
			var b = new StringBuilder().Append(Render_OptionNameAndArgName(opt));

			if (opt.Description.Length == 0)
			{
				return b.AppendLine();
			}

			var first = true;
			foreach (var ln in Split(opt.Description))
			{
				if (first)
				{
					first = false;
				}
				else {
					b.Append(sp);
				}
				b.Append(ln).AppendLine();
			}

			return b;
		}

		StringBuilder Render_Section(string title, IEnumerable<char> content)
		{
			var sp = Spaces(options.IndentWidth);
			var b = new StringBuilder().Append(title).Append(':').AppendLine();
			foreach (var ln in Split(content))
			{
				b.Append(sp).Append(ln).AppendLine();
			}
			return b.AppendLine();
		}
	}
}
