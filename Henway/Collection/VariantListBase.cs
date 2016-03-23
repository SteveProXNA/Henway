using System;
using System.Collections;

namespace Henway.Collection
{
	/// <summary>
	/// http://www.codeproject.com/Articles/36463/A-non-generic-IList-implementation-without-any-Boxing-and-Unboxing.aspx
	/// </summary>
	public abstract class VariantListBase : IList
	{
		private readonly IList innerList = new ArrayList();

		public Int32 Add(object value) { return innerList.Add(value); }
		public void Clear() { innerList.Clear(); }
		public Boolean Contains(object value) { return innerList.Contains(value); }
		public Int32 IndexOf(object value) { return innerList.IndexOf(value); }
		public void Insert(Int32 index, object value) { innerList.Insert(index, value); }
		public Boolean IsFixedSize { get { return innerList.IsFixedSize; } }
		public Boolean IsReadOnly { get { return innerList.IsReadOnly; } }
		public void Remove(object value) { innerList.Remove(value); }
		public void RemoveAt(Int32 index) { innerList.RemoveAt(index); }
		public Object this[Int32 index] 
		{
			get { return innerList[index]; }
			set { innerList[index] = value; }
		}

		public void CopyTo(Array array, Int32 index) { innerList.CopyTo(array, index); }
		public Int32 Count { get { return innerList.Count; } }
		public Boolean IsSynchronized { get { return innerList.IsSynchronized; } }
		public object SyncRoot { get { return innerList.SyncRoot; } }

		public IEnumerator GetEnumerator() { return (innerList as IEnumerable).GetEnumerator(); }
	}
}