namespace LevelUpCSharp.Collections
{
	public interface IKindable<TKey>
	{
		TKey Kind { get; }
	}
}