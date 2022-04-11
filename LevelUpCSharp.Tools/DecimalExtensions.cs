using System;

namespace LevelUpCSharp
{
	public static class DecimalExtensions
	{
		public static decimal Multiplay(this decimal source, int factor)
		{
			return source * (1 + (decimal) factor / 100);
		}

		public static decimal GetVat(this decimal source, int vat)
		{
			if (vat < 0 || vat > 23)
			{
				throw new ArgumentException("out of range");
			}

			return source * (1 + (decimal)vat / 100);
		}
	}
}