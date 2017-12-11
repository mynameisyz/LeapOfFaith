using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour
{
	public GUIStyle leftButtonStyle;
	public GUIStyle rightButtonStyle;
	public GUIStyle selectButtonStyle;

	private Rect leftButtonRect;
	private Rect rightButtonRect;
	private Rect selectButtonRect;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);

	private Ray ray;
	private RaycastHit hit;
	private GameObject thisObject;					// temp holder for object
	public GameObject[] characters;					// Store all characters in scene
	public Transform[] arrows;
	public Transform selectionWheel;

	private int playerSelectionCount = 0;
	private int currentSelection = 0;
	private int otherPlayerSelection = 0;
	private Vector3 intendedArrowPos = Vector3.zero;

	private bool isSelectionDone = false;

	private Color guiColor = Color.clear;
	private static float guiFadeInTimeLeft = 0.0f;
	private static float guiFadeInTimeTotal = 0.0f;
	
	private string hover;
	private float hoverTimer;
	private string lastTooltip;
	
	public AudioClip buttonNext;
	public AudioClip buttonSelect;
	
	// Use this for initialization
	void Start()
	{
		currentSelection = PlayerData.color;
		otherPlayerSelection = PlayerData.peerColor;

		arrows[PlayerData.PLAYER_RED].position = characters[0].transform.position + new Vector3(0f, 1.3f, 0f);
		arrows[PlayerData.PLAYER_RED].renderer.enabled = true;

		arrows[PlayerData.PLAYER_BLUE].position = characters[0].transform.position + new Vector3(0f, 1.3f, 0f);
		arrows[PlayerData.PLAYER_BLUE].renderer.enabled = true;

		arrows[PlayerData.peerColor].GetComponent<TranslationYCycle>().enabled = false;
		
		SetActiveSelection(currentSelection);

		screenRect = AspectUtility.screenRect;

		selectButtonRect = new Rect((screenRect.x + (screenRect.width / 2)) - (screenRect.width / INTENDED_RES.width * 167.5f),
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

		guiColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		hoverTimer = 0.0f;
		
		if(LocalData.isKinectEnabled)
		{
			InputManager.kinectActive = true;
			InputManager.InitKinect();
			InputManager.cursorActive = true;
			Debug.Log("fdsfs");
		}
		else
		{
			InputManager.kinectActive = false;
		}
	}

	public static void StartGUIFadeIn(float _time)
	{
		guiFadeInTimeTotal = _time;
		guiFadeInTimeLeft = guiFadeInTimeTotal;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateSelection(currentSelection);

		if (intendedArrowPos != characters[otherPlayerSelection].transform.position)
		{
			intendedArrowPos = characters[otherPlayerSelection].transform.position;
			arrows[PlayerData.peerColor].GetComponent<SmoothTranslation>().MoveTo(intendedArrowPos, 0.3f);
		}

		if (guiFadeInTimeLeft > 0.0f)
		{
			guiFadeInTimeLeft -= Time.deltaTime;
			if (guiFadeInTimeLeft < 0.0f)
				guiFadeInTimeLeft = 0.0f;

			guiColor.a = (guiFadeInTimeTotal - guiFadeInTimeLeft) / guiFadeInTimeTotal;
		}
		
		
	}
	
	void LateUpdate()
	{
		arrows[PlayerData.peerColor].position = new Vector3(arrows[PlayerData.peerColor].position.x,
															arrows[PlayerData.color].position.y,
															arrows[PlayerData.peerColor].position.z);
	}

	void OnGUI()
	{
		GUI.color = guiColor;

		if (isSelectionDone)
		{
			GUI.enabled = false;
		}

		if (GUI.Button(leftButtonRect, new GUIContent(string.Empty, "left"), leftButtonStyle))
		{
			audio.PlayOneShot(buttonNext);
			ResetActiveSelection(currentSelection);
			int tempIndex = currentSelection + 1;
			if (tempIndex >= 4)
			{
				tempIndex = 0;
			}

			if (characters[tempIndex].GetComponent<SelectionData>().selectedPlayer
				== PlayerData.peerColor)
			{
				selectionWheel.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, ((tempIndex - 1) * 90), 0), Quaternion.Euler(0, (tempIndex * 90), 0), Time.deltaTime * 5.0f);
				tempIndex++;
				if (tempIndex >= 4)
				{
					tempIndex = 0;
				}
			}

			currentSelection = tempIndex;
			SetActiveSelection(currentSelection);
		}

		if (GUI.Button(rightButtonRect, new GUIContent(string.Empty, "right"), rightButtonStyle))
		{
			audio.PlayOneShot(buttonNext);
			ResetActiveSelection(currentSelection);
			int tempIndex = currentSelection - 1;
			if (tempIndex <= -1)
			{
				tempIndex = 3;
			}

			if (characters[tempIndex].GetComponent<SelectionData>().selectedPlayer
				== PlayerData.peerColor)
			{
				selectionWheel.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, ((tempIndex + 1) * 90), 0), Quaternion.Euler(0, (tempIndex * 90), 0), Time.deltaTime * 5.0f);
				tempIndex--;
				if (tempIndex <= -1)
				{
					tempIndex = 3;
				}
			}

			currentSelection = tempIndex;
			SetActiveSelection(currentSelection);

		}

		if (GUI.Button(selectButtonRect, new GUIContent(string.Empty, "select"), selectButtonStyle))
		{
			audio.PlayOneShot(buttonSelect);
			SelectCharacter(currentSelection);
			PlayerData.classId[PlayerData.color] = characters[currentSelection].GetComponent<SelectionData>().characterClassID;
			networkView.RPC("Peer_CharacterSelected", RPCMode.All, characters[currentSelection].GetComponent<SelectionData>().characterClassID);
			isSelectionDone = true;
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

	void UpdateSelection(int index)
	{
		selectionWheel.transform.rotation = Quaternion.Lerp(selectionWheel.transform.rotation, Quaternion.Euler(0, (index * 90), 0), Time.deltaTime * 5.0f);
	}


	void SetActiveSelection(int index)
	{
		characters[index].GetComponent<SelectionData>().SetPlayer(PlayerData.color);
		networkView.RPC("Peer_SetSelection", RPCMode.Others, index);
	}

	void ResetActiveSelection(int index)
	{
		characters[index].GetComponent<SelectionData>().SetPlayer(-1);
	}

	void SelectCharacter(int index)
	{
		characters[index].GetComponent<SelectionData>().SelectPlayer(PlayerData.color);
	}

	[RPC]
	void Peer_CharacterSelected(int index)
	{
		if (index != PlayerData.classId[PlayerData.color])
			PlayerData.classId[PlayerData.peerColor] = index;

		playerSelectionCount++;

		if (playerSelectionCount >= 2)
			LevelManager.Instance.RPC_LoadLevelWithLoadingScreen((int)LevelManager.Scene.Level_1, 0.5f, 1.0f);
	}

	[RPC]
	void Peer_SetSelection(int index)
	{
		otherPlayerSelection = index;
	}
}
