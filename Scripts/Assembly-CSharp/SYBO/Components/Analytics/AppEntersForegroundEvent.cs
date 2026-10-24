namespace SYBO.Components.Analytics;

public class AppEntersForegroundEvent : BaseSessionEvent
{
	public override string EventName => "app_enters_foreground";
}
