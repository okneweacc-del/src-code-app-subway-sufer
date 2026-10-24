using System;

namespace UnityEngine;

public enum EventType
{
	MouseDown = 0,
	MouseUp = 1,
	MouseMove = 2,
	MouseDrag = 3,
	KeyDown = 4,
	KeyUp = 5,
	ScrollWheel = 6,
	Repaint = 7,
	Layout = 8,
	DragUpdated = 9,
	DragPerform = 10,
	DragExited = 15,
	Ignore = 11,
	Used = 12,
	ValidateCommand = 13,
	ExecuteCommand = 14,
	ContextClick = 16,
	MouseEnterWindow = 20,
	MouseLeaveWindow = 21,
	[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
	mouseDown = MouseDown,
	[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
	mouseUp = MouseUp,
	[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
	mouseMove = MouseMove,
	[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
	mouseDrag = MouseDrag,
	[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
	keyDown = KeyDown,
	[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
	keyUp = KeyUp,
	[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
	scrollWheel = ScrollWheel,
	[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
	repaint = Repaint,
	[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
	layout = Layout,
	[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
	dragUpdated = DragUpdated,
	[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
	dragPerform = DragPerform,
	[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
	ignore = Ignore,
	[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
	used = Used
}
