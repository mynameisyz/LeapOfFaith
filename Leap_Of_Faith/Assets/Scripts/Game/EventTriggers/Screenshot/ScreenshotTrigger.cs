using UnityEngine;
using System.Collections;
using System.IO;

public class ScreenshotTrigger : MonoBehaviour 
{
	public GameObject screenshotButton;
	public int forPlayerIndex = -1;

	public float stepSinkMax = 0.0f;
	public float stepSinkSpeed = 0.0f;
	public float stepFloatSpeed = 0.0f;
	private float stepFloatMax = 0.0f;

	private bool isSteppedOn = false;
	private bool isToggled = false;

	private int screenshotIndex = 0;
	private bool isProcessingScreenshot = false;
	public static bool isTakingScreenshot = false;

	void Awake()
	{
		switch (forPlayerIndex)
		{
			case PlayerData.PLAYER_RED:
				screenshotButton.renderer.material.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
				break;

			case PlayerData.PLAYER_BLUE:
				screenshotButton.renderer.material.color = new Color(0.0f, 0.5f, 1.0f, 1.0f);
				break;

			default:
				this.enabled = false;
				break;
		}
	}

	// Use this for initialization
	void Start() 
	{
		stepFloatMax = screenshotButton.transform.position.y;
		stepSinkMax = screenshotButton.transform.position.y - stepSinkMax;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (isProcessingScreenshot)
		{
			if (File.Exists(Application.dataPath + @"/Screenshots/Screenshot_" + screenshotIndex + ".png"))
			{
				isProcessingScreenshot = false;
				ScreenColorOverlay.Instance.FadeToColor(Color.white, 0.1f);
			}
		}

		if (isSteppedOn)
		{
			if (screenshotButton.transform.position.y > stepSinkMax)
			{
				screenshotButton.transform.Translate(0.0f, -stepSinkSpeed * Time.deltaTime, 0.0f);
				if (screenshotButton.transform.position.y <= stepSinkMax)
				{
					isToggled = true;

					bool isToScreenshot = true;
					ScreenshotTrigger[] otherTriggers = (ScreenshotTrigger[])FindObjectsOfType(typeof(ScreenshotTrigger));
					foreach (ScreenshotTrigger trigger in otherTriggers)
					{
						if (!trigger.isToggled)
						{
							isToScreenshot = false;
							break;
						}
					}

					if (isToScreenshot)
					{
						this.isToggled = false;
						foreach (ScreenshotTrigger trigger in otherTriggers)
							trigger.isToggled = false;

						TakeScreenshot();
					}
				}
			}
		}
		else
		{
			if (screenshotButton.transform.position.y < stepFloatMax)
			{
				screenshotButton.transform.Translate(0.0f, stepFloatSpeed * Time.deltaTime, 0.0f);
				if (screenshotButton.transform.position.y > stepFloatMax)
					screenshotButton.transform.Translate(0.0f, stepFloatMax - screenshotButton.transform.position.y, 0.0f);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (isSteppedOn == false &&
			other.gameObject == PlayerData.characters[forPlayerIndex])
			isSteppedOn = true;
	}

	void OnTriggerExit(Collider other)
	{
		if (isSteppedOn == true &&
			other.gameObject == PlayerData.characters[forPlayerIndex])
		{
			isSteppedOn = false;
			isToggled = false;
		}
	}

	private void TakeScreenshot()
	{
		if (!Directory.Exists(Application.dataPath + @"/Screenshots/"))
			Directory.CreateDirectory(Application.dataPath + @"/Screenshots/");

		screenshotIndex++;

		Application.CaptureScreenshot(Application.dataPath + @"/Screenshots/Screenshot_" + screenshotIndex + ".png");
		isTakingScreenshot = true;
		isProcessingScreenshot = true;
	}
}
