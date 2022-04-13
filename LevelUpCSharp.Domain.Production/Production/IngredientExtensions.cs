using System.Collections.Generic;

namespace LevelUpCSharp.Production
{
	public static class IngredientExtensions
	{
		public static IEnumerable<string> AsStrings(this IEnumerable<IGarnish> source)
		{
			foreach (IGarnish ingredient in source)
			{
				yield return ingredient.Name;
			}
		}
	}
}