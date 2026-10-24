using System;

public class PromoCodeEventArgs : EventArgs
{
	private string _code;

	private int _numCharacters;

	private int _numBoards;

	private int _numKeys;

	private int _numCoins;

	public string code => _code;

	public int numCharacters => _numCharacters;

	public int numBoards => _numBoards;

	public int numKeys => _numKeys;

	public int numCoins => _numCoins;

	public PromoCodeEventArgs(string code, int nCharacters, int nBoards, int nKeys, int nCoins)
	{
		_code = code;
		_numCharacters = nCharacters;
		_numBoards = nBoards;
		_numKeys = nKeys;
		_numCoins = nCoins;
	}

	public override string ToString()
	{
		return $"[PromoCodeEventArgs: l-code={_code} 2-numCharacters={_numCharacters} 3-numBoards={_numBoards} 4-numKeys={_numKeys} 5-numCoins={_numCoins}";
	}
}
