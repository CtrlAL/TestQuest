using System.Collections.Generic;

public static class ListExtensions
{
	private static readonly System.Random _random = new System.Random();

	public static T RandomItem<T>(this IList<T> list)
	{
		if (list == null || list.Count == 0)
			return default(T);

		return list[_random.Next(0, list.Count)];
	}
}