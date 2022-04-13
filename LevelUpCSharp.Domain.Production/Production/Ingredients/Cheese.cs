using System;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Cheese : IGarnish, IKeyIngredient
	{
		public DateTime ExpDate { get; }
		public string Name { get; }
		public SandwichKind Kind => SandwichKind.Cheese;
	}
}