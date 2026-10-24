namespace System.Reflection.Emit;

internal struct MonoWin32Resource(int res_type, int res_id, int lang_id, byte[] data)
{
	public int res_type = res_type;

	public int res_id = res_id;

	public int lang_id = lang_id;

	public byte[] data = data;
}
