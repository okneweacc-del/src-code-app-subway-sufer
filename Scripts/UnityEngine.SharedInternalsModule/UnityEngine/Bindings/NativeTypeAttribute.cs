using System;

namespace UnityEngine.Bindings;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
[VisibleToOtherModules]
internal class NativeTypeAttribute : Attribute, IBindingsHeaderProviderAttribute, IBindingsGenerateMarshallingTypeAttribute, IBindingsAttribute
{
	public string Header { set; }

	public string IntermediateScriptingStructName { set; }

	public CodegenOptions CodegenOptions { set; }

	public NativeTypeAttribute()
	{
		CodegenOptions = CodegenOptions.Auto;
	}

	public NativeTypeAttribute(CodegenOptions codegenOptions)
	{
		CodegenOptions = codegenOptions;
	}

	public NativeTypeAttribute(string header)
	{
		if (header == null)
		{
			throw new ArgumentNullException("header");
		}
		if (header == "")
		{
			throw new ArgumentException("header cannot be empty", "header");
		}
		CodegenOptions = CodegenOptions.Auto;
		Header = header;
	}

	public NativeTypeAttribute(CodegenOptions codegenOptions, string intermediateStructName)
		: this(codegenOptions)
	{
		IntermediateScriptingStructName = intermediateStructName;
	}
}
