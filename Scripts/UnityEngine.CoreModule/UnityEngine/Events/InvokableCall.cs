using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngineInternal;

namespace UnityEngine.Events;

internal class InvokableCall : BaseInvokableCall
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private UnityAction Delegate__BackingField;

	private event UnityAction Delegate
	{
		add
		{
			UnityAction unityAction = Delegate__BackingField;
			UnityAction unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref Delegate__BackingField, (UnityAction)System.Delegate.Combine(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
		remove
		{
			UnityAction unityAction = Delegate__BackingField;
			UnityAction unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref Delegate__BackingField, (UnityAction)System.Delegate.Remove(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
	}

	public InvokableCall(object target, MethodInfo theFunction)
		: base(target, theFunction)
	{
		Delegate += (UnityAction)theFunction.CreateDelegate(typeof(UnityAction), target);
	}

	public InvokableCall(UnityAction action)
	{
		Delegate += action;
	}

	public override void Invoke(object[] args)
	{
		if (BaseInvokableCall.AllowInvoke(Delegate__BackingField))
		{
			Delegate__BackingField();
		}
	}

	public void Invoke()
	{
		if (BaseInvokableCall.AllowInvoke(Delegate__BackingField))
		{
			Delegate__BackingField();
		}
	}

	public override bool Find(object targetObj, MethodInfo method)
	{
		return Delegate__BackingField.Target == targetObj && Delegate__BackingField.GetMethodInfo().Equals(method);
	}
}
internal class InvokableCall<T1> : BaseInvokableCall
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private UnityAction<T1> Delegate__BackingField;

	protected event UnityAction<T1> Delegate
	{
		add
		{
			UnityAction<T1> unityAction = this.Delegate;
			UnityAction<T1> unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref this.Delegate, (UnityAction<T1>)System.Delegate.Combine(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
		remove
		{
			UnityAction<T1> unityAction = this.Delegate;
			UnityAction<T1> unityAction2;
			do
			{
				unityAction2 = unityAction;
				unityAction = Interlocked.CompareExchange(ref this.Delegate, (UnityAction<T1>)System.Delegate.Remove(unityAction2, value), unityAction);
			}
			while ((object)unityAction != unityAction2);
		}
	}

	public InvokableCall(object target, MethodInfo theFunction)
		: base(target, theFunction)
	{
		Delegate += (UnityAction<T1>)theFunction.CreateDelegate(typeof(UnityAction<T1>), target);
	}

	public InvokableCall(UnityAction<T1> action)
	{
		Delegate += action;
	}

	public override void Invoke(object[] args)
	{
		if (args.Length != 1)
		{
			throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
		}
		BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
		if (BaseInvokableCall.AllowInvoke(this.Delegate))
		{
			this.Delegate((T1)args[0]);
		}
	}

	public virtual void Invoke(T1 args0)
	{
		if (BaseInvokableCall.AllowInvoke(this.Delegate))
		{
			this.Delegate(args0);
		}
	}

	public override bool Find(object targetObj, MethodInfo method)
	{
		return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
	}
}
internal class InvokableCall<T1, T2> : BaseInvokableCall
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private UnityAction<T1, T2> Delegate;

	public InvokableCall(object target, MethodInfo theFunction)
		: base(target, theFunction)
	{
		Delegate = (UnityAction<T1, T2>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2>), target);
	}

	public override void Invoke(object[] args)
	{
		if (args.Length != 2)
		{
			throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
		}
		BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
		BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
		if (BaseInvokableCall.AllowInvoke(Delegate))
		{
			Delegate((T1)args[0], (T2)args[1]);
		}
	}

	public override bool Find(object targetObj, MethodInfo method)
	{
		return Delegate.Target == targetObj && Delegate.GetMethodInfo().Equals(method);
	}
}
internal class InvokableCall<T1, T2, T3> : BaseInvokableCall
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private UnityAction<T1, T2, T3> Delegate;

	public InvokableCall(object target, MethodInfo theFunction)
		: base(target, theFunction)
	{
		Delegate = (UnityAction<T1, T2, T3>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target);
	}

	public override void Invoke(object[] args)
	{
		if (args.Length != 3)
		{
			throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
		}
		BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
		BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
		BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
		if (BaseInvokableCall.AllowInvoke(Delegate))
		{
			Delegate((T1)args[0], (T2)args[1], (T3)args[2]);
		}
	}

	public override bool Find(object targetObj, MethodInfo method)
	{
		return Delegate.Target == targetObj && Delegate.GetMethodInfo().Equals(method);
	}
}
internal class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
{
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private UnityAction<T1, T2, T3, T4> Delegate;

	public InvokableCall(object target, MethodInfo theFunction)
		: base(target, theFunction)
	{
		Delegate = (UnityAction<T1, T2, T3, T4>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3, T4>), target);
	}

	public override void Invoke(object[] args)
	{
		if (args.Length != 4)
		{
			throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
		}
		BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
		BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
		BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
		BaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
		if (BaseInvokableCall.AllowInvoke(Delegate))
		{
			Delegate((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
		}
	}

	public override bool Find(object targetObj, MethodInfo method)
	{
		return Delegate.Target == targetObj && Delegate.GetMethodInfo().Equals(method);
	}
}
