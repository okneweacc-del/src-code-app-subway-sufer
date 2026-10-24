namespace System.Net;

public interface IWebProxy
{
	Uri GetProxy(Uri destination);

	bool IsBypassed(Uri host);
}
