using UnityEngine;
using System.Collections;

public class FinishEventElevatorUp : MonoBehaviour
{
	public float speedY = 0.0f;
	private bool isStarted = false;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (isStarted)
			this.gameObject.transform.Translate(0.0f, speedY * Time.deltaTime, 0.0f);
	}

	private void OnFinishPointTriggered()
	{
		isStarted = true;
	}
}
