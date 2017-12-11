using UnityEngine;
using System.Collections;

public class DebugMenu : MonoBehaviour {
	
	/*
	public GameObject spawner;
	public Transform playerPrefab;
	public Camera gameCamera;
	public GameObject player1Spawn;
	public GameObject player2Spawn;
	private GameObject[] gestureObjects;
	public Transform playerObject;
		
	// Use this for initialization
	void Start () {
		playerObject = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.Box (new Rect (10,10,200,400), "Debug Menu");
		if(playerObject == null)
		{
			// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
			if (GUI.Button (new Rect (20,40,150,20), "Spawn Player 1")) 
			{
				playerObject = (Transform)Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
				
				if(playerObject == null)
				{
					Debug.Log("object is null");
				}
				else
				{
					playerObject.GetComponent<PlayerData>().setPlayerOne();
					Debug.Log(playerObject.GetComponent<PlayerScript>().playerNo.ToString());
					playerObject.GetComponent<PlatformerController>().ChangeSpawnPoint(player1Spawn);
					playerObject.GetComponent<PlatformerController>().Spawn();
					playerObject.tag = "Player 1";
					playerObject.GetComponent<PlatformerController>().enabled = true;
									
					playerObject.gameObject.GetComponent<PlayerScript>().setElements();
					
					//Camera
					gameCamera.GetComponent<CameraZoom>().setPlayerOneTarget(playerObject.gameObject);
					gameCamera.GetComponent<CameraZoom>().setPlayerTwoTarget(playerObject.gameObject);
				}
			}
					// Make the second button.
			if (GUI.Button (new Rect (20,70,150,20), "Spawn Player 2")) 
			{
				playerObject = (Transform)Instantiate(playerPrefab, this.transform.position, this.transform.rotation);
				
				if(playerObject == null)
				{
					Debug.Log("object is null");
				}
				else
				{
					playerObject.GetComponent<PlayerScript>().setPlayerTwo();
					Debug.Log(playerObject.GetComponent<PlayerScript>().playerNo.ToString());
					playerObject.GetComponent<PlatformerController>().ChangeSpawnPoint(player2Spawn);
					playerObject.GetComponent<PlatformerController>().Spawn();
					playerObject.tag = "Player 2";
					playerObject.GetComponent<PlatformerController>().enabled = true;
					
					playerObject.gameObject.GetComponent<PlayerScript>().setElements();
				}
				
				gameCamera.GetComponent<CameraZoom>().setPlayerOneTarget(playerObject.gameObject);
				gameCamera.GetComponent<CameraZoom>().setPlayerTwoTarget(playerObject.gameObject);
			}
		}
		if(playerObject != null)
		{
			if (GUI.Button (new Rect (20,70,150,20), "Kill Player")) 
			{
				Destroy(playerObject.gameObject);
				playerObject = null;
			}
		}
	}
	*/
}