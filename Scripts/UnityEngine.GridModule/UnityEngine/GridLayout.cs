using UnityEngine.Bindings;

namespace UnityEngine;

[NativeType(Header = "Modules/Grid/Public/Grid.h")]
[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
[RequireComponent(typeof(Transform))]
public class GridLayout : Behaviour
{
}
