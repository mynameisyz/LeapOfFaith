using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
	public Texture backgroundTex;
	public GUIStyle resumeButtonStyle;
	public GUIStyle helpButtonStyle;
	public GUIStyle quitButtonStyle;

	private Rect backgroundRect;
	private Rect resumeButtonRect;
	private Rect helpButtonRect;
	private Rect quitButtonRect;

	private const float PAUSEVALUE_TIME = 0.25f;

	public static bool isPaused = false;
	private float pauseValue = 0.0f;
	private float pauseSpeed = 0.0f;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);
	
	private string lastTooltip;
	private float hoverTimer = 0.0f;

	// Use this for initialization
	void Start() 
	{
		pauseSpeed = 1.0f / PAUSEVALUE_TIME;

		screenRect = AspectUtility.screenRect;

		backgroundRect = new Rect((screenRect.width / 2) - (screenRect.width / INTENDED_RES.width * 282.0f),
									(screenRect.height / 2) - (screenRect.height / INTENDED_RES.height * 315.0f),
									screenRect.width / INTENDED_RES.width * 564.0f,
									screenRect.height / INTENDED_RES.height * 630.0f);

		resumeButtonRect = new Rect((screenRect.width / 2) - (screenRect.width / INTENDED_RES.width * 167.5f),
									(screenRect.height / 2) - (screenRect.height / INTENDED_RES.height * 197.5f),
									screenRect.width / INTENDED_RES.width * 335.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		helpButtonRect = new Rect((screenRect.width / 2) - (screenRect.width / INTENDED_RES.width * 167.5f),
									(screenRect.height / 2) - (screenRect.height / INTENDED_RES.height * 39.5f),
									screenRect.width / INTENDED_RES.width * 335.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		quitButtonRect = new Rect((screenRect.width / 2) - (screenRect.width / INTENDED_RES.width * 167.5f),
									(screenRect.height / 2) + (screenRect.height / INTENDED_RES.height * 118.5f),
									screenRect.width / INTENDED_RES.width * 335.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);
		
		InputManager.kinectActive = LocalData.isKinectEnabled;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (Application.loadedLevel != (int)LevelManager.Scene.LoadingScreen &&
			InputManager.pause)
		{
			isPaused = !isPaused;
		}

		if (isPaused && pauseValue < 1.0f)
		{
			pauseValue += pauseSpeed * Time.deltaTime;
			if (pauseValue > 1.0f)
			{
				pauseValue = 1.0f;
			}
		}
		else if (!isPaused && pauseValue > 0.0f)
		{
			pauseValue -= pauseSpeed * Time.deltaTime;
			if (pauseValue <= 0.0f)
			{
				pauseValue = 0.0f;
				if(InputManager.kinectActive)
				{
					InputManager.cursorActive = false;
				}
			}
		}
		
		if(isPaused && pauseValue == 1.0f && InputManager.kinectActive)
		{
			InputManager.cursorActive = true;
			InputManager.autoClickOnce = true;
		}
		/*
		else if(!isPaused && pauseValue < 1.0f && InputManager.kinectActive && Application.loadedLevel >= 4 && !this.GetComponent<HelpMenu>().GetIsSowingHelp())
			InputManager.cursorActive = false;
		*/
	}
	
	void OnGUI() 
	{
		if (Application.loadedLevel != (int)LevelManager.Scene.LoadingScreen)
		{
			if (pauseValue > 0.0f)
			{
				GUI.DrawTexture(RectMultiply(backgroundRect, pauseValue), backgroundTex);

				if (GUI.Button(RectMultiply(resumeButtonRect, pauseValue), new GUIContent(string.Empty, "resume"), resumeButtonStyle))
				{
					isPaused = false;
				}

				if (Application.loadedLevel < (int)LevelManager.Scene.Level_1)
					GUI.enabled = false;

				if (GUI.Button(RectMultiply(helpButtonRect, pauseValue), new GUIContent(string.Empty, "help"), helpButtonStyle))
				{
					isPaused = false;
					this.GetComponent<HelpMenu>().ShowHelp();
				}

				GUI.enabled = true;

				if (GUI.Button(RectMultiply(quitButtonRect, pauseValue), new GUIContent(string.Empty, "quit"), quitButtonStyle))
				{
					DisconnectionHandler.isForcedDisconnect = true;
					Network.Disconnect();
					isPaused = false;
				}
			}
			
			if(LocalData.isKinectEnabled && InputManager.kinectActive)
			{
				if (GUI.tooltip != lastTooltip)
				{
 					if (GUI.tooltip != "")
					{
						hoverTimer += Time.deltaTime;
						if(hoverTimer >= 2.0f)
						{
							InputManager.clickOnce();
							hoverTimer = 0.0f;
						}
					}
       		     lastTooltip = GUI.tooltip;
      			}		
				else
				{
					hoverTimer = 0.0f;	
				}
				InputManager.setCursorFill(hoverTimer / 2.0f);
			}
		}
	}

	private Rect RectMultiply(Rect rect, float multiplier)
	{
		if (multiplier != 1.0f)
		{
			rect.x *= multiplier;
			rect.y *= multiplier;
			rect.width *= multiplier;
			rect.height *= multiplier;
		}

		rect.x += screenRect.x;
		rect.y += screenRect.y;

		return rect;
	}
}
