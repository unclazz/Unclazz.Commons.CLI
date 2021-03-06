﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Commons.CLI
{
	/// <summary>
	/// コマンドライン・オプションの定義のコレクションです。
	/// オプションは<see cref="IOption.Name"/>の自然順序でソートされます。
	/// </summary>
	public class OptionCollection<T> : IEnumerable<IOption<T>>
	{
		private readonly IList<IOption<T>> list;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="options">オプションのシーケンス</param>
		public OptionCollection(IEnumerable<IOption<T>> options)
		{
			list = options.OrderBy(o => o.Name)
			              .ToList().AsReadOnly();
		}
        /// <summary>
        /// 列挙子を返します。
        /// </summary>
        /// <returns>列挙子</returns>
		public IEnumerator<IOption<T>> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		/// <summary>
		/// コレクションに含まれるオプションの定義の数です。
		/// </summary>
		/// <value>オプションの定義の数</value>
		public int Count { get { return list.Count; } }
		/// <summary>
		/// コレクションに含まれるオプションの定義にインデックスによりアクセスします。
		/// </summary>
		/// <param name="i">インデックス</param>
		public IOption<T> this[int i]
		{
			get {
				return list[i];
			}
		}
		/// <summary>
		/// コレクションに含まれるオプションの定義にオプションの名前によりアクセスします。
		/// </summary>
		/// <param name="name">オプションの名前</param>
		public IOption<T> this[string name]
		{
			get
			{
				return list.First(o => o.Name.Equals(name));
			}
		}
	}
}
