using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour 
{
	private Vector3 endRotation = Vector3.zero;
	private Vector3 deltaRotation = Vector3.zero;
	private float timeLeft = 0.0f;

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (timeLeft > 0.0f)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0.0f)
			{
				transform.eulerAngles = endRotation;
			}
			else
			{
				transform.Rotate(deltaRotation * Time.deltaTime);
			}
		}
	}

	public void RotateTo(Vector3 targetRotation, float time)
	{
		deltaRotation = targetRotation - this.gameObject.transform.eulerAngles;
		endRotation = targetRotation;
		timeLeft = time;
	}
}
