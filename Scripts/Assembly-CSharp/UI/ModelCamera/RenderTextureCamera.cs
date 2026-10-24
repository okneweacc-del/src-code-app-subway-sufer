using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.ModelCamera;

public class RenderTextureCamera
{
	private GameObject _cameraGo;

	private readonly Camera _camera;

	public UnityEngine.RenderTexture renderTexture;

	public Camera camera => _camera;

	public bool isActive { get; private set; }

	public int cameraLayer => _cameraGo.layer;

	public RenderTextureCamera(Camera camera, GameObject cameraGo)
	{
		_camera = camera;
		_cameraGo = cameraGo;
		Deactivate();
	}

	public Transform GetAnchor()
	{
		return _cameraGo.transform.GetChild(0);
	}

	public void Activate(CameraSettings overrideCameraSettings)
	{
		CameraSettings.ApplyRenderTextureCameraSettings(_camera, overrideCameraSettings);
		isActive = true;
		_cameraGo.SetActive(value: true);
	}

	public void Deactivate()
	{
		isActive = false;
		_cameraGo.SetActive(value: false);
		GetAnchor().KillChildren();
		ClearRenderTexture(renderTexture);
	}

	private void ClearRenderTexture(UnityEngine.RenderTexture renderTextureToClear)
	{
		if (renderTextureToClear != null)
		{
			UnityEngine.RenderTexture active = UnityEngine.RenderTexture.active;
			UnityEngine.RenderTexture.active = renderTextureToClear;
			GL.Clear(clearDepth: true, clearColor: true, Color.clear);
			UnityEngine.RenderTexture.active = active;
		}
	}
}
