using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	public const int PLAYER_RED = 0;
	public const int PLAYER_BLUE = 1;

	public const int CHARACTERCLASS_INDIAN = 0;
	public const int CHARACTERCLASS_CHINESE = 1;
	public const int CHARACTERCLASS_MALAY = 2;
	public const int CHARACTERCLASS_EURASIAN = 3;

	public static int color = -1;
	public static int peerColor = -1;

	public static int[] classId = new int[2];

	public static GameObject[] characters = new GameObject[2];
	
	void Awake()
	{
		DontDestroyOnLoad(this);
		if (Network.isServer)
		{
			color = PLAYER_RED;
			peerColor = PLAYER_BLUE;
		}
		else
		{
			color = PLAYER_BLUE;
			peerColor = PLAYER_RED;
		}
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
	}
}
