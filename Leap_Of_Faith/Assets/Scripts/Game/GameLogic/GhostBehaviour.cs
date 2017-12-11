using UnityEngine;
using System.Collections;

public class GhostBehaviour : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	public static void UpdateGhostBehaviour()
	{
		GameObject[] ghostObjects = GameObject.FindGameObjectsWithTag("GhostRed");

		foreach (GameObject obj in ghostObjects)
		{
			// Disable parent collider
			if (obj.collider != null)
			{
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_RED].collider, obj.collider, false);
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_BLUE].collider, obj.collider, true);
			}

			// Disable children collider
			foreach (Collider childCollider in obj.GetComponentsInChildren<Collider>())
			{
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_RED].collider, childCollider, false);
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_BLUE].collider, childCollider, true);
			}

			//fade in/out red platforms here!
			Color redColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);

			// Disable parent renderer
			if (obj.renderer != null)
			{
				obj.renderer.material.color = redColor;
				obj.renderer.enabled = (PlayerData.color != PlayerData.PLAYER_RED);
				if (obj.transform.Find("/GlowEffect") != null)
					obj.transform.Find("/GlowEffect").renderer.enabled = (PlayerData.color != PlayerData.PLAYER_RED);
			}

			// Disable children renderer
			foreach (Renderer childRenderer in obj.GetComponentsInChildren<Renderer>())
			{
				childRenderer.material.color = redColor;
				childRenderer.enabled = (PlayerData.color != PlayerData.PLAYER_RED);
				if (childRenderer.transform.Find("GlowEffect") != null)
					childRenderer.transform.Find("GlowEffect").renderer.enabled = (PlayerData.color != PlayerData.PLAYER_RED);
			}
		}

		ghostObjects = GameObject.FindGameObjectsWithTag("GhostBlue");

		foreach (GameObject obj in ghostObjects)
		{
			// Disable parent collider
			if (obj.collider != null)
			{
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_RED].collider, obj.collider, true);
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_BLUE].collider, obj.collider, false);
			}

			// Disable children collider
			foreach (Collider childCollider in obj.GetComponentsInChildren<Collider>())
			{
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_RED].collider, childCollider, true);
				Physics.IgnoreCollision(PlayerData.characters[PlayerData.PLAYER_BLUE].collider, childCollider, false);
			}

			//
			//
			//fade in/out blue platforms here!
			Color blueColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);

			// Disable parent renderer
			if (obj.renderer != null)
			{
				obj.renderer.material.color = blueColor;
				obj.renderer.enabled = (PlayerData.color != PlayerData.PLAYER_BLUE);
				if (obj.transform.Find("/GlowEffect") != null)
					obj.transform.Find("/GlowEffect").renderer.enabled = (PlayerData.color != PlayerData.PLAYER_BLUE);
			}

			// Disable children renderer
			foreach (Renderer childRenderer in obj.GetComponentsInChildren<Renderer>())
			{
				childRenderer.material.color = blueColor;
				childRenderer.enabled = (PlayerData.color != PlayerData.PLAYER_BLUE);
				if (childRenderer.transform.Find("GlowEffect") != null)
					childRenderer.transform.Find("GlowEffect").renderer.enabled = (PlayerData.color != PlayerData.PLAYER_BLUE);
			}
		}
	}
}