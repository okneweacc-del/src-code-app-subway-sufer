using Data.Save.Internal;

namespace Data.Save;

public abstract class DataManager<T> where T : DataManager<T>, new()
{
	private static T _instance;

	protected static T instance => _instance ?? (_instance = new T());

	protected static LocalData local
	{
		get
		{
			T val = instance;
			return val.save.local;
		}
	}

	protected static CloudData cloud
	{
		get
		{
			T val = instance;
			return val.save.cloud;
		}
	}

	protected static LocalDebugData localDebug
	{
		get
		{
			T val = instance;
			return val.save.localDebug;
		}
	}

	protected SaveGame save { get; private set; }

	protected DataManager()
	{
		SaveManager.Inject(this);
		SaveManager.saveGameChanged += OnSaveGameChanged;
		if (_instance != null)
		{
			SDebug.LogError($"Singleton instance of type '{GetType()}' was already set, did you manually create a new instance?");
		}
	}

	public void SetSaveGame(SaveGame save)
	{
		this.save = save;
	}

	private void OnSaveGameChanged()
	{
		SaveManager.Inject(this);
	}
}
