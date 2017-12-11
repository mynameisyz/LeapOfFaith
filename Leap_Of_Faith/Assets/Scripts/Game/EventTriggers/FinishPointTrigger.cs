using UnityEngine;
using System.Collections;

public class FinishPointTrigger : MonoBehaviour
{
	public float fadeInTime = 0.0f;
	public float fadeOutTime = 0.0f;

	private int characterCount = 0;
	
	public Transform elevator;
	public AudioClip elevatorDoor;
	
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	void OnTriggerEnter(Collider other)
	{
		characterCount++;
		if (characterCount >= 2)
		{
			SendMessageUpwards("OnFinishPointTriggered", SendMessageOptions.DontRequireReceiver);
			LevelManager.Instance.RPC_LoadLevelWithLoadingScreen(Application.loadedLevel + 1, fadeInTime, fadeOutTime);
			
			if (Application.loadedLevelName == "Level_3")
			{
				audio.PlayOneShot(elevatorDoor);
				elevator.animation.Play("close");
				elevator.GetComponent<TranslationYCycle>().enabled = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		characterCount--;
	}
}
