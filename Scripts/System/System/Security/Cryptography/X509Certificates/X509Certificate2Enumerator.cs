using System.Collections;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509Certificate2Enumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public X509Certificate2 Current => (X509Certificate2)enumerator.Current;

	internal X509Certificate2Enumerator(X509Certificate2Collection collection)
	{
		enumerator = ((IEnumerable)collection).GetEnumerator();
	}

	bool IEnumerator.MoveNext()
	{
		return enumerator.MoveNext();
	}

	void IEnumerator.Reset()
	{
		enumerator.Reset();
	}

	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	public void Reset()
	{
		enumerator.Reset();
	}
}
