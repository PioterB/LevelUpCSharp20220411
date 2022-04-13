using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LevelUpCSharp.Helpers;
using LevelUpCSharp.Production.Ingredients;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production
{
    public class Vendor
    {
	    private static readonly Random Rand;

        private readonly ConcurrentQueue<Sandwich> _warehouse = new ConcurrentQueue<Sandwich>();
        private readonly ConcurrentQueue<PendingOrder> _orders = new ConcurrentQueue<PendingOrder>();
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
	        howMuch = howMuch == 0 ? _warehouse.Count : howMuch;
	        int index = 0;
	        var toSell = new List<Sandwich>();
	        while (_warehouse.TryDequeue(out var sandwich) && howMuch > index)
	        {
		        toSell.Add(sandwich);
		        index++;
	        }

	        return toSell;
        }

        public void Order(SandwichKind kind, int count)
        {
	        _orders.Enqueue(new PendingOrder(count, kind));
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

            var stock = _warehouse.ToArray();
            var result = stock
	            .GroupBy(
					sandwich => sandwich.Kind, 
					(kind, sandwiches) => new StockItem(kind, sandwiches.Count()))
	            .ToArray();
            return result;
        }

        private IEnumerable<Sandwich> Produce(SandwichKind kind, int amount = 1)
        {
	        while (amount -- > 0)
	        {
		        yield return  kind switch
		        {
			        SandwichKind.Beef => ProduceSandwich(new Beef(), DateTimeOffset.Now.AddMinutes(3)),
			        SandwichKind.Cheese => ProduceSandwich(new Cheese(), DateTimeOffset.Now.AddSeconds(90)),
			        SandwichKind.Chicken => ProduceSandwich(new Chicken(), DateTimeOffset.Now.AddMinutes(4)),
			        SandwichKind.Pork => ProduceSandwich(new Pork(), DateTimeOffset.Now.AddSeconds(150)),
			        SandwichKind.Deer => ProduceSandwich(new Deer(), DateTimeOffset.Now.AddSeconds(150)),
			        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
		        };
	        }
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

		        _orders.Enqueue(new PendingOrder(1, (SandwichKind)kind));

		        var productionTasks = new List<Task<IEnumerable<Sandwich>>>();
                while ( _orders.TryDequeue(out var pendingOrder))
		        {
			        var orderItemProductionTask = Task.Factory.StartNew(() => Produce(pendingOrder.Kind, pendingOrder.Amount));
			        productionTasks.Add(orderItemProductionTask);
			        Thread.Sleep(1 * 500);
		        }

                Task.WaitAll(productionTasks.ToArray());
                var sandwiches = productionTasks.SelectMany(tasks => tasks.Result).ToList();
                sandwiches.ForEach(sandwich => _warehouse.Enqueue(sandwich));
                Produced?.Invoke(sandwiches.ToArray());

                productionTasks.Clear();

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
