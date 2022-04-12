using System;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Beef : IKeyIngredient
	{
		public Beef()
		{
			ExpDate = DateTime.Now.AddDays(1);
		}

		public DateTime ExpDate { get; }

		public SandwichKind Kind => SandwichKind.Beef;
	}
}