using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[ExcludeFromPreset]
public sealed class AssetBundle : Object
{
	private AssetBundle()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundleCreateRequest LoadFromFileAsync(string path, [DefaultValue("0")] uint crc, [DefaultValue("0")] ulong offset);

	[ExcludeFromDocs]
	public static AssetBundleCreateRequest LoadFromFileAsync(string path)
	{
		ulong offset = 0uL;
		uint crc = 0u;
		return LoadFromFileAsync(path, crc, offset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern AssetBundle LoadFromFile(string path, [DefaultValue("0")] uint crc, [DefaultValue("0")] ulong offset);

	[ExcludeFromDocs]
	public static AssetBundle LoadFromFile(string path)
	{
		ulong offset = 0uL;
		uint crc = 0u;
		return LoadFromFile(path, crc, offset);
	}

	public T LoadAsset<T>(string name) where T : Object
	{
		return (T)LoadAsset(name, typeof(T));
	}

	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	public Object LoadAsset(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAsset_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
	private extern Object LoadAsset_Internal(string name, Type type);

	public AssetBundleRequest LoadAssetAsync<T>(string name)
	{
		return LoadAssetAsync(name, typeof(T));
	}

	public AssetBundleRequest LoadAssetAsync(string name, Type type)
	{
		if (name == null)
		{
			throw new NullReferenceException("The input asset name cannot be null.");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("The input asset name cannot be empty.");
		}
		if (type == null)
		{
			throw new NullReferenceException("The input type cannot be null.");
		}
		return LoadAssetAsync_Internal(name, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern AssetBundleRequest LoadAssetAsync_Internal(string name, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Unload(bool unloadAllLoadedObjects);
}
