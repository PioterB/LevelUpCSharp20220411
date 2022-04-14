using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LevelUpCSharp.Collections;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Retail
{
    public class Retailer
    {
        private static Retailer _instance;
        private readonly IRack<SandwichKind, Sandwich> _lines;

        protected Retailer(string name)
        {
            Name = name;
            _lines = new SandwichesRack();
        }

        public static Retailer Instance => _instance ?? (_instance = new Retailer("Build-in"));

        public event Action<PackingSummary> Packed;
        public event Action<DateTimeOffset, Sandwich> Purchase;

        public string Name { get; }

        public Result<Sandwich> Sell(SandwichKind kind)
        {
	        return SellImpl(kind);
        }

        public Task<Result<Sandwich>> SellAsync(SandwichKind kind)
        {
	        return Task.Run(() => SellImpl(kind));
        }

        public void Pack(IEnumerable<Sandwich> package, string deliver)
        {
            package = package.ToArray();
            PopulateRack(package);
            var summary = ComputeReport(package, deliver);
            OnPacked(summary);
        }

        public Task PackAsync(IEnumerable<Sandwich> package, string deliver)
        {
	        return Task.Run(() =>
	        { 
		        package = package.ToArray();
		        PopulateRack(package);
		        var summary = ComputeReport(package, deliver);
		        OnPacked(summary);
	        });
        }

        protected virtual void OnPacked(PackingSummary summary)
        {
            Packed?.Invoke(summary);
        }

        protected virtual void OnPurchase(DateTimeOffset time, Sandwich product)
        {
            Purchase?.Invoke(time, product);
        }
        
        private static PackingSummary ComputeReport(IEnumerable<Sandwich> package, string deliver)
        {
	        var summaryPositions = package
		        .GroupBy(
			        p => p.Kind,
			        (kind, sandwiches) => new LineSummary(kind, sandwiches.Count()))
		        .ToArray();

	        var summary = new PackingSummary(summaryPositions, deliver);
	        return summary;
        }

        private void PopulateRack(IEnumerable<Sandwich> package)
        {
			package.ForEach(p =>
			{
				lock (_lines)
				{
					_lines.Add(p);
				}
			});
        }

        private Result<Sandwich> SellImpl(SandwichKind kind)
        {
	        Sandwich sandwich;
	        lock (_lines)
	        {
		        if (!_lines.Contains(kind))
		        {
			        return Result<Sandwich>.Failed();
		        }

		        sandwich = _lines[kind];
	        }

	        OnPurchase(DateTimeOffset.Now, sandwich);

	        return sandwich.AsSuccess();
        }
    }
}
