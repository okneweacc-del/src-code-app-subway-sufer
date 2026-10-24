using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Camera/RenderManager.h")]
[NativeHeader("Runtime/Shaders/Shader.h")]
[NativeHeader("Runtime/Misc/GameObjectUtility.h")]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
public sealed class Camera : Behaviour
{
	public delegate void CameraCallback(Camera cam);

	public static CameraCallback onPreCull;

	public static CameraCallback onPreRender;

	public static CameraCallback onPostRender;

	public extern int targetDisplay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern Camera[] allCameras
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static extern int allCamerasCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern float[] layerCullDistances
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[NativeProperty("Near")]
	public extern float nearClipPlane
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("Far")]
	public extern float farClipPlane
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("Fov")]
	public extern float fieldOfView
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float orthographicSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern bool orthographic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern TransparencySortMode transparencySortMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern float aspect
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int cullingMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public extern int eventMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Color backgroundColor
	{
		get
		{
			get_backgroundColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_backgroundColor_Injected(ref value);
		}
	}

	public extern CameraClearFlags clearFlags
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("NormalizedViewportRect")]
	public Rect rect
	{
		get
		{
			get_rect_Injected(out var ret);
			return ret;
		}
		set
		{
			set_rect_Injected(ref value);
		}
	}

	[NativeProperty("ScreenViewportRect")]
	public Rect pixelRect
	{
		get
		{
			get_pixelRect_Injected(out var ret);
			return ret;
		}
	}

	public extern int pixelWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetPixelWidth", HasExplicitThis = true)]
		get;
	}

	public extern int pixelHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("CameraScripting::GetPixelHeight", HasExplicitThis = true)]
		get;
	}

	public extern RenderTexture targetTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Matrix4x4 worldToCameraMatrix
	{
		get
		{
			get_worldToCameraMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_worldToCameraMatrix_Injected(ref value);
		}
	}

	public Matrix4x4 projectionMatrix
	{
		get
		{
			get_projectionMatrix_Injected(out var ret);
			return ret;
		}
		set
		{
			set_projectionMatrix_Injected(ref value);
		}
	}

	public static extern Camera main
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("FindMainCamera")]
		get;
	}

	public static extern Camera current
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetCurrentCameraPtr")]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetAllCameras(Camera[] cameras);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Render();

	public bool RenderToCubemap(RenderTexture cubemap, [DefaultValue("63")] int faceMask)
	{
		return Internal_RenderToCubemapRT(cubemap, faceMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetReplacementShader(Shader shader, string replacementTag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetReplacementShader();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResetProjectionMatrix();

	public Vector3 WorldToScreenPoint(Vector3 position)
	{
		WorldToScreenPoint_Injected(ref position, out var ret);
		return ret;
	}

	public Vector3 WorldToViewportPoint(Vector3 position)
	{
		WorldToViewportPoint_Injected(ref position, out var ret);
		return ret;
	}

	public Vector3 ViewportToWorldPoint(Vector3 position)
	{
		ViewportToWorldPoint_Injected(ref position, out var ret);
		return ret;
	}

	public Vector3 ScreenToWorldPoint(Vector3 position)
	{
		ScreenToWorldPoint_Injected(ref position, out var ret);
		return ret;
	}

	public Vector3 ScreenToViewportPoint(Vector3 position)
	{
		ScreenToViewportPoint_Injected(ref position, out var ret);
		return ret;
	}

	private Ray ScreenPointToRay(Vector2 pos)
	{
		ScreenPointToRay_Injected(ref pos, out var ret);
		return ret;
	}

	public Ray ScreenPointToRay(Vector3 pos)
	{
		return ScreenPointToRay((Vector2)pos);
	}

	[FreeFunction("CameraScripting::RaycastTry", HasExplicitThis = true)]
	internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
	{
		return RaycastTry_Injected(ref ray, distance, layerMask);
	}

	[FreeFunction("CameraScripting::RaycastTry2D", HasExplicitThis = true)]
	internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
	{
		return RaycastTry2D_Injected(ref ray, distance, layerMask);
	}

	[RequiredByNativeCode]
	private static void FireOnPreCull(Camera cam)
	{
		if (onPreCull != null)
		{
			onPreCull(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPreRender(Camera cam)
	{
		if (onPreRender != null)
		{
			onPreRender(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPostRender(Camera cam)
	{
		if (onPostRender != null)
		{
			onPostRender(cam);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_backgroundColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_backgroundColor_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rect_Injected(out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rect_Injected(ref Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_pixelRect_Injected(out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_worldToCameraMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_worldToCameraMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_projectionMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_projectionMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToScreenPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void WorldToViewportPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ViewportToWorldPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenToWorldPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenToViewportPoint_Injected(ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ScreenPointToRay_Injected(ref Vector2 pos, out Ray ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject RaycastTry_Injected(ref Ray ray, float distance, int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject RaycastTry2D_Injected(ref Ray ray, float distance, int layerMask);
}
