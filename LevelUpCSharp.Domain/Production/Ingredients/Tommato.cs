using System;

namespace LevelUpCSharp.Production.Ingredients
{
	internal class Tommato : IGarnish
	{
		public Tommato()
		{
			ExpDate = DateTime.Now.AddDays(4);
		}

		public DateTime ExpDate { get; }

		public string Name => "Tommato";
	}
}