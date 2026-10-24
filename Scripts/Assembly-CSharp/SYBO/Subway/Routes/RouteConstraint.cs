using SYBO.Subway.Level;

namespace SYBO.Subway.Routes;

public struct RouteConstraint(EnvironmentKind[] allowedEnvironmentKinds)
{
	public readonly EnvironmentKind[] AllowedEnvironmentKinds = allowedEnvironmentKinds;

	public static RouteConstraint None = new RouteConstraint(new EnvironmentKind[0]);
}
