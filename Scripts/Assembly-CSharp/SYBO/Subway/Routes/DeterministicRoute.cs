using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Routes;

public class DeterministicRoute : Route
{
	[SerializeField]
	private DeterministicRouteBuilder _deterministicRouteBuilder;

	public override RouteType Type => RouteType.Default;

	public override RouteBuilder Builder => _deterministicRouteBuilder;
}
