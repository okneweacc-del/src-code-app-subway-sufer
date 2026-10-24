using System;
using Info;
using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States.Settings;

public class PogostickStateSettings : ScriptableSingleton<PogostickStateSettings>
{
	[Serializable]
	public class PickupWeight
	{
		public UpgradeType UpgradeType;

		public PickupType PickupType;
	}

	[SerializeField]
	private float _jumpHeight = 95f;

	[SerializeField]
	private float _jumpDistance = 800f;

	[SerializeField]
	private float _characterChangeTrackLength = 60f;

	[SerializeField]
	private float _finalJumpSpeed;

	[SerializeField]
	private int _rows = 5;

	[SerializeField]
	private int _startRowPosition = 1;

	[SerializeField]
	private int _endRowPosition = 1;

	[SerializeField]
	private PickupWeight[] _pickupList;

	[SerializeField]
	private AnimationCurve _jumpCurve;

	[SerializeField]
	private AnimationCurve _cameraCurve;

	[SerializeField]
	private AnimationCurve _cameraAimFollowCurve;

	[SerializeField]
	private float _fadeInPosition = 0.1f;

	[SerializeField]
	private float _hangtimePosition = 0.5f;

	[SerializeField]
	private float _smoothCameraXDuration = 0.05f;

	[SerializeField]
	private Vector3 _cameraAimOffset = Vector3.zero;

	public float JumpHeight => _jumpHeight;

	public float JumpDistance => _jumpDistance;

	public float CharacterChangeTrackLength => _characterChangeTrackLength;

	public float FinalJumpSpeed => _finalJumpSpeed;

	public int Rows => _rows;

	public int StartRowPosition => _startRowPosition;

	public int EndRowPosition => _endRowPosition;

	public PickupWeight[] PickupList => _pickupList;

	public AnimationCurve JumpCurve => _jumpCurve;

	public AnimationCurve CameraCurve => _cameraCurve;

	public AnimationCurve CameraAimFollowCurve => _cameraAimFollowCurve;

	public float FadeInPosition => _fadeInPosition;

	public float HangtimePosition => _hangtimePosition;

	public float SmoothCameraXDuration => _smoothCameraXDuration;

	public Vector3 CameraAimOffset => _cameraAimOffset;
}
