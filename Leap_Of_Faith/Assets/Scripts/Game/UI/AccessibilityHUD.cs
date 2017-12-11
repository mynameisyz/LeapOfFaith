using UnityEngine;
using System.Collections;

public class AccessibilityHUD : MonoBehaviour
{
	public GUIStyle pauseButtonStyle;
	public GUIStyle muteButtonStyle;
	public GUIStyle unmuteButtonStyle;

	private Rect pauseButtonRect;
	private Rect muteButtonRect;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);

	// Use this for initialization
	void Start() 
	{
		screenRect = AspectUtility.screenRect;

		pauseButtonRect = new Rect(screenRect.x + (screenRect.width / INTENDED_RES.width * 10.0f),
									screenRect.y + (screenRect.height / INTENDED_RES.height * 10.0f),
									screenRect.width / INTENDED_RES.width * 100.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		muteButtonRect = new Rect(screenRect.x + screenRect.width - (screenRect.width / INTENDED_RES.width * 110.0f),
									screenRect.y + (screenRect.height / INTENDED_RES.height * 10.0f),
									screenRect.width / INTENDED_RES.width * 100.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnGUI()
	{
		if (Application.loadedLevel != (int)LevelManager.Scene.LoadingScreen &&
			!HelpMenu.isShowingHelp)
		{
			if (!LocalData.isKinectEnabled && !PauseMenu.isPaused)
			{
				if (GUI.Button(pauseButtonRect, string.Empty, pauseButtonStyle))
				{
					PauseMenu.isPaused = true;
				}
			}

			if (!LocalData.isKinectEnabled || PauseMenu.isPaused)
			{
				if (GUI.Button(muteButtonRect, string.Empty, AudioListener.pause ? unmuteButtonStyle : muteButtonStyle))
				{
					AudioListener.pause = !AudioListener.pause;
				}
			}
		}
	}
}
