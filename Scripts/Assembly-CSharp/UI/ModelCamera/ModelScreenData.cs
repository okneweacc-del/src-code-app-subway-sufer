using SYBO.Subway.Info;
using UnityEngine;

namespace UI.ModelCamera;

public class ModelScreenData
{
	private Transform _modelTranform;

	private GameObject _modelGo;

	public CharacterModel characterModel { get; private set; }

	public Camera camera { get; private set; }

	public Characters.CharacterId characterId { get; set; }

	public int themeVersion { get; set; }

	public Transform modelTranform => _modelTranform;

	public GameObject modelGo => _modelGo;

	public Transform cameraTransform => camera.transform;

	public bool isModelActive => _modelTranform.gameObject.activeSelf;

	public bool isCameraActive => camera != null && camera.gameObject.activeSelf;

	public int cameraLayer => camera.gameObject.layer;

	public ModelScreenData(GameObject modelGo, Camera camera, Characters.CharacterId characterId, int themeVersion)
	{
		_modelGo = modelGo;
		_modelTranform = modelGo.transform;
		this.camera = camera;
		this.characterId = characterId;
		this.themeVersion = themeVersion;
		characterModel = modelTranform.GetComponent<CharacterModel>();
	}

	public void Deactivate()
	{
		characterModel.gameObject.SetActive(value: false);
	}

	public void Activate()
	{
		characterModel.gameObject.SetActive(value: true);
	}

	public void UpdateCharacterDataIfNeeded(Characters.CharacterId charId, int currentlySelectedThemeVersion)
	{
		if (characterId != charId)
		{
			characterId = charId;
		}
		if (themeVersion != currentlySelectedThemeVersion)
		{
			themeVersion = currentlySelectedThemeVersion;
		}
	}
}
