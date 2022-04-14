using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using LevelUpCSharp.Collections;
using LevelUpCSharp.Products;
using Newtonsoft.Json;

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
            if (!_lines.Contains(kind))
            {
                return Result<Sandwich>.Failed();
            }

            var sandwich = _lines[kind];
            OnPurchase(DateTimeOffset.Now, sandwich);

            return sandwich.AsSuccess();
        }

        public void Pickup()
        {
	        try
	        {
		        IEnumerable<Sandwich> sandwiches;
		        using (var connection = BuildConnection())
		        {
			        using (var stream = connection.GetStream())
			        {
				        SendCommand(stream);
				        sandwiches = ReadResponse(stream);
			        }
		        }

		        Pack(sandwiches, "remote");
	        }
	        catch (SocketException)
	        {
	        }
        }

        protected virtual void OnPacked(PackingSummary summary)
        {
            Packed?.Invoke(summary);
        }

        protected virtual void OnPurchase(DateTimeOffset time, Sandwich product)
        {
            Purchase?.Invoke(time, product);
        }

        private void Pack(IEnumerable<Sandwich> package, string deliver)
        {
	        package = package.ToArray();
	        PopulateRack(package);
	        var summary = ComputeReport(package, deliver);
	        OnPacked(summary);
        }

        private void PopulateRack(IEnumerable<Sandwich> package)
        {
	        package.ForEach(p => _lines.Add(p));
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

        #region networking
        private TcpClient BuildConnection()
        {
	        TcpClient client = new TcpClient();
	        client.Connect("localhost", 13000);
	        return client;
        }

        private IEnumerable<Sandwich> ReadResponse(NetworkStream stream)
        {
	        using (var sr = new StreamReader(stream))
	        {
		        using (var jsonReader = new JsonTextReader(sr))
		        {
			        return new JsonSerializer().Deserialize<IEnumerable<Sandwich>>(jsonReader);
		        }
	        }
        }

        private void SendCommand(NetworkStream stream)
        {
	        var data = System.Text.Encoding.ASCII.GetBytes("p+b");
	        stream.Write(data, 0, data.Length);
        }
        #endregion
    }
}
