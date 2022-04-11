using System;
using System.Collections.Generic;

namespace LevelUpCSharp.Collections
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
			{
				action(item);
			}
		}
	}
}