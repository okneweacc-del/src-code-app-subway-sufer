using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[Serializable]
[ComVisible(true)]
[MonoTODO("Serialization format not compatible with .NET")]
public sealed class Evidence : IEnumerable, ICollection
{
	private class EvidenceEnumerator : IEnumerator
	{
		private IEnumerator currentEnum;

		private IEnumerator hostEnum;

		private IEnumerator assemblyEnum;

		public object Current => currentEnum.Current;

		public EvidenceEnumerator(IEnumerator hostenum, IEnumerator assemblyenum)
		{
			hostEnum = hostenum;
			assemblyEnum = assemblyenum;
			currentEnum = hostEnum;
		}

		public bool MoveNext()
		{
			if (currentEnum == null)
			{
				return false;
			}
			bool flag = currentEnum.MoveNext();
			if (!flag && hostEnum == currentEnum && assemblyEnum != null)
			{
				currentEnum = assemblyEnum;
				flag = assemblyEnum.MoveNext();
			}
			return flag;
		}

		public void Reset()
		{
			if (hostEnum != null)
			{
				hostEnum.Reset();
				currentEnum = hostEnum;
			}
			else
			{
				currentEnum = assemblyEnum;
			}
			if (assemblyEnum != null)
			{
				assemblyEnum.Reset();
			}
		}
	}

	private bool _locked;

	private ArrayList hostEvidenceList;

	private ArrayList assemblyEvidenceList;

	private int _hashCode;

	public int Count
	{
		get
		{
			int num = 0;
			if (hostEvidenceList != null)
			{
				num += hostEvidenceList.Count;
			}
			if (assemblyEvidenceList != null)
			{
				num += assemblyEvidenceList.Count;
			}
			return num;
		}
	}

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	internal ArrayList HostEvidenceList
	{
		get
		{
			if (hostEvidenceList == null)
			{
				hostEvidenceList = ArrayList.Synchronized(new ArrayList());
			}
			return hostEvidenceList;
		}
	}

	internal ArrayList AssemblyEvidenceList
	{
		get
		{
			if (assemblyEvidenceList == null)
			{
				assemblyEvidenceList = ArrayList.Synchronized(new ArrayList());
			}
			return assemblyEvidenceList;
		}
	}

	public Evidence()
	{
	}

	public Evidence(Evidence evidence)
	{
		if (evidence != null)
		{
			Merge(evidence);
		}
	}

	public void AddAssembly(object id)
	{
		AssemblyEvidenceList.Add(id);
		_hashCode = 0;
	}

	public void AddHost(object id)
	{
		if (_locked && SecurityManager.SecurityEnabled)
		{
			new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
		}
		HostEvidenceList.Add(id);
		_hashCode = 0;
	}

	public void CopyTo(Array array, int index)
	{
		int num = 0;
		if (hostEvidenceList != null)
		{
			num = hostEvidenceList.Count;
			if (num > 0)
			{
				hostEvidenceList.CopyTo(array, index);
			}
		}
		if (assemblyEvidenceList != null && assemblyEvidenceList.Count > 0)
		{
			assemblyEvidenceList.CopyTo(array, index + num);
		}
	}

	[ComVisible(false)]
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is Evidence evidence))
		{
			return false;
		}
		if (HostEvidenceList.Count != evidence.HostEvidenceList.Count)
		{
			return false;
		}
		if (AssemblyEvidenceList.Count != evidence.AssemblyEvidenceList.Count)
		{
			return false;
		}
		for (int i = 0; i < hostEvidenceList.Count; i++)
		{
			bool flag = false;
			int num = 0;
			while (num < evidence.hostEvidenceList.Count)
			{
				if (hostEvidenceList[i].Equals(evidence.hostEvidenceList[num]))
				{
					flag = true;
					break;
				}
				i++;
			}
			if (!flag)
			{
				return false;
			}
		}
		for (int j = 0; j < assemblyEvidenceList.Count; j++)
		{
			bool flag2 = false;
			int num2 = 0;
			while (num2 < evidence.assemblyEvidenceList.Count)
			{
				if (assemblyEvidenceList[j].Equals(evidence.assemblyEvidenceList[num2]))
				{
					flag2 = true;
					break;
				}
				j++;
			}
			if (!flag2)
			{
				return false;
			}
		}
		return true;
	}

	public IEnumerator GetEnumerator()
	{
		IEnumerator hostenum = null;
		if (hostEvidenceList != null)
		{
			hostenum = hostEvidenceList.GetEnumerator();
		}
		IEnumerator assemblyenum = null;
		if (assemblyEvidenceList != null)
		{
			assemblyenum = assemblyEvidenceList.GetEnumerator();
		}
		return new EvidenceEnumerator(hostenum, assemblyenum);
	}

	[ComVisible(false)]
	public override int GetHashCode()
	{
		if (_hashCode == 0)
		{
			if (hostEvidenceList != null)
			{
				for (int i = 0; i < hostEvidenceList.Count; i++)
				{
					_hashCode ^= hostEvidenceList[i].GetHashCode();
				}
			}
			if (assemblyEvidenceList != null)
			{
				for (int j = 0; j < assemblyEvidenceList.Count; j++)
				{
					_hashCode ^= assemblyEvidenceList[j].GetHashCode();
				}
			}
		}
		return _hashCode;
	}

	public IEnumerator GetHostEnumerator()
	{
		return HostEvidenceList.GetEnumerator();
	}

	public void Merge(Evidence evidence)
	{
		if (evidence == null || evidence.Count <= 0)
		{
			return;
		}
		if (evidence.hostEvidenceList != null)
		{
			foreach (object hostEvidence in evidence.hostEvidenceList)
			{
				AddHost(hostEvidence);
			}
		}
		if (evidence.assemblyEvidenceList != null)
		{
			foreach (object assemblyEvidence in evidence.assemblyEvidenceList)
			{
				AddAssembly(assemblyEvidence);
			}
		}
		_hashCode = 0;
	}

	internal static Evidence GetDefaultHostEvidence(Assembly a)
	{
		return new Evidence();
	}
}
