using UnityEngine;
using System.Collections;

public class ScreenColorOverlay : MonoBehaviour 
{
	#region Singleton

	private static ScreenColorOverlay instance = null;
	public static ScreenColorOverlay Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	#endregion

	private Texture2D fadeTexture = null;
	private GUIStyle fadeStyle = new GUIStyle();
	private Rect screenRect;

	private float fadeTimeLeft = 0.0f;

	private Color currentColor = Color.clear;
	private Color targetColor = Color.clear;
	private Color deltaColor = Color.clear;

	// Use this for initialization
	void Start() 
	{
		fadeTexture = new Texture2D(1, 1);
		fadeStyle.normal.background = fadeTexture;
		screenRect = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (currentColor != targetColor)
		{
			fadeTimeLeft -= Time.deltaTime;

			if (fadeTimeLeft <= 0.0f)
				SetColor(targetColor);
			else
				SetColor(currentColor + (deltaColor * Time.deltaTime));
		}
	}

	void OnGUI()
	{
		if (currentColor.a > 0.0f)
		{
			GUI.depth = -1;
			GUI.Label(screenRect, fadeTexture, fadeStyle);
		}
	}

	public void FadeToColor(Color targetColor, float fadeTime)
	{
		this.targetColor = targetColor;

		if (fadeTime <= 0.0f)
			SetColor(targetColor);
		else
		{
			fadeTimeLeft = fadeTime;
			deltaColor = (targetColor - currentColor) / fadeTime;
		}
	}

	private void SetColor(Color color)
	{
		currentColor = color;
		fadeTexture.SetPixel(0, 0, currentColor);
		fadeTexture.Apply();

		if (currentColor == targetColor)
			SendMessage("OnScreenColorOverlayFadeComplete", currentColor, SendMessageOptions.DontRequireReceiver);
	}
}
