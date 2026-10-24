using System.Collections.Generic;

namespace UnityEngine.CSSLayout;

internal class LockDictionary<TKey, TValue>
{
	private object _cacheLock = new object();

	private Dictionary<TKey, TValue> _cacheItemDictionary = new Dictionary<TKey, TValue>();

	public bool TryGetValue(TKey key, out TValue cacheItem)
	{
		bool flag;
		lock (_cacheLock)
		{
			flag = _cacheItemDictionary.TryGetValue(key, out cacheItem);
		}
		if (!flag)
		{
			cacheItem = default;
		}
		return flag;
	}
}
