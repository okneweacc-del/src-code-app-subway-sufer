using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore;

[ComVisible(true)]
public interface ISymbolWriter
{
	void Close();

	void CloseMethod();

	void CloseNamespace();

	ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);

	void DefineLocalVariable(string name, FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1, int addr2, int addr3, int startOffset, int endOffset);

	void DefineSequencePoints(ISymbolDocumentWriter document, int[] offsets, int[] lines, int[] columns, int[] endLines, int[] endColumns);

	void Initialize(IntPtr emitter, string filename, bool fFullBuild);

	void OpenMethod(SymbolToken method);

	void OpenNamespace(string name);

	void SetMethodSourceRange(ISymbolDocumentWriter startDoc, int startLine, int startColumn, ISymbolDocumentWriter endDoc, int endLine, int endColumn);

	void SetSymAttribute(SymbolToken parent, string name, byte[] data);
}
