using System.Collections.Generic;
using Data.Managers;
using Kiloo.Common.UI;
using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups;

public class CharacterOwnagePopup : UIPopup
{
	private class CharacterOwnageListAdapter : ListAdapter<KeyValuePair<Characters.CharacterId, Characters.Model>>
	{
		private GameObject _cellPrefab;

		public CharacterOwnageListAdapter(List<KeyValuePair<Characters.CharacterId, Characters.Model>> items, GameObject cell)
			: base(items)
		{
			_cellPrefab = cell;
		}

		public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
		{
			Container container2 = container;
			KeyValuePair<Characters.CharacterId, Characters.Model> item = GetItem(dataIndex);
			if (container2 == null)
			{
				container2 = new Container(_cellPrefab);
				CharacterOwnageDebugCellHelper component = container2.model.GetComponent<CharacterOwnageDebugCellHelper>();
				container2.tag = component;
			}
			if (!isMeasurePass)
			{
				CharacterOwnageDebugCellHelper characterOwnageDebugCellHelper = (CharacterOwnageDebugCellHelper)container2.tag;
				characterOwnageDebugCellHelper.Init(item);
			}
			return container2;
		}
	}

	[SerializeField]
	private GameObject _cellPrefab;

	[SerializeField]
	private ScrollView _scrollView;

	private CharacterOwnageListAdapter _adapter;

	private bool _showAllCharacters;

	[SerializeField]
	private GameObject _offIcon;

	[SerializeField]
	private GameObject _onIcon;

	private List<KeyValuePair<Characters.CharacterId, Characters.Model>> _list;

	protected override float screenOffset => -13f;

	protected override void Init()
	{
		_list = new List<KeyValuePair<Characters.CharacterId, Characters.Model>>();
		_offIcon.SetActive(_showAllCharacters);
		_onIcon.SetActive(!_showAllCharacters);
	}

	protected override void Show()
	{
		_list.Clear();
		foreach (KeyValuePair<Characters.CharacterId, Characters.Model> characterDatum in Characters.characterData)
		{
			if (_showAllCharacters || CharacterManager.IsCharacterOwned(characterDatum.Key))
			{
				_list.Add(characterDatum);
			}
		}
		_adapter = new CharacterOwnageListAdapter(_list, _cellPrefab);
		_scrollView.SetAdapter(_adapter);
	}

	public void ToggleActive()
	{
		_showAllCharacters = !_showAllCharacters;
		_offIcon.SetActive(_showAllCharacters);
		_onIcon.SetActive(!_showAllCharacters);
		Show();
	}
}
