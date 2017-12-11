using UnityEngine;
using System.Collections;

public class HelpMenu : MonoBehaviour
{
	public GUIStyle resumeButtonStyle;
	public GUIStyle leftButtonStyle;
	public GUIStyle rightButtonStyle;

	private Rect resumeButtonRect;
	private Rect leftButtonRect;
	private Rect rightButtonRect;

	public GameObject loadingBackgroundObj;

	public Texture[] helpScreenTex;
	public Texture[] kinectHelpScreenTex;

	private Texture[] scrollableTex;
	private bool isScrollable = false;
	private int scrollableIndexTotal = 0;
	private int currentPage = 0;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);

	public static bool isShowingHelp = false;
	
	private string lastTooltip;
	private float hoverTimer = 0.0f;
	
	public AudioClip select_sound;
	public AudioClip next_sound;
	
	// Use this for initialization
	void Start()
	{
		if (LocalData.isKinectEnabled)
			scrollableTex = kinectHelpScreenTex;
		else
			scrollableTex = helpScreenTex;

		currentPage = 0;
		isScrollable = true;
		scrollableIndexTotal = scrollableTex.Length - 1;
		loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];

		screenRect = AspectUtility.screenRect;
		loadingBackgroundObj.guiTexture.pixelInset = new Rect(-screenRect.width / 2, -screenRect.height / 2, screenRect.width, screenRect.height);

		resumeButtonRect = new Rect((screenRect.x + (screenRect.width / 2)) - (screenRect.width / INTENDED_RES.width * 167.5f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 124.0f),
									screenRect.width / INTENDED_RES.width * 335.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		leftButtonRect = new Rect((screenRect.x + (screenRect.width / 2)) - (screenRect.width / INTENDED_RES.width * 335.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 124.0f),
									screenRect.width / INTENDED_RES.width * 100.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		rightButtonRect = new Rect((screenRect.x + (screenRect.width / 2)) + (screenRect.width / INTENDED_RES.width * 235.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 124.0f),
									screenRect.width / INTENDED_RES.width * 100.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);
		
		if(LocalData.isKinectEnabled)
		{
			InputManager.kinectActive = true;
		}
		else
		{
			InputManager.kinectActive = false;
		}
		
	}

	// Update is called once per frame
	void Update()
	{
		if (isShowingHelp &&
			Input.GetKeyDown(KeyCode.Escape))
		{
			isShowingHelp = false;
			loadingBackgroundObj.guiTexture.enabled = false;
		}
	}

	public void ShowHelp()
	{
		isShowingHelp = true;
		currentPage = 0;
		loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
		loadingBackgroundObj.guiTexture.enabled = true;
		InputManager.cursorActive = true;
	}

	void OnGUI()
	{
		if (isShowingHelp)
		{
			if (GUI.Button(resumeButtonRect, new GUIContent(string.Empty, "resume"), resumeButtonStyle))
			{
				isShowingHelp = false;
				loadingBackgroundObj.guiTexture.enabled = false;
				audio.PlayOneShot(select_sound);
			}

			if (isScrollable)
			{
				if (currentPage <= 0)
					GUI.enabled = false;
				if (GUI.Button(leftButtonRect, new GUIContent(string.Empty, "left"), leftButtonStyle))
				{
					currentPage--;
					loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
					audio.PlayOneShot(next_sound);
				}
				GUI.enabled = true;

				if (currentPage >= scrollableIndexTotal)
					GUI.enabled = false;
				if (GUI.Button(rightButtonRect, new GUIContent(string.Empty, "right"), rightButtonStyle))
				{
					currentPage++;
					loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
					audio.PlayOneShot(next_sound);
				}
				GUI.enabled = true;
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
	
	public bool GetIsSowingHelp()
	{
		return isShowingHelp;
	}
}
