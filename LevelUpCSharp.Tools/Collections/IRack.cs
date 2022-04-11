using System.Collections.Generic;

namespace LevelUpCSharp.Collections
{
	public interface IRack<TKey, TItem> : IEnumerable<TItem> where TItem : IKindable<TKey>
	{
		/// <summary>
		/// Gets actual number of <see cref="TItem"/> being on the Rack
		/// </summary>
		int Amount { get; }

		TItem this[TKey key] { get; }

		void Add(TItem sandwich);

		TItem Get(TKey kind);

		bool Contains(TKey kind);
	}
}