namespace Mono.Security.Protocol.Tls;

public class ValidationResult
{
	private bool trusted;

	private int error_code;

	public bool Trusted => trusted;

	public int ErrorCode => error_code;
}
