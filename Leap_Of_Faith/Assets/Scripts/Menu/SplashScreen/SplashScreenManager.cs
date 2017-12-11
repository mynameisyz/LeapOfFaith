using UnityEngine;
using System.Collections;

public class SplashScreenManager : MonoBehaviour 
{
	public float splashTimeout = 0.0f;

	// Use this for initialization
	void Start() 
	{
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (splashTimeout > 0.0f)
		{
			splashTimeout -= Time.deltaTime;
			if (splashTimeout <= 0.0f ||
				Input.anyKeyDown)
			{
				SkipSplashScreen();
				splashTimeout = 0.0f;
			}
		}
	}

	private void SkipSplashScreen()
	{
		ScreenColorOverlay.Instance.FadeToColor(Color.black, 1.0f);
	}

	private void OnScreenColorOverlayFadeComplete(Color overlayColor)
	{
		if (overlayColor == Color.black)
		{
			Application.LoadLevel((int)LevelManager.Scene.MainMenu);
			ScreenColorOverlay.Instance.FadeToColor(Color.clear, 1.0f);
		}
		else if (overlayColor == Color.clear)
		{
			Destroy(this.gameObject);
		}
	}
}
