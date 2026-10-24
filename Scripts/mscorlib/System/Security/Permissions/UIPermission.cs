using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[ComVisible(true)]
public sealed class UIPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
{
	private UIPermissionWindow _window;

	private UIPermissionClipboard _clipboard;

	public UIPermissionClipboard Clipboard
	{
		set
		{
			if (!Enum.IsDefined(typeof(UIPermissionClipboard), value))
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "UIPermissionClipboard");
			}
			_clipboard = value;
		}
	}

	public UIPermissionWindow Window
	{
		set
		{
			if (!Enum.IsDefined(typeof(UIPermissionWindow), value))
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "UIPermissionWindow");
			}
			_window = value;
		}
	}

	public UIPermission(PermissionState state)
	{
		if (CodeAccessPermission.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_clipboard = UIPermissionClipboard.AllClipboard;
			_window = UIPermissionWindow.AllWindows;
		}
	}

	public UIPermission(UIPermissionWindow windowFlag, UIPermissionClipboard clipboardFlag)
	{
		Clipboard = clipboardFlag;
		Window = windowFlag;
	}

	public override void FromXml(SecurityElement esd)
	{
		CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
		if (CodeAccessPermission.IsUnrestricted(esd))
		{
			_window = UIPermissionWindow.AllWindows;
			_clipboard = UIPermissionClipboard.AllClipboard;
			return;
		}
		string text = esd.Attribute("Window");
		if (text == null)
		{
			_window = UIPermissionWindow.NoWindows;
		}
		else
		{
			_window = (UIPermissionWindow)(int)Enum.Parse(typeof(UIPermissionWindow), text);
		}
		string text2 = esd.Attribute("Clipboard");
		if (text2 == null)
		{
			_clipboard = UIPermissionClipboard.NoClipboard;
		}
		else
		{
			_clipboard = (UIPermissionClipboard)(int)Enum.Parse(typeof(UIPermissionClipboard), text2);
		}
	}

	public override bool IsSubsetOf(IPermission target)
	{
		UIPermission uIPermission = Cast(target);
		if (uIPermission == null)
		{
			return IsEmpty(_window, _clipboard);
		}
		if (uIPermission.IsUnrestricted())
		{
			return true;
		}
		return _window <= uIPermission._window && _clipboard <= uIPermission._clipboard;
	}

	public bool IsUnrestricted()
	{
		return _window == UIPermissionWindow.AllWindows && _clipboard == UIPermissionClipboard.AllClipboard;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (_window == UIPermissionWindow.AllWindows && _clipboard == UIPermissionClipboard.AllClipboard)
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			if (_window != UIPermissionWindow.NoWindows)
			{
				securityElement.AddAttribute("Window", _window.ToString());
			}
			if (_clipboard != UIPermissionClipboard.NoClipboard)
			{
				securityElement.AddAttribute("Clipboard", _clipboard.ToString());
			}
		}
		return securityElement;
	}

	private bool IsEmpty(UIPermissionWindow w, UIPermissionClipboard c)
	{
		return w == UIPermissionWindow.NoWindows && c == UIPermissionClipboard.NoClipboard;
	}

	private UIPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		UIPermission uIPermission = target as UIPermission;
		if (uIPermission == null)
		{
			CodeAccessPermission.ThrowInvalidPermission(target, typeof(UIPermission));
		}
		return uIPermission;
	}
}
