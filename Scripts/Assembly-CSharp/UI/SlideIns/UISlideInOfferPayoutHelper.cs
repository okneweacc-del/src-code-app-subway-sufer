using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns;

public class UISlideInOfferPayoutHelper : UISlideIn
{
	[SerializeField]
	private UILabel _line1;

	[SerializeField]
	private UILabel _price;

	[SerializeField]
	private Layout _layout;

	[SerializeField]
	private Layout _line2Layout;

	public override void SetupSlideIn(SlideIn slideIn)
	{
		base.gameObject.SetActive(value: true);
		_line1.text = Strings.Get("SLIDE_IN_PAYOUT_RECEIVED");
		_price.text = slideIn.stringValue;
		_layout.Align();
		_line2Layout.Align();
		SlideIn();
	}
}
