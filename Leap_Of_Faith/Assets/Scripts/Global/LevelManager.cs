using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	#region Singleton

	private static LevelManager instance = null;
	public static LevelManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	#endregion

	public enum Scene
	{
		SplashScreen,
		MainMenu,
		CharacterSelection,
		LoadingScreen,
		Level_1,
		Level_3,
		Credits
	}

	private int levelToLoad = 0;
	private float fadeOutTime = 0.0f;
	private int playersFadedCount = 0;

	private bool isFadingIn = false;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void RPC_LoadLevel(int _levelToLoad, float _fadeInTime, float _fadeOutTime)
	{
		levelToLoad = _levelToLoad;
		playersFadedCount = 0;

		if (Network.isServer)
			networkView.RPC("StartFadingIn", RPCMode.All, _fadeInTime, _fadeOutTime);
	}

	public void RPC_LoadLevelWithLoadingScreen(int _levelToLoad, float _fadeInTime, float _fadeOutTime)
	{
		LoadingScreenManager.nextLevel = _levelToLoad;
		RPC_LoadLevel((int)Scene.LoadingScreen, _fadeInTime, _fadeOutTime);
	}

	#region Private_Implementations

	[RPC]
	private void StartFadingIn(float _fadeInTime, float _fadeOutTime, NetworkMessageInfo info)
	{
		fadeOutTime = _fadeOutTime;
		ScreenColorOverlay.Instance.FadeToColor(Color.black, _fadeInTime);
		isFadingIn = true;
	}

	private void OnScreenColorOverlayFadeComplete(Color overlayColor)
	{
		if (!isFadingIn)
			return;

		if (overlayColor == Color.black)
		{
			isFadingIn = false;
			networkView.RPC("Peer_PlayerFadedIn", RPCMode.All);
		}
	}

	[RPC]
	private void Peer_PlayerFadedIn()
	{
		playersFadedCount++;
		if (playersFadedCount >= 2 && Network.isServer)
			networkView.RPC("DoLoadLevel", RPCMode.All, levelToLoad);
	}

	[RPC]
	private void DoLoadLevel(int _levelToLoad, NetworkMessageInfo info)
	{
		Application.LoadLevel(_levelToLoad);
		ScreenColorOverlay.Instance.FadeToColor(Color.clear, fadeOutTime);
	}

	#endregion
}
