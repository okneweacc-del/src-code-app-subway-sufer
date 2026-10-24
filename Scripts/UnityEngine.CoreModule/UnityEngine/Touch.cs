namespace UnityEngine;

public struct Touch
{
	private int m_FingerId;

	private Vector2 m_Position;

	private Vector2 m_RawPosition;

	private Vector2 m_PositionDelta;

	private float m_TimeDelta;

	private int m_TapCount;

	private TouchPhase m_Phase;

	private TouchType m_Type;

	private float m_Pressure;

	private float m_maximumPossiblePressure;

	private float m_Radius;

	private float m_RadiusVariance;

	private float m_AltitudeAngle;

	private float m_AzimuthAngle;

	public int fingerId => m_FingerId;

	public Vector2 position => m_Position;

	public int tapCount => m_TapCount;

	public TouchPhase phase => m_Phase;

	public TouchType type => m_Type;
}
