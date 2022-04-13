using System;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Deer : IKeyIngredient
	{
		public DateTime ExpDate { get; }
		public SandwichKind Kind => SandwichKind.Deer;
	}
}