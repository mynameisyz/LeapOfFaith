/*
 * 
 * WRITTEN BY ONG HENG LE
 * 
 * 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MMClient : MonoBehaviour
{
	private string mmserverTypeName = "Leap_Of_Faith_MMServer";
	
	// Use this for initialization
	void Start() 
	{
		MasterServer.ipAddress = InputServerIP();
		MasterServer.port = 23466;

		Network.proxyIP = MasterServer.ipAddress;
		Network.proxyPort = 10746;
	}

	private string InputServerIP()
	{
		if (!Directory.Exists(Application.dataPath + @"/ServerConfig/"))
			Directory.CreateDirectory(Application.dataPath + @"/ServerConfig/");

		if (File.Exists(Application.dataPath + @"/ServerConfig/ServerIP.txt"))
		{
			try
			{
				using (StreamReader sw = new StreamReader(Application.dataPath + @"/ServerConfig/ServerIP.txt"))
				{
					string readIp = sw.ReadLine();
					string[] ipFields = readIp.Split('.');

					if (ipFields.Length != 4)
						return Network.player.ipAddress;

					int ipFieldValue = -1;
					foreach (string ipField in ipFields)
					{
						ipFieldValue = -1;

						if (int.TryParse(ipField, out ipFieldValue) == false)
							return Network.player.ipAddress;

						if (ipFieldValue < 0 || ipFieldValue > 999)
							return Network.player.ipAddress;
					}

					return readIp;
				}
			}
			catch (Exception e)
			{
				Debug.Log("[InputServerIP] " + e.ToString());
			}
		}

		return Network.player.ipAddress;
	}
	
	// Update is called once per frame
	void Update() 
	{
		UpdateMMPhase();
		UpdateMMServerConnection();
		UpdateMatchmaking();
		UpdateConnectMatch();
		UpdateServerTimeout();
	}

	#region MMClientList

	private enum PeerConnectionType
	{
		None,
		Proxy,
		PeerToPeer
	}

	private class MMClientPeer
	{
		public NetworkPlayer networkPlayer;
		public PeerConnectionType peerConnectionType;

		private Ping ping;
		private int pingValueTotal;
		private int pingCount;

		private float pingTimeout;
		private const float PING_TIMEOUT = 3.0f;

		private int packetLossCount;
		private const int PACKETLOSS_THRESHOLD = 4;

		public MMClientPeer(NetworkPlayer player)
		{
			networkPlayer = player;
			peerConnectionType = PeerConnectionType.PeerToPeer;

			ping = null;
			pingValueTotal = 0;
			pingCount = 0;

			pingTimeout = 0.0f;
			packetLossCount = 0;
		}

		public void Update()
		{
			if (ping != null)
			{
				if (ping.isDone)
				{
					pingValueTotal += ping.time;
					pingCount++;
					ping = null;
				}
				else if (Time.time >= pingTimeout)
				{
					packetLossCount++;
					ping = null;

					if (packetLossCount >= PACKETLOSS_THRESHOLD)
						peerConnectionType = PeerConnectionType.Proxy;
				}
			}
		}

		public void StartPing()
		{
			if (ping == null)
			{
				ping = new Ping(networkPlayer.externalIP);
				pingTimeout = Time.time + PING_TIMEOUT;
			}
		}

		public void KillPing()
		{
			ping = null;
		}

		public int GetAveragePing()
		{
			if (pingCount < 5)
				return int.MaxValue;

			return pingValueTotal / pingCount;
		}
	}

	private int mmclientIndex = -1;
	private List<MMClientPeer> mmclientList = new List<MMClientPeer>();
	private List<string> failedPeerGUIDList = new List<string>();

	[RPC]
	void MMCommon_AddConnectedPlayer(NetworkPlayer player)
	{
		mmclientList.Add(new MMClientPeer(player));

		if (failedPeerGUIDList.Contains(player.guid))
		{
			mmclientList[mmclientList.Count - 1].peerConnectionType = PeerConnectionType.Proxy;
		}
		else if (player == Network.player)
		{
			mmclientIndex = mmclientList.Count - 1;
			mmclientList[mmclientIndex].peerConnectionType = PeerConnectionType.None;
			SwitchMMPhase(MMPhase.FindingMatch);
		}
	}

	[RPC]
	void MMCommon_RemoveConnectedPlayerAtIndex(int playerIndex)
	{
		mmclientList.RemoveAt(playerIndex);

		if (playerIndex < mmclientIndex)
			mmclientIndex--;
	}

	#endregion

	#region MMPhase Async

	public enum MMPhase
	{
		MMServerNotFound = -4,
		MMServerConnectionLost = -3,
		MMServerConnectionTimedOut = -2,
		MMServerConnectionFailed = -1,
		None,
		RequestingMMServerConnection,
		ConnectingToMMServer,
		ReadyAndIdle,
		FindingMatch,
		RequestingMatchConnection,
		MatchFound,
		DisconnectingFromMMServer,
		ClientConnectingToMatchServer,
		ServerWaitingForMatchClient,
		Completed
	}

	public MMPhase mmPhase = MMPhase.None;
	private MMPhase nextPhase = MMPhase.None;
	private bool isConnectedToMMServer = false;

	public void SwitchMMPhase(MMPhase nextPhase)
	{
		this.nextPhase = nextPhase;
	}
	
	void UpdateMMPhase()
	{
		if (mmPhase != nextPhase)
		{
			if (nextPhase > mmPhase)
			{
				if (mmPhase != MMPhase.ConnectingToMMServer || isConnectedToMMServer)
					mmPhase++;
			}
			else if (nextPhase >= 0)
			{
				if (nextPhase == MMPhase.None || !isConnectedToMMServer)
					mmPhase = MMPhase.None;
				else
					mmPhase = MMPhase.ReadyAndIdle;
			}
			else
			{
				mmPhase = nextPhase;
			}

			switch (mmPhase)
			{
				case MMPhase.MMServerNotFound:
					break;

				case MMPhase.MMServerConnectionLost:
					break;

				case MMPhase.MMServerConnectionTimedOut:
					break;

				case MMPhase.MMServerConnectionFailed:
					break;

				case MMPhase.None:
					isConnectedToMMServer = false;
					MasterServer.UnregisterHost();
					Network.Disconnect();
					break;

				case MMPhase.RequestingMMServerConnection:
					mmclientList.Clear();
					mmclientIndex = -1;
					MasterServer.ClearHostList();
					MasterServer.RequestHostList(mmserverTypeName);
					mmserverConnectionTimeout = Time.time + MMSERVERCONNECTION_TIMEOUT;
					break;

				case MMPhase.ConnectingToMMServer:
					break;

				case MMPhase.ReadyAndIdle:
					mmclientList.ForEach(mmclientPeer => mmclientPeer.KillPing());
					break;

				case MMPhase.FindingMatch:
					nextPingTime = Time.time + PINGTIME_INTERVAL;
					nextMatchPeerTime = Time.time + MATCHPEERTIME_INTERVAL;
					PingAllPeers();
					break;

				case MMPhase.RequestingMatchConnection:
					break;

				case MMPhase.MatchFound:
					break;

				case MMPhase.DisconnectingFromMMServer:
					isConnectedToMMServer = false;
					Network.Disconnect();
					break;

				case MMPhase.ClientConnectingToMatchServer:
					isWaitingConnectMatch = true;
					connectMatchWaitTime = Time.time + CONNECTMATCHWAIT_TIME;
					connectMatchFailCount = 0;
					break;

				case MMPhase.ServerWaitingForMatchClient:
					serverTimeout = Time.time + SERVER_TIMEOUT;
					if (!Network.isClient)
						Network.InitializeServer(1, 25003, !Network.HavePublicAddress());
					break;

				case MMPhase.Completed:
					break;
			}
		}
	}

	#endregion

	#region Matchmaking Logic

	private float nextPingTime = 0.0f;
	private const float PINGTIME_INTERVAL = 1.0f;

	private float nextMatchPeerTime = 0.0f;
	private const float MATCHPEERTIME_INTERVAL = 5.0f;

	private int pingThreshold = 0;
	private const int PINGTHRESHOLD_INTERVAL = 50;

	void UpdateMatchmaking()
	{
		if (mmPhase == MMPhase.FindingMatch)
		{
			mmclientList.ForEach(mmclientPeer => mmclientPeer.Update());

			if (Time.time >= nextMatchPeerTime &&
				mmclientList.Count > 1)
			{
				nextMatchPeerTime = Time.time + MATCHPEERTIME_INTERVAL;
				FindBestPeer();
			}

			if (Time.time >= nextPingTime)
			{
				nextPingTime = Time.time + PINGTIME_INTERVAL;
				PingAllPeers();
			}
		}
	}

	void PingAllPeers()
	{
		foreach (MMClientPeer peer in mmclientList)
		{
			if (peer.peerConnectionType == PeerConnectionType.PeerToPeer)
				peer.StartPing();
		}
	}
	
	void FindBestPeer()
	{
		int bestPeerIndex = -1;
		bool isSwitchingToProxy = true;

		int bestPeerPing = int.MaxValue;
		int peerPing = int.MaxValue;

		for (int i = 0; i < mmclientList.Count; i++)
		{
			if (mmclientList[i].peerConnectionType == PeerConnectionType.PeerToPeer)
			{
				isSwitchingToProxy = false;

				peerPing = mmclientList[i].GetAveragePing();

				if ((bestPeerPing < 0 && peerPing <= pingThreshold) ||
					(bestPeerPing >= 0 && peerPing < bestPeerPing))
				{
					bestPeerIndex = i;
					bestPeerPing = peerPing;
				}
			}
		}

		if (isSwitchingToProxy)
		{
			networkView.RPC("MMServer_ClientProxyConnectRequest", RPCMode.Server, mmclientIndex);
			SwitchMMPhase(MMPhase.RequestingMatchConnection);
		}
		else if (bestPeerIndex >= 0)
		{
			networkView.RPC("MMServer_ClientConnectRequest", RPCMode.Server, mmclientIndex, bestPeerIndex);
			SwitchMMPhase(MMPhase.RequestingMatchConnection);
		}
		else
		{
			pingThreshold += PINGTHRESHOLD_INTERVAL;
		}
	}

	[RPC]
	void MMClient_ClientConnectRequestFailed()
	{
		SwitchMMPhase(MMPhase.FindingMatch);
	}

	#endregion

	#region Hosting / Joining Match

	private string serverUID = "";
	private string matchedPlayerGUID = "";
	private bool isHostingGameServer = false;

	public void StartMatchConnection()
	{
		if (mmPhase == MMPhase.MatchFound)
		{
			if (isHostingGameServer)
				SwitchMMPhase(MMPhase.ServerWaitingForMatchClient);
			else
				SwitchMMPhase(MMPhase.ClientConnectingToMatchServer);
		}
	}

	#region Hosting Matched Server

	private float serverTimeout = 0.0f;
	private const float SERVER_TIMEOUT = 15.0f;

	[RPC]
	void MMClient_ClientHostServer(string serverUID, string playerGUID, bool isProxyEnabled)
	{
		this.serverUID = serverUID;
		matchedPlayerGUID = playerGUID;

		Network.useProxy = isProxyEnabled;
		isHostingGameServer = true;
		SwitchMMPhase(MMPhase.MatchFound);
	}

	void OnServerInitialized(NetworkPlayer player)
	{
		if (Network.useProxy)
			MasterServer.RegisterHost(serverUID, "Matchmade Game", player.ipAddress + ":" + player.port);
		else
			MasterServer.RegisterHost(serverUID, "Matchmade Game");
	}

	void UpdateServerTimeout()
	{
		if (mmPhase == MMPhase.ServerWaitingForMatchClient)
		{
			if (Time.time >= serverTimeout)
			{
				//failed
				failedPeerGUIDList.Add(matchedPlayerGUID);
				SwitchMMPhase(MMPhase.FindingMatch);
			}
		}
	}

	#endregion

	#region Joining Matched Server

	private bool isWaitingConnectMatch = false;
	private float connectMatchWaitTime = 0.0f;
	private const float CONNECTMATCHWAIT_TIME = 3.0f;

	private float connectMatchTimeout = 0.0f;
	private const float CONNECTMATCH_TIMEOUT = 5.0f;

	private int connectMatchFailCount = 0;
	private const int CONNECTMATCHFAILCOUNT_THRESHOLD = 3;

	[RPC]
	void MMClient_ClientJoinServer(string serverUID, string playerGUID)
	{
		this.serverUID = serverUID;
		matchedPlayerGUID = playerGUID;
		
		isHostingGameServer = false;
		SwitchMMPhase(MMPhase.MatchFound);
	}

	void UpdateConnectMatch()
	{
		if (mmPhase == MMPhase.ClientConnectingToMatchServer)
		{
			if (isWaitingConnectMatch)
			{
				if (Time.time >= connectMatchWaitTime)
				{
					isWaitingConnectMatch = false;
					connectMatchTimeout = Time.time + CONNECTMATCH_TIMEOUT;
					MasterServer.ClearHostList();
					MasterServer.RequestHostList(serverUID);
				}
			}
			else
			{
				if (Time.time >= connectMatchTimeout)
				{
					Debug.Log("ConnectMatch Timeout!");
					OnFailedToConnect(NetworkConnectionError.ConnectionFailed);
				}
			}
		}
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		Debug.Log(msEvent.ToString());
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			HostData[] data = MasterServer.PollHostList();
			if (data.Length > 0)
			{
				if (data[0].comment != string.Empty)
					Network.Connect(data[0].comment.Split(':')[0], System.Convert.ToInt32(data[0].comment.Split(':')[1]));
				else
					Network.Connect(data[0]);
			}
			else
			{
				//No host found!
				if (mmPhase == MMPhase.ConnectingToMMServer)
					SwitchMMPhase(MMPhase.MMServerNotFound);
				else if (mmPhase == MMPhase.ClientConnectingToMatchServer)
					OnFailedToConnect(NetworkConnectionError.EmptyConnectTarget);
			}
		}
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("OnFailedToConnect: " + error);
		connectMatchFailCount++;
		if (connectMatchFailCount < CONNECTMATCHFAILCOUNT_THRESHOLD)
		{
			connectMatchWaitTime = Time.time + CONNECTMATCHWAIT_TIME;
			isWaitingConnectMatch = true;
		}
		else
		{
			//failed
			failedPeerGUIDList.Add(matchedPlayerGUID);
			SwitchMMPhase(MMPhase.FindingMatch);
		}
	}

	#endregion

	[RPC]
	void MMClient_ClientHasPublicIP()
	{
		networkView.RPC("MMServer_ClientHasPublicIP", RPCMode.Server, mmclientIndex, Network.HavePublicAddress());
	}

	[RPC]
	void MMClient_RestartMatchmaking()
	{
		SwitchMMPhase(MMPhase.FindingMatch);
	}

	#endregion
	
	#region Connection Results

	private float mmserverConnectionTimeout = 0.0f;
	private const float MMSERVERCONNECTION_TIMEOUT = 30.0f;

	void UpdateMMServerConnection()
	{
		if (mmPhase == MMPhase.ConnectingToMMServer)
			if (Time.time >= mmserverConnectionTimeout)
			{
				Debug.Log("MMServer Connection Timed Out!");
				SwitchMMPhase(MMPhase.MMServerConnectionTimedOut);
			}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (mmPhase == MMPhase.ServerWaitingForMatchClient)
			SwitchMMPhase(MMPhase.Completed);
	}

	void OnConnectedToServer()
	{
		if (mmPhase == MMPhase.ConnectingToMMServer)
			isConnectedToMMServer = true;
		else if (mmPhase == MMPhase.ClientConnectingToMatchServer)
			SwitchMMPhase(MMPhase.Completed);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (isConnectedToMMServer)
			SwitchMMPhase(MMPhase.MMServerConnectionLost);
	}

	void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		SwitchMMPhase(MMPhase.MMServerConnectionFailed);
	}

	#endregion

	#region MMServer_Declarations

	[RPC]
	void MMServer_ClientProxyConnectRequest(int senderIndex)
	{
	}

	[RPC]
	void MMServer_ClientConnectRequest(int senderIndex, int playerIndex)
	{
	}

	[RPC]
	void MMServer_ClientHasPublicIP(int senderIndex, bool hasPublicIP)
	{
	}

	#endregion
}
