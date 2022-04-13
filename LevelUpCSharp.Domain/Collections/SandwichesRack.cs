using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelUpCSharp.Helpers;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Collections
{
	internal class SandwichesRack : IRack<SandwichKind, Sandwich>
	{
		private readonly IDictionary<SandwichKind, Queue<Sandwich>> _lines;
		private readonly Comparison<Sandwich> _comparison;
		private int _amount;

		public SandwichesRack()
		{
			_lines = InitializeLines();
			_comparison = DefaultComparer;
		}

		public SandwichesRack(Comparison<Sandwich> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException(nameof(comparison) + " can not be null");
			}

			_lines = InitializeLines();
			_comparison = comparison;
		}

		public SandwichesRack(IEnumerable<Sandwich> initialSandwiches) 
			: this(initialSandwiches, DefaultComparer)
		{
		}

		public SandwichesRack(IEnumerable<Sandwich> initialSandwiches, Comparison<Sandwich> comparison)
			: this(comparison)
		{
			if (initialSandwiches == null)
			{
				throw new ArgumentNullException(nameof(initialSandwiches) + " can not be null");
			}

			_amount = initialSandwiches.Count();

			PutOnRack(initialSandwiches);
		}

		public int Amount => _amount;

		public Sandwich this[SandwichKind kind] => Dequeue(kind);
		
		public void Add(Sandwich sandwich)
		{
			_lines[sandwich.Kind].Enqueue(sandwich);
			_amount++;
		}

		public bool Contains(SandwichKind kind)
		{
			return _lines.ContainsKey(kind) && _lines[kind].Count > 0;
		}

		public Sandwich Get(SandwichKind kind)
		{
			return Dequeue(kind);
		}

		public IEnumerator<Sandwich> GetEnumerator()
		{
			var sortedByExpirationDate = AggregateSandwiches();
			return new RackEnumerator(sortedByExpirationDate);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static int DefaultComparer(Sandwich x, Sandwich y)
		{
			return x.ExpirationDate.CompareTo(y.ExpirationDate);
		}

		private Sandwich Dequeue(SandwichKind kind)
		{
			_amount--;
			return _lines[kind].Dequeue();
		}

		private IEnumerable<Sandwich> AggregateSandwiches()
		{
			var result = new List<Sandwich>();
			foreach (var sandwiches in _lines.Values)
			{
				result.AddRange(sandwiches);
			}

			result.Sort(_comparison);

			return result;
		}

		private IDictionary<SandwichKind, Queue<Sandwich>> InitializeLines()
		{
			var result = new Dictionary<SandwichKind, Queue<Sandwich>>();

			foreach (var sandwichKind in EnumHelper.GetValues<SandwichKind>())
			{
				result.Add(sandwichKind, new Queue<Sandwich>());
			}

			return result;
		}

		private void PutOnRack(IEnumerable<Sandwich> items)
		{
			items.ForEach(item => _lines[item.Kind].Enqueue(item));
		}

		private class RackEnumerator : IEnumerator<Sandwich>
		{
			private readonly IEnumerator<Sandwich> _enumerator;

			public RackEnumerator(IEnumerable<Sandwich> sortedByExpirationDate)
			{
				_enumerator = sortedByExpirationDate.GetEnumerator();
			}

			public Sandwich Current => _enumerator.Current;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}
		}
	}
}