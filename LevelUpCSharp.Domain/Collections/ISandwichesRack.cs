using System.Collections.Generic;
using System.Threading.Tasks;

namespace LevelUpCSharp.Collections
{
	internal interface ISandwichesRack<TKey, TItem> : IEnumerable<TItem> where TItem : IKindable<TKey>
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