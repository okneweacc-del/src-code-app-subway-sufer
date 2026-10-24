using System.Collections;
using Info;
using SYBO.Common;
using SYBO.Components.Utilities;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using UnityEngine;

public class CoinMagnet : CharacterModifier
{
	private bool isActive;

	public float pullSpeed = 150f;

	private Character character;

	private CollisionNotifier coinMagnetCollider;

	private CharacterRendering characterRendering;

	private Transform coinEFX;

	private Game game;

	public AudioClipInfo powerDownSound;

	private static CoinMagnet instance;

	public bool IsActive => isActive;

	public static CoinMagnet Instance => instance ?? (instance = Object.FindObjectOfType(typeof(CoinMagnet)) as CoinMagnet);

	public void Awake()
	{
		instance = this;
	}

	public void Initialize()
	{
		character = Character.Instance;
		coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		characterRendering = CharacterRendering.Instance;
		coinMagnetCollider = character.CoinMagnetCollider;
		coinMagnetCollider.TriggerEnter += CoinHit;
		SetUpHoldAnimation();
		game = Game.Instance;
		game.OnCharacterModelRecreated += Game_OnCharacterModelRecreated;
	}

	private void SetUpHoldAnimation()
	{
		characterRendering.CharacterAnimation["hold_magnet"].AddMixingTransform(characterRendering.CharacterModel.ShoulderTransform);
		characterRendering.CharacterAnimation["hold_magnet"].layer = 3;
		characterRendering.CharacterAnimation["hold_magnet"].weight = 0.9f;
		characterRendering.CharacterAnimation["hold_magnet"].enabled = false;
	}

	private void OnDestroy()
	{
		if (coinMagnetCollider != null)
		{
			coinMagnetCollider.TriggerEnter -= CoinHit;
		}
	}

	private void Game_OnCharacterModelRecreated()
	{
		SetUpHoldAnimation();
	}

	public override void Reset()
	{
		base.Reset();
		StopCoinMagnet();
	}

	public override IEnumerator Begin()
	{
		if (character == null)
		{
			Initialize();
		}
		characterRendering.CharacterModel.MeshCoinMagnet.enabled = true;
		GameStats.Instance.PickedUpPowerupsIncrement();
		base.Paused = false;
		SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.Magnet);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		characterRendering.CharacterAnimation["hold_magnet"].enabled = true;
		characterRendering.CharacterAnimation.Play("hold_magnet");
		base.Powerup = GameStats.Instance.TriggerPowerup(UpgradeType.coinmagnet);
		coinMagnetCollider.GetComponent<Collider>().enabled = true;
		isActive = true;
		base.enabled = true;
		stop = StopSignal.DONT_STOP;
		while (base.Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			coinEFX.position = characterRendering.CharacterModel.MeshCoinMagnet.transform.position;
			yield return null;
		}
		StopCoinMagnet();
	}

	private void StopCoinMagnet()
	{
		coinMagnetCollider.GetComponent<Collider>().enabled = false;
		isActive = false;
		base.enabled = false;
		characterRendering.CharacterModel.MeshCoinMagnet.enabled = false;
		coinEFX.localPosition = CharacterPickupParticles.coinEfxOffset;
		characterRendering.CharacterAnimation["hold_magnet"].enabled = false;
		SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.MagnetStop);
		if (base.Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void Update()
	{
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			bool flag = character.TransformPosition.y >= 70f;
			bool flag2 = component.transform.position.y >= 70f;
			if (component != null && flag == flag2)
			{
				component.MoveToMagnet(pullSpeed);
			}
		}
	}
}
