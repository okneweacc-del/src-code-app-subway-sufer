using NaughtyAttributes;
using UnityEngine;

namespace UI;

public class ColorData : ScriptableObject
{
	private static ColorData _instance;

	[BoxGroup("Blue Vibrant")]
	public Color cloud;

	[BoxGroup("Blue Vibrant")]
	public Color sky;

	[BoxGroup("Blue Vibrant")]
	public Color smurf;

	[BoxGroup("Blue Vibrant")]
	public Color push;

	[BoxGroup("Blue Vibrant")]
	public Color boston;

	[BoxGroup("Blue Vibrant")]
	public Color peacock;

	[BoxGroup("Blue Vibrant")]
	public Color ocean;

	[BoxGroup("Blue Vibrant")]
	public Color royale;

	[BoxGroup("Blue Vibrant")]
	public Color midnight;

	[BoxGroup("Teal Vibrant")]
	public Color scooter;

	[BoxGroup("Teal Vibrant")]
	public Color bulbasaur;

	[BoxGroup("Teal Vibrant")]
	public Color turquoise;

	[BoxGroup("Teal Vibrant")]
	public Color teal;

	[BoxGroup("Teal Vibrant")]
	public Color pit;

	[BoxGroup("Green Vibrant")]
	public Color grape;

	[BoxGroup("Green Vibrant")]
	public Color lime;

	[BoxGroup("Green Vibrant")]
	public Color grass;

	[BoxGroup("Green Vibrant")]
	public Color avocado;

	[BoxGroup("Yellow Vibrant")]
	public Color glow;

	[BoxGroup("Yellow Vibrant")]
	public Color sun;

	[BoxGroup("Yellow Vibrant")]
	public Color yolk;

	[BoxGroup("Yellow Vibrant")]
	public Color orange;

	[BoxGroup("Yellow Vibrant")]
	public Color dutch;

	[BoxGroup("Yellow Vibrant")]
	public Color fox;

	[BoxGroup("Yellow Vibrant")]
	public Color ruby;

	[BoxGroup("Blue Dusty")]
	public Color pigeon;

	[BoxGroup("Blue Dusty")]
	public Color tuna;

	[BoxGroup("Blue Dusty")]
	public Color storm;

	[BoxGroup("Blue Dusty")]
	public Color oxford;

	[BoxGroup("Green Dusty")]
	public Color mint;

	[BoxGroup("Green Dusty")]
	public Color spring;

	[BoxGroup("Green Dusty")]
	public Color christi;

	[BoxGroup("Green Dusty")]
	public Color verdun;

	[BoxGroup("Yellow Dusty")]
	public Color sand;

	[BoxGroup("Yellow Dusty")]
	public Color desert;

	[BoxGroup("Red Dusty")]
	public Color pink;

	[BoxGroup("Red Dusty")]
	public Color peach;

	[BoxGroup("Red Dusty")]
	public Color safran;

	[BoxGroup("Red Dusty")]
	public Color crimson;

	[BoxGroup("Red Dusty")]
	public Color mahogany;

	[BoxGroup("Red Dusty")]
	public Color bordeaux;

	[BoxGroup("Grayscale")]
	public Color white;

	[BoxGroup("Grayscale")]
	public Color stroking;

	[BoxGroup("Grayscale")]
	public Color chrome;

	[BoxGroup("Grayscale")]
	public Color slate;

	[BoxGroup("Grayscale")]
	public Color iron;

	[BoxGroup("Grayscale")]
	public Color rock;

	[BoxGroup("Grayscale")]
	public Color smoke;

	[BoxGroup("Grayscale")]
	public Color blueBlack;

	[BoxGroup("Grayscale")]
	public Color shadow;

	[BoxGroup("Purple Dusty")]
	public Color sliggo;

	[BoxGroup("Purple Dusty")]
	public Color lavender;

	[BoxGroup("Purple Dusty")]
	public Color amethyst;

	[Header("BG Effects")]
	public Color stripeColor;

	public Color glowColor;

	public Color starParticlesColor;

	[Header("Word hunt Letters specific")]
	[ColorUsage(false)]
	[SerializeField]
	public Color dailyLetterColorActiveShadow;

	[ColorUsage(false)]
	[SerializeField]
	public Color dailyLetterColorInactive;

	[ColorUsage(false)]
	[SerializeField]
	public Color dailyLetterColorInactiveShadow;

	[Header("Word Hunt")]
	public Color wordHuntCurrentDayGradientTop;

	public Color wordHuntCurrentDayGradientBottom;

	public Color wordHuntBubbleColor;

	public Color wordHuntBubbleShadowColor;

	[Header("My Tour footer")]
	public Color myTourFooterColorMain;

	public Color myTourFooterColorLight;

	public Color myTourFooterColorSelected;

	public static ColorData instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<ColorData>("Info/ColorData");
			}
			return _instance;
		}
	}

	[Button(null)]
	private void Save()
	{
	}
}
