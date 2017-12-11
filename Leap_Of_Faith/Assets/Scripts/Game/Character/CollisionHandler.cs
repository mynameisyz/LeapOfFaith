using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionHandler : MonoBehaviour 
{
	public AudioClip checkpoint_reached;

	private class RendererTimeoutPair 
	{
		public GameObject gameObject;
		public float timeout;
		
		public RendererTimeoutPair(GameObject obj, float timeLeft)
		{
			gameObject = obj;
			timeout = Time.time + timeLeft;
		}
	}
	
	private List<RendererTimeoutPair> renderList = new List<RendererTimeoutPair>();
	
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
		for (int i = renderList.Count-1; i >= 0; i--) 
		{
			if (Time.time >= renderList[i].timeout) 
			{
				if ((PlayerData.color == PlayerData.PLAYER_RED &&
					GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostRed", renderList[i].gameObject.transform)) ||
					(PlayerData.color == PlayerData.PLAYER_BLUE &&
					GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostBlue", renderList[i].gameObject.transform)))
				{
					renderList[i].gameObject.renderer.enabled = false;
					renderList[i].gameObject.transform.Find("GlowEffect").renderer.enabled = false;
				}
				renderList.RemoveAt(i);
			}
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) 
	{
		switch (hit.gameObject.tag)
		{
			case "DeathOnTouch":
				this.gameObject.GetComponent<PlatformerController>().RPC_Respawn();
				break;

			case "Checkpoint":
				int spawnPointIndex = LevelData.Instance.Checkpoints_IndexOf(hit.gameObject);

				if (spawnPointIndex >= this.gameObject.GetComponent<PlatformerController>().currentSpawnPointIndex)
					this.gameObject.GetComponent<PlatformerController>().RPC_ChangeSpawnPoint(spawnPointIndex);

				Physics.IgnoreCollision(this.gameObject.collider, hit.gameObject.collider);

				hit.gameObject.audio.PlayOneShot(checkpoint_reached);

				if (networkView.isMine)
				{
					hit.gameObject.animation.Play("open");
					hit.gameObject.animation.PlayQueued("idle");
				}

				break;
		}
		
		if ((PlayerData.color == PlayerData.PLAYER_RED &&
			GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostRed", hit.transform)) ||
			(PlayerData.color == PlayerData.PLAYER_BLUE &&
			GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostBlue", hit.transform)))
		{
			RendererTimeoutPair existingObj = renderList.Find(obj => obj.gameObject == hit.gameObject);

			if (existingObj != null)
				existingObj.timeout = Time.time + 1.0f;
			else
			{
				hit.gameObject.renderer.enabled = true;
				hit.transform.Find("GlowEffect").renderer.enabled = true;
				renderList.Add(new RendererTimeoutPair(hit.gameObject, 1.0f));
			}
		}
	}
}
