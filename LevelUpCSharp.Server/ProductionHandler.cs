using System.Collections.Generic;
using System.Linq;
using LevelUpCSharp.Networking;
using LevelUpCSharp.Production;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Server
{
    [Ctrl("p")]
    internal class ProductionHandler
    {
        private readonly IEnumerable<Vendor> _vendors;

        public ProductionHandler(IEnumerable<Vendor> vendors)
        {
            _vendors = vendors;
        }

        [Worker("s")]
        public IEnumerable<Sandwich> Sandwiches()
        {
            return _vendors.SelectMany(v => v.Buy()).ToArray();
        }

        [Worker("b")]
        public IEnumerable<Sandwich> Beef()
        {
	        return _vendors.SelectMany(v => v.Buy()).Where(s => s.Kind == SandwichKind.Beef).ToArray();
        }
    }

    [Ctrl("ps")]
    internal class ProductionHandlerX
    {
	    private readonly IEnumerable<Vendor> _vendors;

	    public ProductionHandlerX(IEnumerable<Vendor> vendors)
	    {
		    _vendors = vendors;
	    }

	    [Worker("y")]
	    public IEnumerable<Sandwich> Sandwiches()
	    {
		    return _vendors.SelectMany(v => v.Buy()).Where(s => s.Kind == SandwichKind.Pork).ToArray();
	    }
    }
}
