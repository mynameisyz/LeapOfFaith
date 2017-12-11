using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisconnectionHandler : MonoBehaviour 
{
	public static bool isDisconnectedFromPeer = false;
	public static bool isForcedDisconnect = false;
	
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		if (!isForcedDisconnect)
			isDisconnectedFromPeer = true;
		
		MasterServer.UnregisterHost();
		Network.Disconnect();
		Destroy(this.gameObject);
		Application.LoadLevel((int)LevelManager.Scene.MainMenu);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		if (!isForcedDisconnect)
			isDisconnectedFromPeer = true;
		
		Destroy(this.gameObject);
		Application.LoadLevel((int)LevelManager.Scene.MainMenu);
	}
}