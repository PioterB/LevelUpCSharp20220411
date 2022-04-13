using System;
using System.Collections.Generic;
using System.Threading;
using LevelUpCSharp.Helpers;
using LevelUpCSharp.Production.Ingredients;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production
{
    public class Vendor
    {
	    private static readonly Random Rand;

        private readonly List<Sandwich> _warehouse = new List<Sandwich>();
        private readonly List<PendingOrder> _orders = new List<PendingOrder>();
		private readonly Thread _production;

        static Vendor()
        {
	        Rand = new Random((int)DateTime.Now.Ticks);
        }

        public Vendor(string name)
        {
	        Name = name;
            _production = new Thread(DoProduction) { IsBackground = true };
            _production.Start();
        }

        public event Action<Sandwich[]> Produced;

        public string Name { get; }

        public IEnumerable<Sandwich> Buy(int howMuch = 0)
        {
			lock (_warehouse)
			{
				if (_warehouse.Count == 0)
				{
					return Array.Empty<Sandwich>();
				}

				if (howMuch == 0 || _warehouse.Count <= howMuch)
				{
					var result = _warehouse.ToArray();
					_warehouse.Clear();
					return result;
				}

				var toSell = new List<Sandwich>();
				for (int i = 0; i < howMuch; i++)
				{
					var first = _warehouse[0];
					toSell.Add(first);
					_warehouse.Remove(first);
				}

				return toSell; 
			}
        }

        public void Order(SandwichKind kind, int count)
        {
	        lock (_orders)
	        {
		        _orders.Add(new PendingOrder(count, kind));
	        }
		}

        public IEnumerable<StockItem> GetStock()
        {
            Dictionary<SandwichKind, int> counts = new Dictionary<SandwichKind, int>()
            {
                {SandwichKind.Cheese, 0},
                {SandwichKind.Chicken, 0},
                {SandwichKind.Beef, 0},
                {SandwichKind.Pork, 0},
                {SandwichKind.Deer, 0},
            };

            foreach (var sandwich in _warehouse)
            {
                counts[sandwich.Kind] += 1;
            }

            var result = new StockItem[counts.Count];

            int i = 0;
            foreach (var count in counts)
            {
                result[i] = new StockItem(count.Key, count.Value);
                i++;
            }

            return result;
        }

        private Sandwich Produce(SandwichKind kind)
        {
            return kind switch
            {
                SandwichKind.Beef => ProduceSandwich(new Beef(), DateTimeOffset.Now.AddMinutes(3)),
                SandwichKind.Cheese => ProduceSandwich(new Cheese(), DateTimeOffset.Now.AddSeconds(90)),
                SandwichKind.Chicken => ProduceSandwich(new Chicken(), DateTimeOffset.Now.AddMinutes(4)),
                SandwichKind.Pork => ProduceSandwich(new Pork(), DateTimeOffset.Now.AddSeconds(150)),
                SandwichKind.Deer => ProduceSandwich(new Deer(), DateTimeOffset.Now.AddSeconds(150)),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }

        private Sandwich ProduceSandwich(IKeyIngredient kind, DateTimeOffset addMinutes)
        {
	        var sandwich = SandwichBuilder.WithButter(false)
								.Use(kind)
								.AddGarnish(new Tommato())
								.AddGarnish(new Cheese())
								.AddGarnish(new Onion())
								.AddTopping(new GarlicSos())
								.Wrap();

	        return sandwich;
        }

        private void DoProduction(object obj)
        {
			SandwichKind[] sandwichKinds = EnumHelper.GetValues<SandwichKind>();

            while (true)
	        {
		        var kind = Rand.Next(sandwichKinds.Length);

                var sandwich = Produce((SandwichKind)kind);

		        lock (_warehouse)
		        {
			        _warehouse.Add(sandwich);
		        }

		        Produced?.Invoke(new[] { sandwich });
                
		        IEnumerable<PendingOrder> sideWork;
		        lock (_orders)
		        {
			        sideWork = _orders.ToArray();
			        _orders.Clear();
		        }

		        foreach (var pendingOrder in sideWork)
		        {
			        var sandwiches = new List<Sandwich>();

			        for (int i = 0; i < pendingOrder.Amount; i++)
			        {
				        sandwich = Produce(pendingOrder.Kind);

				        lock (_warehouse)
				        {
					        _warehouse.Add(sandwich);
				        }

				        sandwiches.Add(sandwich);
				        Thread.Sleep(1 * 500);
			        }

			        Produced?.Invoke(sandwiches.ToArray());

			        Thread.Sleep(1 * 500);
		        }
                
		        Thread.Sleep(5 * 1000);
	        }
        }

        private class PendingOrder
        {
	        public PendingOrder(int amount, SandwichKind kind)
	        {
		        Amount = amount;
		        Kind = kind;
	        }

	        public int Amount { get; }

	        public SandwichKind Kind { get; }
        }
    }
}
