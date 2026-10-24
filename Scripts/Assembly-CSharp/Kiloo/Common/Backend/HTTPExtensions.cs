using JetBrains.Annotations;
using Kiloo.Common.HTTP;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Backend;

public static class HTTPExtensions
{
	public static Error GetServiceError([NotNull] this IRequest request, [NotNull] IJsonSerializer jsonSerializer)
	{
		if (!request.isSuccessful)
		{
			IResponse response = request.response;
			if (response != null && response.TryParseJson<ServiceResponse<object>>(out var result, jsonSerializer) && result != null)
			{
				return result.error;
			}
		}
		return null;
	}
}
