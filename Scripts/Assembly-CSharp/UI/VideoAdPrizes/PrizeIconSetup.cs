using System.Collections.Generic;
using UnityEngine;

namespace UI.VideoAdPrizes;

public abstract class PrizeIconSetup
{
	private const string ICON_NAME_MYSTERY_BOX = "icon_MysteryBox_big";

	private const string ICON_NAME_MINI_MYSTERY_BOX = "icon_big_mini_mysteryBox";

	private const string ICON_NAME_SUPER_MYSTERY_BOX = "icon_bigSuperMysteryBox";

	private const string ICON_NAME_COINS = "Icon_coin_pouch_larger";

	private const string ICON_NAME_KEY = "icon_key_cloud";

	[SerializeField]
	protected VideoAdPrizeType _prizeType;

	[SerializeField]
	protected Vector3 _iconPosition;

	[SerializeField]
	protected Vector3 _iconRotation;

	[SerializeField]
	protected int _iconHeight = 52;

	public VideoAdPrizeType prizeType => _prizeType;

	public abstract UISprite iconSprite { get; set; }

	public void Setup()
	{
		SetupIconSprite();
		OnSetup();
	}

	public void Deactivate()
	{
		DeactivateSetupIconSprite();
		OnDeactivate();
	}

	private void SetupIconSprite()
	{
		if (!(iconSprite == null))
		{
			iconSprite.enabled = true;
			switch (_prizeType)
			{
			case VideoAdPrizeType.SuperMysteryBox:
				iconSprite.spriteName = "icon_bigSuperMysteryBox";
				break;
			case VideoAdPrizeType.MysteryBox:
				iconSprite.spriteName = "icon_MysteryBox_big";
				break;
			case VideoAdPrizeType.MiniMysteryBox:
				iconSprite.spriteName = "icon_big_mini_mysteryBox";
				break;
			case VideoAdPrizeType.Keys:
			case VideoAdPrizeType.DailyVideoPrize:
				iconSprite.spriteName = "icon_key_cloud";
				break;
			case VideoAdPrizeType.Coins:
				iconSprite.spriteName = "Icon_coin_pouch_larger";
				break;
			default:
				Debug.LogError("PrizeIconSetup: Could not associate VideoAdPrizeType with icon name.");
				break;
			}
			iconSprite.transform.localPosition = _iconPosition;
			iconSprite.transform.localEulerAngles = _iconRotation;
			iconSprite.MakePixelPerfect();
			iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
			iconSprite.height = _iconHeight;
		}
	}

	private void DeactivateSetupIconSprite()
	{
		if (iconSprite != null)
		{
			iconSprite.enabled = false;
			iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		}
	}

	protected virtual void OnSetup()
	{
	}

	protected virtual void OnDeactivate()
	{
	}

	public static List<T> GetPrizeIconSetupsWithTypeAndDeactivateTheRest<T>(IEnumerable<T> prizeIconSetups, VideoAdPrizeType prizeType) where T : PrizeIconSetup
	{
		List<T> list = new List<T>();
		foreach (T prizeIconSetup in prizeIconSetups)
		{
			T current = prizeIconSetup;
			if (current.prizeType == prizeType)
			{
				list.Add(current);
			}
			else
			{
				current.Deactivate();
			}
		}
		return list;
	}
}
