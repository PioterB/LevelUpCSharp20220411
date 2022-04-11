using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LevelUpCSharp.Collections
{
	public class Rack<TKey, TItem> : IRack<TKey, TItem> where TItem : IKindable<TKey>
	{
		private readonly IDictionary<TKey, Queue<TItem>> _lines;
		private int _amount;
		
		public Rack()
		{
			_lines = new Dictionary<TKey, Queue<TItem>>();	
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return Aggregate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Amount => _amount;

		public TItem this[TKey key] => Dequeue(key);

		public void Add(TItem sandwich)
		{
			_amount++;
			_lines[sandwich.Kind].Enqueue(sandwich);
		}

		public TItem Get(TKey kind)
		{
			return Dequeue(kind);
		}

		public bool Contains(TKey kind)
		{
			return _lines.ContainsKey(kind) && _lines[kind].Any();
		}

		private TItem Dequeue(TKey kind)
		{
			_amount--;
			return _lines[kind].Dequeue();
		}

		private IEnumerable<TItem> Aggregate()
		{
			var result = new List<TItem>();
			foreach (var sandwiches in _lines.Values)
			{
				result.AddRange(sandwiches);
			}

			result.Sort();

			return result;
		}
	}
}