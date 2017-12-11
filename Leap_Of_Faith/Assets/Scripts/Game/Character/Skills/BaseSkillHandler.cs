using UnityEngine;
using System.Collections;

public class BaseSkillHandler : MonoBehaviour 
{
	// Power up variables
	public float powerUpDuration = 5.0f;
	public float powerUpRunSpeed = 10.0f;
	public float powerUpCooldownLimit = 2.0f;

	[System.NonSerialized]
	public float powerUpDurationTimer = 0.0f;
	[System.NonSerialized]
	public bool powerUpIsEnabled = false;
	[System.NonSerialized]
	public bool powerUpOnCooldown = false;
	[System.NonSerialized]
	public float powerUpCooldownTimer = 0.0f;

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	public float CooldownValue
	{
		get { return 1.0f - (powerUpCooldownTimer / powerUpCooldownLimit); }
	}
}
