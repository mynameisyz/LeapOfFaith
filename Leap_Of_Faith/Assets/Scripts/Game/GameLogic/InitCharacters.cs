using UnityEngine;
using System.Collections;

public class InitCharacters : MonoBehaviour
{
	public GameObject[] characterClasses;
		
	// Use this for initialization
	void Start()
	{
		Random.seed = (int)NetworkTime.Instance.Time;

		GameObject playerObject = (GameObject)Network.Instantiate(characterClasses[PlayerData.classId[PlayerData.color]],
															LevelData.Instance.checkpoints[PlayerData.color].transform.position,
															LevelData.Instance.checkpoints[PlayerData.color].transform.rotation, 
															0);

		playerObject.GetComponent<PlatformerController>().RPC_ChangeSpawnPoint(PlayerData.color);
		PlayerData.characters[PlayerData.color] = playerObject;

		Camera.main.GetComponent<SkillIcon>().enabled = true;

		networkView.RPC("Peer_StoreOtherCharacter", RPCMode.Others);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	[RPC]
	void Peer_StoreOtherCharacter()
	{
		GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");

		if (characters.Length >= 2)
		{
			foreach (GameObject character in characters)
			{
				if (character != PlayerData.characters[PlayerData.color])
					PlayerData.characters[PlayerData.peerColor] = character;

				if (character == PlayerData.characters[PlayerData.PLAYER_RED])
				{
					character.GetComponent<GlowEffect>().SetColor(new Color(0.75f, 0.25f, 0.25f, 1.0f));
					character.GetComponent<GlowEffect>().glowPos.z -= 0.01f;
				}
				else
					character.GetComponent<GlowEffect>().SetColor(new Color(0.1f, 0.25f, 0.5f, 1.0f));
			}
		}

		GhostBehaviour.UpdateGhostBehaviour();
	}
}