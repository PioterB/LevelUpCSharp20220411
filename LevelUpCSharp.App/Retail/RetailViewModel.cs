using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using LevelUpCSharp.Collections;
using LevelUpCSharp.Helpers;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Retail
{
    internal class RetailViewModel : ViewModelBase
    {
        private readonly IDictionary<SandwichKind, LineSummaryViewModel> _lines;

        public RetailViewModel()
        {
            _lines = InitializeLines();
            Logs = new ObservableCollection<string>();
            Retailer.Instance.Packed += OnPacked;
            Retailer.Instance.Purchase += OnPurchase;
        }

        public IEnumerable<LineSummaryViewModel> Lines => _lines.Values;
        public ObservableCollection<string> Logs { get; }

        private IDictionary<SandwichKind, LineSummaryViewModel> InitializeLines()
        {
            var result = new Dictionary<SandwichKind, LineSummaryViewModel>();

            foreach (var value in EnumHelper.GetValues<SandwichKind>())
            {
                result.Add(value, new LineSummaryViewModel(value));
            }

            return result;
        }

        private void OnPurchase(DateTimeOffset time, Sandwich product)
        {
            _lines[product.Kind].Reduce();

            Log($"{time} purchase {product.Kind}");
        }

        private void OnPacked(PackingSummary summary)
        {
            summary.Positions.ForEach(TopUp);

            /* add total number of added items to log statement */
            Log($"{summary.TimeStamp} topped up, {summary.Vendor}.");
        }

        private void TopUp(LineSummary summaryPosition)
        {
	        _lines[summaryPosition.Kind].TopUp(summaryPosition.Added);
        }

        private void Log(string message)
        {
	        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
	        {
		        Logs.Add(message);
	        });
        }
    }
}