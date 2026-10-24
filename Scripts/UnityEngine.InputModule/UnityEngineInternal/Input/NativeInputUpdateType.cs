namespace UnityEngineInternal.Input;

public enum NativeInputUpdateType
{
	BeginFixed = 0,
	EndFixed = 1,
	BeginDynamic = 2,
	EndDynamic = 3,
	BeginBeforeRender = 4,
	EndBeforeRender = 5,
	BeginEditor = 6,
	EndEditor = 7
}
