using Data.Managers;
using Info;

public class BoardScreenManager
{
	private static BoardScreenManager _instance;

	public static BoardScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BoardScreenManager();
			}
			return _instance;
		}
	}

	public bool forceRefreshOnBoardScreen { get; set; }

	public bool forceCenterOnSpecificBoard { get; set; }

	public bool willResetBoardAnimation { get; set; }

	public bool shouldResetThemeSelection { get; set; }

	public bool refreshBoardsWhenNotShowingPopup { get; set; }

	public Boards.BoardId currentlyShownBoardId { get; set; }

	public int currentlyShownThemeIndex { get; set; }

	private BoardScreenManager()
	{
	}

	public void ChangedDisplayedHoverboard(Boards.BoardId newBoard)
	{
		currentlyShownBoardId = newBoard;
		BoardManager.ResetDisplayedCustomSets(newBoard);
	}
}
