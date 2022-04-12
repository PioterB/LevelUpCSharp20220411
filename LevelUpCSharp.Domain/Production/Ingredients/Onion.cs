using System;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Onion : IGarnish
	{
		public DateTime ExpDate { get; }

		public string Name => "Onion";
	}
}