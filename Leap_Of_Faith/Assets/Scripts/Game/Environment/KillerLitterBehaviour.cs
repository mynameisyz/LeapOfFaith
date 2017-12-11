using UnityEngine;
using System.Collections;

public class KillerLitterBehaviour : MonoBehaviour 
{
	private float myTimer;
	public float lifeTime = 1.5f;

	private static Color redColor = new Color(0.75f, 0.25f, 0.25f, 1.0f);
	private static Color blueColor = new Color(0.1f, 0.25f, 0.5f, 1.0f);

	// Use this for initialization
	void Start () 
	{
	}

	public void RPC_SetGhostColor(bool isRed)
	{
		networkView.RPC("SetGhostColor", RPCMode.All, isRed);
	}

	[RPC]
	private void SetGhostColor(bool isRed)
	{
		if (isRed)
		{
			this.GetComponentInChildren<GlowEffect>().SetColor(redColor);
			if (PlayerData.color == PlayerData.PLAYER_RED)
			{
				foreach (Renderer rend in this.GetComponentsInChildren<Renderer>())
					rend.enabled = false;
			}
		}
		else
		{
			this.GetComponentInChildren<GlowEffect>().SetColor(blueColor);
			if (PlayerData.color == PlayerData.PLAYER_BLUE)
			{
				foreach (Renderer rend in this.GetComponentsInChildren<Renderer>())
					rend.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		myTimer += Time.deltaTime;
		
		if (myTimer >= lifeTime)
		{
			Destroy (this.gameObject);
		}
	}
}
