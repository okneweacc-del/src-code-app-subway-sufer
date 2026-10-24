using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Xml;

namespace System.Security;

[Serializable]
[ComVisible(true)]
public sealed class SecurityElement
{
	internal class SecurityAttribute
	{
		private string _name;

		private string _value;

		public string Name => _name;

		public string Value => _value;

		public SecurityAttribute(string name, string value)
		{
			if (!IsValidAttributeName(name))
			{
				throw new ArgumentException(Locale.GetText("Invalid XML attribute name") + ": " + name);
			}
			if (!IsValidAttributeValue(value))
			{
				throw new ArgumentException(Locale.GetText("Invalid XML attribute value") + ": " + value);
			}
			_name = name;
			_value = Unescape(value);
		}
	}

	private string text;

	private string tag;

	private ArrayList attributes;

	private ArrayList children;

	private static readonly char[] invalid_tag_chars = new char[3] { ' ', '<', '>' };

	private static readonly char[] invalid_text_chars = new char[2] { '<', '>' };

	private static readonly char[] invalid_attr_name_chars = new char[3] { ' ', '<', '>' };

	private static readonly char[] invalid_attr_value_chars = new char[3] { '"', '<', '>' };

	private static readonly char[] invalid_chars = new char[5] { '<', '>', '"', '\'', '&' };

	public Hashtable Attributes
	{
		get
		{
			if (attributes == null)
			{
				return null;
			}
			Hashtable hashtable = new Hashtable(attributes.Count);
			foreach (SecurityAttribute attribute in attributes)
			{
				hashtable.Add(attribute.Name, attribute.Value);
			}
			return hashtable;
		}
	}

	public ArrayList Children => children;

	public string Tag => tag;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (value != null && !IsValidText(value))
			{
				throw new ArgumentException(Locale.GetText("Invalid XML string") + ": " + value);
			}
			text = Unescape(value);
		}
	}

	public SecurityElement(string tag)
		: this(tag, null)
	{
	}

	public SecurityElement(string tag, string text)
	{
		if (tag == null)
		{
			throw new ArgumentNullException("tag");
		}
		if (!IsValidTag(tag))
		{
			throw new ArgumentException(Locale.GetText("Invalid XML string") + ": " + tag);
		}
		this.tag = tag;
		Text = text;
	}

	public void AddAttribute(string name, string value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (GetAttribute(name) != null)
		{
			throw new ArgumentException(Locale.GetText("Duplicate attribute : " + name));
		}
		if (attributes == null)
		{
			attributes = new ArrayList();
		}
		attributes.Add(new SecurityAttribute(name, value));
	}

	public void AddChild(SecurityElement child)
	{
		if (child == null)
		{
			throw new ArgumentNullException("child");
		}
		if (children == null)
		{
			children = new ArrayList();
		}
		children.Add(child);
	}

	public string Attribute(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return GetAttribute(name)?.Value;
	}

	public static string Escape(string str)
	{
		if (str == null)
		{
			return null;
		}
		if (str.IndexOfAny(invalid_chars) == -1)
		{
			return str;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int length = str.Length;
		for (int i = 0; i < length; i++)
		{
			char c = str[i];
			switch (c)
			{
			case '<':
				stringBuilder.Append("&lt;");
				break;
			case '>':
				stringBuilder.Append("&gt;");
				break;
			case '"':
				stringBuilder.Append("&quot;");
				break;
			case '\'':
				stringBuilder.Append("&apos;");
				break;
			case '&':
				stringBuilder.Append("&amp;");
				break;
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	private static string Unescape(string str)
	{
		if (str == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(str);
		stringBuilder.Replace("&lt;", "<");
		stringBuilder.Replace("&gt;", ">");
		stringBuilder.Replace("&amp;", "&");
		stringBuilder.Replace("&quot;", "\"");
		stringBuilder.Replace("&apos;", "'");
		return stringBuilder.ToString();
	}

	public static SecurityElement FromString(string xml)
	{
		if (xml == null)
		{
			throw new ArgumentNullException("xml");
		}
		if (xml.Length == 0)
		{
			throw new XmlSyntaxException(Locale.GetText("Empty string."));
		}
		try
		{
			SecurityParser securityParser = new SecurityParser();
			securityParser.LoadXml(xml);
			return securityParser.ToXml();
		}
		catch (Exception inner)
		{
			string message = Locale.GetText("Invalid XML.");
			throw new XmlSyntaxException(message, inner);
		}
	}

	public static bool IsValidAttributeName(string name)
	{
		return name != null && name.IndexOfAny(invalid_attr_name_chars) == -1;
	}

	public static bool IsValidAttributeValue(string value)
	{
		return value != null && value.IndexOfAny(invalid_attr_value_chars) == -1;
	}

	public static bool IsValidTag(string tag)
	{
		return tag != null && tag.IndexOfAny(invalid_tag_chars) == -1;
	}

	public static bool IsValidText(string text)
	{
		return text != null && text.IndexOfAny(invalid_text_chars) == -1;
	}

	public SecurityElement SearchForChildByTag(string tag)
	{
		if (tag == null)
		{
			throw new ArgumentNullException("tag");
		}
		if (children == null)
		{
			return null;
		}
		for (int i = 0; i < children.Count; i++)
		{
			SecurityElement securityElement = (SecurityElement)children[i];
			if (securityElement.tag == tag)
			{
				return securityElement;
			}
		}
		return null;
	}

	public override string ToString()
	{
		StringBuilder s = new StringBuilder();
		ToXml(ref s, 0);
		return s.ToString();
	}

	private void ToXml(ref StringBuilder s, int level)
	{
		s.Append("<");
		s.Append(tag);
		if (attributes != null)
		{
			s.Append(" ");
			for (int i = 0; i < attributes.Count; i++)
			{
				SecurityAttribute securityAttribute = (SecurityAttribute)attributes[i];
				s.Append(securityAttribute.Name).Append("=\"").Append(Escape(securityAttribute.Value))
					.Append("\"");
				if (i != attributes.Count - 1)
				{
					s.Append(Environment.NewLine);
				}
			}
		}
		if ((text == null || text == string.Empty) && (children == null || children.Count == 0))
		{
			s.Append("/>").Append(Environment.NewLine);
			return;
		}
		s.Append(">").Append(Escape(text));
		if (children != null)
		{
			s.Append(Environment.NewLine);
			foreach (SecurityElement child in children)
			{
				child.ToXml(ref s, level + 1);
			}
		}
		s.Append("</").Append(tag).Append(">")
			.Append(Environment.NewLine);
	}

	internal SecurityAttribute GetAttribute(string name)
	{
		if (attributes != null)
		{
			foreach (SecurityAttribute attribute in attributes)
			{
				if (attribute.Name == name)
				{
					return attribute;
				}
			}
		}
		return null;
	}
}
