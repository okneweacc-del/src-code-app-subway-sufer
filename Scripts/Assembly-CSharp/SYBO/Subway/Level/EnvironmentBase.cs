using UnityEngine;

namespace SYBO.Subway.Level;

public class EnvironmentBase : EnvironmentObject
{
	public enum EnvironmentBaseType
	{
		Tracks = 0,
		Empty = 1
	}

	[SerializeField]
	private EnvironmentBaseType _type;

	[SerializeField]
	[Range(0f, 3f)]
	private int _numTracks = 3;

	[Range(2f, 100f)]
	[SerializeField]
	private int _blockCount;

	public EnvironmentBaseType Type => _type;

	public int NumTracks => _numTracks;

	public int BlockCount => _blockCount;

	private void OnValidate()
	{
		_blockCount = Mathf.Clamp(_blockCount, 2, int.MaxValue);
		_blockCount = _blockCount / 2 * 2;
	}
}
