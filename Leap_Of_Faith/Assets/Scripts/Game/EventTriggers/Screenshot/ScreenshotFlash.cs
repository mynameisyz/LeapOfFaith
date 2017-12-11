using UnityEngine;
using System.Collections;

public class ScreenshotFlash : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
	{	
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	private void OnScreenColorOverlayFadeComplete(Color overlayColor)
	{
		if (ScreenshotTrigger.isTakingScreenshot &&
			overlayColor == Color.white)
		{
			ScreenshotTrigger.isTakingScreenshot = false;
			ScreenColorOverlay.Instance.FadeToColor(Color.clear, 0.3f);
		}
	}
}
