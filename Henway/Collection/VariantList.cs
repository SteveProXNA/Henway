using System;
using System.Collections;

namespace Henway.Collection
{
	/// <summary>
	/// http://www.codeproject.com/Articles/36463/A-non-generic-IList-implementation-without-any-Boxing-and-Unboxing.aspx
	/// </summary>
	public class VariantList : VariantListBase
	{
		public int Append<T>(T value) where T : struct
		{
			return (this as IList).Add(new T[1] { value });
		}

		public T GetAt<T>(Int32 index) where T : struct
		{
			T[] value = this[index] as T[];
			return value[0];
		}
	}
}
