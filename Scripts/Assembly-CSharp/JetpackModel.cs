using UnityEngine;

public class JetpackModel : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer mesh;

	[SerializeField]
	private Transform jetpackCloudPositionL;

	[SerializeField]
	private Transform jetpackCloudPositionR;

	[SerializeField]
	private Transform[] jetpackThrusters;

	public MeshRenderer Mesh => mesh;

	public Transform JetpackCloudPositionL => jetpackCloudPositionL;

	public Transform JetpackCloudPositionR => jetpackCloudPositionR;

	public Transform[] JetpackThrusters => jetpackThrusters;
}
