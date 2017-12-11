using UnityEngine;
using System.Collections;

public class CloudLightningEmitter : MonoBehaviour
{
	public GameObject LIGHTNING_OBJ = null;
	public float FLASHTIME = 0.0f;

	public float INTERVAL_MIN = 0.0f;
	public float INTERVAL_MAX = 0.0f;

	public float INTENSITY_MIN = 0.0f;
	public float INTENSITY_MAX = 0.0f;

	public int FLASHCOUNT_MIN = 0;
	public int FLASHCOUNT_MAX = 0;

	private float nextLightningTime = 0.0f;
	private bool isDoingLightning = false;
	private float flashTimeLeft = 0.0f;
	private float intensity = 0.0f;
	
	public AudioClip lightning;

	// Use this for initialization
	void Start() 
	{
		nextLightningTime = Time.time + Random.Range(INTERVAL_MIN, INTERVAL_MAX);
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (isDoingLightning)
		{
			flashTimeLeft -= Time.deltaTime;
			if (flashTimeLeft <= 0.0f)
			{
				flashTimeLeft = 0.0f;
				isDoingLightning = false;
				nextLightningTime = Time.time + Random.Range(INTERVAL_MIN, INTERVAL_MAX);
			}

			float value = ((FLASHTIME - (flashTimeLeft % FLASHTIME)) / FLASHTIME) * 2;
			if (value < 1.0f)
			{
				LIGHTNING_OBJ.light.intensity = value * intensity;
			}
			else
			{
				value -= 1.0f;
				LIGHTNING_OBJ.light.intensity = (1.0f - value) * intensity;
			}
		}
		else
		{
			if (Time.time >= nextLightningTime)
			{
				isDoingLightning = true;
				flashTimeLeft = FLASHTIME * Random.Range(FLASHCOUNT_MIN, FLASHCOUNT_MAX);
				intensity = Random.Range(INTENSITY_MIN, INTENSITY_MAX);
				audio.PlayOneShot(lightning);
			}
		}
	}
}
