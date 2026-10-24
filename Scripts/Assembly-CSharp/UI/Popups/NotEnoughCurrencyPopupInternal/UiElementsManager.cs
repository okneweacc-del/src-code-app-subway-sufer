using System.Collections.Generic;
using UI.Screens.NotEnoughCurrencyPopupInternal;
using UnityEngine;

namespace UI.Popups.NotEnoughCurrencyPopupInternal;

public class UiElementsManager
{
	private Dictionary<UiElementType, GameObject> _uiElements;

	public UiElementsManager()
	{
		_uiElements = new Dictionary<UiElementType, GameObject>();
	}

	public void ActivateUiElement(UiElementType uiElementType, GameObject prefab, Transform parent, OnActivatedPrefab onActivatedPrefab = null)
	{
		bool wasSpawned = false;
		GameObject gameObject = null;
		if (!_uiElements.ContainsKey(uiElementType))
		{
			wasSpawned = true;
			gameObject = Object.Instantiate(prefab, parent);
			_uiElements.Add(uiElementType, gameObject);
		}
		else
		{
			gameObject = _uiElements[uiElementType];
			gameObject.SetActive(value: true);
		}
		onActivatedPrefab?.Invoke(gameObject, wasSpawned);
	}

	public void DeactivateAllUiElements()
	{
		foreach (KeyValuePair<UiElementType, GameObject> uiElement in _uiElements)
		{
			DeactivateUiElement(uiElement.Key);
		}
	}

	public void DeactivateUiElement(UiElementType uiElementType)
	{
		GameObject gameObject = _uiElements[uiElementType];
		if (gameObject != null)
		{
			gameObject.SetActive(value: false);
		}
	}

	public GameObject GetUIElement(UiElementType key)
	{
		return _uiElements[key];
	}
}
