namespace LevelUpCSharp
{
	public static class ObjectExtensions
	{
		public static Result<T> AsSuccess<T>(this T item)
		{
			return Result<T>.Success(item);
		}
	}
}