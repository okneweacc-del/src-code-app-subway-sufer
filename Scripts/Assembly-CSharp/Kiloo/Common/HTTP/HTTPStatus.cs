namespace Kiloo.Common.HTTP;

public static class HTTPStatus
{
	public const int CONTINUE = 100;

	public const int SWITCHING_PROTOCOLS = 101;

	public const int OK = 200;

	public const int CREATED = 201;

	public const int ACCEPTED = 202;

	public const int NON_AUTHORITATIVE_INFORMATION = 203;

	public const int NO_CONTENT = 204;

	public const int RESET_CONTENT = 205;

	public const int PARTIAL_CONTENT = 206;

	public const int MULTIPLE_CHOICES = 300;

	public const int MOVED_PERMANENTLY = 301;

	public const int FOUND = 302;

	public const int SEE_OTHER = 303;

	public const int NOT_MODIFIED = 304;

	public const int USE_PROXY = 305;

	public const int TEMPORARY_REDIRECT = 307;

	public const int BAD_REQUEST = 400;

	public const int UNAUTHORIZED = 401;

	public const int PAYMENT_REQUIRED = 402;

	public const int FORBIDDEN = 403;

	public const int NOT_FOUND = 404;

	public const int METHOD_NOT_ALLOWED = 405;

	public const int NOT_ACCEPTABLE = 406;

	public const int PROXY_AUTHENTICATION_REQUIRED = 407;

	public const int REQUEST_TIMEOUT = 408;

	public const int CONFLICT = 409;

	public const int GONE = 410;

	public const int LENGTH_REQUIRED = 411;

	public const int PRECONDITION_FAILED = 412;

	public const int REQUEST_ENTITY_TOO_LARGE = 413;

	public const int REQUEST_URI_TOO_LONG = 414;

	public const int UNSUPPORTED_MEDIA_TYPE = 415;

	public const int REQUESTED_RANGE_NOT_SATISFIABLE = 416;

	public const int EXPECTATION_FAILED = 417;

	public const int INTERNAL_SERVER_ERROR = 500;

	public const int NOT_IMPLEMENTED = 501;

	public const int BAD_GATEWAY = 502;

	public const int SERVICE_UNAVAILABLE = 503;

	public const int GATEWAY_TIMEOUT = 504;

	public const int HTTP_VERSION_NOT_SUPPORTED = 505;

	public static string ToString(int status)
	{
		return status switch
		{
			100 => "Continue", 
			101 => "Switching Protocols", 
			200 => "OK", 
			201 => "Created", 
			202 => "Accepted", 
			203 => "Non-Authoritative Information", 
			204 => "No Content", 
			205 => "Reset Content", 
			206 => "Partial Content", 
			300 => "Multiple Choices", 
			301 => "Moved Permanently", 
			302 => "Found", 
			303 => "See Other", 
			304 => "Not Modified", 
			305 => "Use Proxy", 
			307 => "Temporary Redirect", 
			400 => "Bad Request", 
			401 => "Unauthorized", 
			402 => "Payment Required", 
			403 => "Forbidden", 
			404 => "Not Found", 
			405 => "Method Not Allowed", 
			406 => "Not Acceptable", 
			407 => "Proxy Authentication Required", 
			408 => "Request Time-out", 
			409 => "Conflict", 
			410 => "Gone", 
			411 => "Length Required", 
			412 => "Precondition Failed", 
			413 => "Request Entity Too Large", 
			414 => "Request-URI Too Long", 
			415 => "Unsupported Media Type", 
			416 => "Requested range not satisfiable", 
			417 => "Expectation Failed", 
			500 => "Internal Server Error", 
			501 => "Not Implemented", 
			502 => "Bad Gateway", 
			503 => "Service Unavailable", 
			504 => "Gateway Time-out", 
			505 => "HTTP Version not supported", 
			_ => "Unknown " + status, 
		};
	}
}
