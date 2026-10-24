using System;

public class CharacterModifierCollectionChangedEventArgs : EventArgs
{
	private CharacterModifierCollectionChangeType _characterModifierCollectionChangeType;

	private CharacterModifier _characterModifier;

	public CharacterModifierCollectionChangeType CharacterModifierCollectionChangeType => _characterModifierCollectionChangeType;

	public CharacterModifier CharacterModifier => _characterModifier;

	public CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType characterModifierCollectionChangeType, CharacterModifier characterModifier)
	{
		_characterModifierCollectionChangeType = characterModifierCollectionChangeType;
		_characterModifier = characterModifier;
	}
}
