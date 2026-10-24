using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes;

public class DefaultRoute : Route
{
	[SerializeField]
	private DefaultRouteBuilder _defaultRouteBuilder;

	public override RouteBuilder Builder => _defaultRouteBuilder;

	public override RouteType Type => RouteType.Default;
}
