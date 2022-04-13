using System;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Pork : IKeyIngredient
	{
		public DateTime ExpDate { get; }
		public SandwichKind Kind => SandwichKind.Pork;
	}
}