using UnityEngine;
using System.Collections;

public class SelectionData : MonoBehaviour 
{
	public ParticleSystem particlesRed;
	public ParticleSystem particlesBlue;
	public Light lightBlue;
	public Light lightRed;
	public int characterClassID = 0;
	public int selectedPlayer = -1;
	
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}
	
	public void SetPlayer(int player)
	{
		networkView.RPC("RPC_SetPlayer", RPCMode.All, player);
	}
	
	public void SelectPlayer(int player)
	{
		networkView.RPC("RPC_SetParticles", RPCMode.All, player);
	}
	
	[RPC]
	void RPC_SetParticles(int player)
	{
		if(player == 0)
		{
			particlesRed.enableEmission = true;
			particlesBlue.enableEmission = false;
		}
		if(player == 1)
		{
			particlesRed.enableEmission = false;
			particlesBlue.enableEmission = true;
		}
	}
	
	[RPC]
	void RPC_SetPlayer(int player)
	{
		this.selectedPlayer = player;
		if(selectedPlayer == -1)
		{
			lightBlue.enabled = false;
			lightRed.enabled = false;
		}
		if(selectedPlayer == 0)
		{
			lightBlue.enabled = false;
			lightRed.enabled = true;
		}
		if(selectedPlayer == 1)
		{
			lightBlue.enabled = true;
			lightRed.enabled = false;
		}
	}
}
