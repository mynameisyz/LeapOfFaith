using UnityEngine;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour 
{
	public GUIStyle readyButtonStyle;
	public Texture waitingTex;
	public GUIStyle leftButtonStyle;
	public GUIStyle rightButtonStyle;

	private Rect readyButtonRect;
	private Rect waitingTexRect;
	private Rect leftButtonRect;
	private Rect rightButtonRect;

	public GameObject loadingBackgroundObj;
	public Texture[] helpScreenTex;
	public Texture[] kinectHelpScreenTex;
	public Texture[] charactersScreenTex;

	public GameObject crabObject;

	private Texture[] scrollableTex;
	private bool isScrollable = false;
	private int scrollableIndexTotal = 0;
	private int currentPage = 0;

	private bool isReady = false;
	private int playersReady = 0;

	public float scrollableTimeout = 0.0f;
	public float loadScreenTimeout = 0.0f;

	private float loadingScreenTimeout = 0.0f;
	public static int nextLevel = 0;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);
	
	private string lastTooltip;
	private float hoverTimer = 0.0f;
	
	public AudioClip sound_next;
	public AudioClip sound_select;
	
	// Use this for initialization
	void Start() 
	{
		if (nextLevel == (int)LevelManager.Scene.Level_1)
		{
			if (LocalData.isKinectEnabled)
				scrollableTex = kinectHelpScreenTex;
			else
				scrollableTex = helpScreenTex;

			currentPage = 0;
			isScrollable = true;
			scrollableIndexTotal = scrollableTex.Length - 1;
			loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
			loadingScreenTimeout = scrollableTimeout;
			Destroy(crabObject);
		}
		else
		{
			loadingScreenTimeout = loadScreenTimeout;
			loadingBackgroundObj.guiTexture.texture = charactersScreenTex[PlayerData.classId[PlayerData.color]];
		}
		
		screenRect = AspectUtility.screenRect;
		loadingBackgroundObj.guiTexture.pixelInset = new Rect(-screenRect.width / 2, -screenRect.height / 2, screenRect.width, screenRect.height);
		loadingBackgroundObj.guiTexture.enabled = true;

		readyButtonRect = new Rect((screenRect.x + (screenRect.width / 2)) - (screenRect.width / INTENDED_RES.width * 167.5f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 124.0f),
									screenRect.width / INTENDED_RES.width * 335.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		waitingTexRect = new Rect((screenRect.x + (screenRect.width / 2)) - (screenRect.width / INTENDED_RES.width * 235.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 103.5f),
									screenRect.width / INTENDED_RES.width * 470.0f,
									screenRect.height / INTENDED_RES.height * 37.0f);

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
			InputManager.cursorActive = true;
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (!isReady)
		{
			//loadingScreenTimeout -= Time.deltaTime;
			if (loadingScreenTimeout <= 0.0f)
			{
				networkView.RPC("Peer_IsReady", RPCMode.All);
				isReady = true;
			}
		}
	}

	void OnGUI()
	{
		if (isReady)
		{
			GUI.enabled = false;
			GUI.DrawTexture(waitingTexRect, waitingTex);
			GUI.enabled = true;
		}
		else if (GUI.Button(readyButtonRect, new GUIContent(string.Empty, "ready"), readyButtonStyle))
		{
			audio.PlayOneShot(sound_select);
			networkView.RPC("Peer_IsReady", RPCMode.All);
			isReady = true;
		}

		if (isScrollable)
		{
			if (currentPage <= 0)
				GUI.enabled = false;
			if (GUI.Button(leftButtonRect, new GUIContent(string.Empty, "left"), leftButtonStyle))
			{
				audio.PlayOneShot(sound_next);
				currentPage--;
				loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
			}
			GUI.enabled = true;

			if (currentPage >= scrollableIndexTotal)
				GUI.enabled = false;
			if (GUI.Button(rightButtonRect, new GUIContent(string.Empty, "right"), rightButtonStyle))
			{
				audio.PlayOneShot(sound_next);
				currentPage++;
				loadingBackgroundObj.guiTexture.texture = scrollableTex[currentPage];
			}
			GUI.enabled = true;
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

	[RPC]
	private void Peer_IsReady()
	{
		playersReady++;

		if (playersReady >= 2)
			LevelManager.Instance.RPC_LoadLevel(nextLevel, 0.5f, 1.0f);
	}
}
