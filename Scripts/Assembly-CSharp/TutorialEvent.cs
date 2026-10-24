using System.Collections;
using Data.Save;
using Onboarding;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
	public enum TutorialText
	{
		None = 0,
		FreshMoves = 1,
		DoubleTap = 2,
		YouRock = 3
	}

	private Game game;

	[SerializeField]
	private bool displayText;

	[SerializeField]
	private TutorialText tutorialText;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float direction;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool endTutorial;

	[SerializeField]
	private bool allowHoverboard;

	private Hoverboard hoverboard;

	private Character character;

	private bool _initialized;

	private GameObject _mesh;

	private GameObject mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.gameObject.transform.Find("arrow").gameObject;
			}
			return _mesh;
		}
	}

	private void Awake()
	{
		game = Game.Instance;
		hoverboard = Hoverboard.Instance;
	}

	private void Start()
	{
		if (!(game == null) && !_initialized)
		{
			character = game.Character;
			_initialized = true;
		}
	}

	private IEnumerator ShowArrow()
	{
		mesh.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		mesh.SetActive(value: true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(pTween.To(time, (float t) =>
		{
			mesh.transform.localPosition = Vector3.Lerp(pos - mesh.transform.up * 5f, pos + mesh.transform.up * 5f, t);
			mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!game.IsInGame.Value)
			{
				mesh.transform.localPosition = new Vector3(2000f, 2000f, 0f);
			}
		}));
		mesh.SetActive(value: false);
	}

	private void OnDisable()
	{
		if (_mesh != null)
		{
			_mesh.SetActive(value: false);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!character.StopColliding && collider.gameObject.name.Equals("Character"))
		{
			if (displayText)
			{
				UIScreenController.Instance.QueueMessage(GetStringForTutorial());
			}
			if (displayMesh)
			{
				StartCoroutine(ShowArrow());
			}
			if (allowHoverboard)
			{
				hoverboard.IsAllowed = true;
			}
			if (endTutorial)
			{
				Flurry.LogEvent("Tutorial exit");
				Game.Instance.IsRunningOnTutorialTrack = false;
				OnboardingSave.isTutorialCompleted = true;
				OnboardingTracking.EnableOnboardingTracking(OnboardingTracking.ScreenType.Gameover);
				OnboardingTracking.EnableOnboardingTracking(OnboardingTracking.ScreenType.FrontScreen);
			}
		}
	}

	private string GetStringForTutorial()
	{
		string key = string.Empty;
		switch (tutorialText)
		{
		case TutorialText.FreshMoves:
			key = "TUTORIAL_FRESH_MOVES";
			break;
		case TutorialText.DoubleTap:
			key = "TUTORIAL_DOUBLE_TAP";
			break;
		case TutorialText.YouRock:
			key = "TUTORIAL_YOU_ROCK";
			break;
		}
		return Strings.Get(key);
	}
}
