using JetBrains.Annotations;
using TopRun;
using TopRun.UI;
using UI.Generic.Layout;
using UnityEngine;

public class TopRunOfflineStateHelper : MonoBehaviour
{
	public enum OfflineState
	{
		TryAgain = 0,
		Loading = 1
	}

	[SerializeField]
	private GameObject _tryAgainState;

	[SerializeField]
	private GameObject _loadingState;

	[SerializeField]
	private UILabel _tryAgainDescriptionLabel;

	[SerializeField]
	private UILabel _tryAgainButtonLabel;

	[SerializeField]
	private UILabel _loadingLabel;

	[SerializeField]
	private Layout _tryAgainLayout;

	[SerializeField]
	private Layout _loadingLayout;

	private TopRunOnlineScreen _topRunOnlineScreen;

	private GameOverTopRunContent _topRunOnlineGameOverScreen;

	public void SetState(OfflineState newState, TopRunOnlineScreen topRunScreen = null, GameOverTopRunContent topRunGOScreen = null)
	{
		_topRunOnlineScreen = topRunScreen;
		_topRunOnlineGameOverScreen = topRunGOScreen;
		switch (newState)
		{
		case OfflineState.TryAgain:
			SetTryAgainState();
			break;
		case OfflineState.Loading:
			SetLoadingState();
			break;
		default:
			SDebug.LogError(string.Concat("No state found for ", newState, "."));
			break;
		}
	}

	private void SetTryAgainState()
	{
		_loadingState.SetActive(value: false);
		_tryAgainState.SetActive(value: true);
		_tryAgainDescriptionLabel.text = Strings.Get("CONNECTION_PROBLEM");
		_tryAgainButtonLabel.text = Strings.Get("BUTTON_TRY_AGAIN");
		_tryAgainLayout.Align();
		base.gameObject.GetComponent<UIButtonOverlayOff>().enabled = true;
	}

	private void SetLoadingState()
	{
		_tryAgainState.SetActive(value: false);
		_loadingState.SetActive(value: true);
		_loadingLabel.text = Strings.Get("SOCIAL_BUTTON_FEEDBACK_CONNECTING");
		_loadingLayout.Align();
		base.gameObject.GetComponent<UIButtonOverlayOff>().enabled = false;
	}

	[UsedImplicitly]
	private void OnClick()
	{
		EnableLoadingOverlay();
		SetState(OfflineState.Loading);
		TopRunRoot.manager?.RefreshTournament();
	}

	private void EnableLoadingOverlay()
	{
		if (_topRunOnlineScreen != null)
		{
			_topRunOnlineScreen.loadingOverlay.SetActive(value: true);
		}
		else if (_topRunOnlineGameOverScreen != null)
		{
			_topRunOnlineGameOverScreen.loadingOverlay.SetActive(value: true);
		}
	}
}
