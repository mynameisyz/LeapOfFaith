using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkTime : MonoBehaviour
{
	#region Singleton

	private static NetworkTime instance = null;
	public static NetworkTime Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	#endregion

	private float deltaTime;

	public float Time
	{
		get { return (float)Network.time + deltaTime; }
	}

	void Start()
	{
		if (Network.isServer)
			deltaTime = -(float)Network.time;
		else
			networkView.RPC("GetServerTime", RPCMode.Server);
	}

	void Update()
	{
	}

	[RPC]
	void GetServerTime(NetworkMessageInfo info)
	{
		networkView.RPC("SyncDeltaTime", info.sender, (float)Network.time + deltaTime);
	}

	[RPC]
	void SyncDeltaTime(float serverTime, NetworkMessageInfo info)
	{
		deltaTime = serverTime - (float)info.timestamp;
	}
}