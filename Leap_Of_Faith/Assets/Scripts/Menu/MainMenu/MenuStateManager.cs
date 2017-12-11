using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStateManager : MonoBehaviour 
{
	public Transform[] textPositions;

	private enum MenuState
	{
		None,
		MainMenu,
		FindingMatch,
		MatchFound,
		OptionsMenu,
		MMServerConnectionFailed,
		MMServerConnectionTimedOut,
		MMServerConnectionLost,
		MMServerNotFound,
		PeerConnectionLost,
		Completed
	}
	private MenuState currentState = MenuState.None;
	private BoundsButton[] buttons;

	private BitTextManager bitTextManager = null;
	private MMClient mmClient = null;
	
	public LocalData localData;

	// Use this for initialization
	void Start() 
	{
		bitTextManager = GetComponent<BitTextManager>();
		mmClient = GetComponent<MMClient>();
		
		buttons = new BoundsButton[3];
		for (int i = 0; i < buttons.Length; i++ )
			buttons[i] = new BoundsButton();
	}

	void LateUpdate()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i] != null)
			{
				if (BoundsButton.BoundsContainsScreenPoint(buttons[i].bounds, Input.mousePosition))
					bitTextManager.GlowString(buttons[i].bounds);
			}
		}
	}

	// Update is called once per frame
	void Update() 
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i] != null)
				buttons[i].Update();
		}

		switch (currentState)
		{
			case MenuState.None:
				if (DisconnectionHandler.isDisconnectedFromPeer)
					FunctionCallHelper.Instance.DelayedCallOnce(Func_ShowConnectionLost, 1.0f);
				else
					FunctionCallHelper.Instance.DelayedCallOnce(Func_ToMainMenu, 0.5f);
				break;

			case MenuState.FindingMatch:
				if (bitTextManager.IsAnimationCompleted())
				{
					FunctionCallHelper.Instance.CallOnce(Func_StartMatchmaking);

					// -- Connection Error Messages --
					if (mmClient.mmPhase == MMClient.MMPhase.MMServerConnectionFailed)
					{
						mmClient.SwitchMMPhase(MMClient.MMPhase.None);
						SwitchState(MenuState.MMServerConnectionFailed);
					}
					else if (mmClient.mmPhase == MMClient.MMPhase.MMServerConnectionTimedOut)
					{
						mmClient.SwitchMMPhase(MMClient.MMPhase.None);
						SwitchState(MenuState.MMServerConnectionTimedOut);
					}
					else if (mmClient.mmPhase == MMClient.MMPhase.MMServerConnectionLost)
					{
						mmClient.SwitchMMPhase(MMClient.MMPhase.None);
						SwitchState(MenuState.MMServerConnectionLost);
					}
					else if (mmClient.mmPhase == MMClient.MMPhase.MMServerNotFound)
					{
						mmClient.SwitchMMPhase(MMClient.MMPhase.None);
						SwitchState(MenuState.MMServerNotFound);
					}
					// Connected!
					else if (mmClient.mmPhase >= MMClient.MMPhase.MatchFound)
						SwitchState(MenuState.MatchFound);
				}
				break;

			case MenuState.MatchFound:
				if (bitTextManager.IsAnimationCompleted())
				{
					if (mmClient.mmPhase < MMClient.MMPhase.MatchFound)
					{
						SwitchState(MenuState.FindingMatch);
					}
					else if (mmClient.mmPhase == MMClient.MMPhase.MatchFound)
					{
						mmClient.StartMatchConnection();
					}
					else if (mmClient.mmPhase == MMClient.MMPhase.Completed)
					{
						FunctionCallHelper.Instance.CallOnce(Func_LoadCharacterSelection);
						FunctionCallHelper.Instance.DelayedCallOnce(Func_ToCompleted, 1.0f);
					}
				}
				break;

			case MenuState.Completed:
				if (bitTextManager.IsAnimationCompleted())
				{
					FunctionCallHelper.Instance.CallOnce(Func_ToCharacterSelection);
					FunctionCallHelper.Instance.CallOnce(Func_DestroyMainMenu);
				}
				break;
		}
	}

	private void SwitchState(MenuState state)
	{
		FunctionCallHelper.Instance.Reset();
		bitTextManager.Reset(1.0f);
		for (int i = 0; i < buttons.Length; i++ )
			buttons[i].SetEnabled(false, 0.0f);

		currentState = state;

		switch (currentState)
		{
			case MenuState.None:
				break;

			case MenuState.MainMenu:
				bitTextManager.SetupBitString("Leap Of Faith", textPositions[0].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[   Play   ]", textPositions[1].position, 1.0f),
										Func_ToFindingMatch);
				buttons[0].SetEnabled(true, 1.0f);
				buttons[1].Reconstruct(bitTextManager.SetupBitString("[ Options ]", textPositions[2].position, 1.0f),
										Func_ToOptionsMenu);
				buttons[1].SetEnabled(true, 1.0f);
				buttons[2].Reconstruct(bitTextManager.SetupBitString("[   Exit   ]", textPositions[3].position, 1.0f),
										Func_ExitGame);
				buttons[2].SetEnabled(true, 1.0f);
				break;

			case MenuState.FindingMatch:
				bitTextManager.SetupBitString("Finding match...", textPositions[4].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[ Cancel ]", textPositions[2].position, 1.0f),
										Func_CancelMatchmaking);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.MatchFound:
				bitTextManager.SetupBitString("Match found!", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("Connecting...", textPositions[3].position, 1.0f);
				break;

			case MenuState.OptionsMenu:
				bitTextManager.SetupBitString("Options", textPositions[0].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[ Kinect: " + (LocalData.isKinectEnabled ? "On" : "Off") + " ]", textPositions[1].position, 1.0f),
										Func_ToggleKinect);
				buttons[0].SetEnabled(true, 1.0f);
				buttons[1].Reconstruct(bitTextManager.SetupBitString("[ Cancel ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[1].SetEnabled(true, 1.0f);
				break;

			case MenuState.MMServerConnectionFailed:
				bitTextManager.SetupBitString("Error!", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("Connection failed", textPositions[1].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[    OK    ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.MMServerConnectionTimedOut:
				bitTextManager.SetupBitString("Error!", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("Connection timed out", textPositions[1].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[    OK    ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.MMServerConnectionLost:
				bitTextManager.SetupBitString("Error!", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("Connection lost", textPositions[1].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[    OK    ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.MMServerNotFound:
				bitTextManager.SetupBitString("Error!", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("Server not found", textPositions[1].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[    OK    ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.PeerConnectionLost:
				bitTextManager.SetupBitString("Your new buddy", textPositions[4].position, 1.0f);
				bitTextManager.SetupBitString("has left the game...", textPositions[1].position, 1.0f);
				buttons[0].Reconstruct(bitTextManager.SetupBitString("[    OK    ]", textPositions[3].position, 1.0f),
										Func_ToMainMenu);
				buttons[0].SetEnabled(true, 1.0f);
				break;

			case MenuState.Completed:
				break;
		}
	}

	#region ButtonClick_Func_Delegates

	private void Func_ToMainMenu()
	{
		SwitchState(MenuState.MainMenu);
	}

	private void Func_ToOptionsMenu()
	{
		SwitchState(MenuState.OptionsMenu);
	}

	private void Func_ToFindingMatch()
	{
		SwitchState(MenuState.FindingMatch);
	}

	private void Func_CancelMatchmaking()
	{
		mmClient.SwitchMMPhase(MMClient.MMPhase.None);
		SwitchState(MenuState.MainMenu);
	}

	private void Func_ToggleKinect()
	{
		LocalData.isKinectEnabled = !LocalData.isKinectEnabled;
		SwitchState(MenuState.OptionsMenu);
	}

	private void Func_ExitGame()
	{
		System.Diagnostics.Process.GetCurrentProcess().Kill();
	}

	#endregion

	#region GameLoop_Func_Delegates

	private void Func_ShowConnectionLost()
	{
		DisconnectionHandler.isDisconnectedFromPeer = false;
		SwitchState(MenuState.PeerConnectionLost);
	}

	private void Func_StartMatchmaking()
	{
		mmClient.SwitchMMPhase(MMClient.MMPhase.FindingMatch);
	}

	private void Func_LoadCharacterSelection()
	{
		Application.LoadLevelAdditive((int)LevelManager.Scene.CharacterSelection);
	}

	private void Func_ToCompleted()
	{
		SwitchState(MenuState.Completed);
	}

	private void Func_ToCharacterSelection()
	{
		Camera.main.GetComponent<SmoothTranslation>().MoveTo(new Vector3(-0.542f, -107.3539f, -43.9f), 1.0f);
		Camera.main.GetComponent<Rotation>().RotateTo(new Vector3(5.81f, 0.0f, 0.0f), 1.0f);
		CharacterSelection.StartGUIFadeIn(1.0f);
	}

	private void Func_DestroyMainMenu()
	{
		// Destroy main menu objects here.
		Destroy(this.gameObject);
	}

	#endregion
}
