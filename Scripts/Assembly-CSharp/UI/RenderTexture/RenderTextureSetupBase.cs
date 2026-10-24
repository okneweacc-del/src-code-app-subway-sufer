using System;
using System.Collections.Generic;
using UI.Generic;
using UnityEngine;

namespace UI.RenderTexture;

[Serializable]
public abstract class RenderTextureSetupBase
{
	[Tooltip("Reference to the UI texture that will eventually serve as the UI target for the render texture")]
	[SerializeField]
	protected UITexture texture;

	[Tooltip("List of objects that will be instantiated and rendered into the render texture, incl. their position, rotation, scale and desired animation")]
	[SerializeField]
	protected List<RenderTextureObject> renderTextureObjects;

	protected List<GameObject> _objectInstances = new List<GameObject>();

	protected RenderTextureManager _renderTextureManager;

	public virtual void Setup(CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		_renderTextureManager = RenderTextureManager.instance;
		for (int i = 0; i < renderTextureObjects.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(renderTextureObjects[i].prefab);
			gameObject.transform.localPosition = renderTextureObjects[i].position;
			gameObject.transform.localRotation = Quaternion.Euler(renderTextureObjects[i].rotation);
			gameObject.transform.localScale = renderTextureObjects[i].scale;
			if (renderTextureObjects[i].animationClip != null)
			{
				UnscaledAnimation unscaledAnimation = gameObject.AddComponent<UnscaledAnimation>();
				unscaledAnimation.loop = true;
				unscaledAnimation.playAutomatically = true;
				unscaledAnimation.clip = renderTextureObjects[i].animationClip;
				unscaledAnimation.StartAnimating();
			}
			_objectInstances.Add(gameObject);
		}
		texture.enabled = true;
		texture.mainTexture = _renderTextureManager.GetRenderTextureFor(_objectInstances, texture, overrideCameraSettings, copySettings);
		OnSetup();
	}

	protected virtual void OnSetup()
	{
	}

	public void ReleaseRenderTexture()
	{
		_renderTextureManager.ReleaseTexture((UnityEngine.RenderTexture)texture.mainTexture);
		_objectInstances.Clear();
		texture.mainTexture = null;
		texture.enabled = false;
	}

	protected virtual void OnReleaseRenderTexture()
	{
	}
}
