using UnityEngine;
using System.Collections;

public class IndianSkillHandler : BaseSkillHandler 
{
	// booleans and vector calculation variables
	private bool skillIsEnabled = false;
	private float skillTimer = 0.0f;
	private float skillDuration = 5.0f;
	public GameObject instantiatedObject;
	private GameObject activeCreatedPlatform;
	
	private Ray ray;
	
	public bool funMode = false;
	
	public AudioClip powerUpSound;
	public AudioClip indianSkill;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// If power up is on cooldown
		if (powerUpOnCooldown)
		{
			// Ignore cooldown if funmode is enabled
			if (!funMode)
			{
				// Begin timer
				if (powerUpCooldownTimer >= powerUpCooldownLimit)
				{
					powerUpCooldownTimer = 0.0f;
					powerUpOnCooldown = false;
				}
				else
				{
					powerUpCooldownTimer += Time.deltaTime;
				}
			}
			else
			{
				powerUpCooldownTimer = 0.0f;
				powerUpOnCooldown = false;
			}
		}
		
		// Update ray to cast into game world where the mouse position is
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		// If player is the appropriate class (Indian), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 0 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			InputManager.skill &&
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			audio.PlayOneShot(powerUpSound);
			powerUpIsEnabled = true;
			
			//if using kinect
			if(InputManager.kinectActive)
			{
				InputManager.cursorActive = true;
				InputManager.autoClickOnce = true;
			}
		}
		
		if (powerUpIsEnabled)
		{
			// If powered up and player selects an area
			if (Input.GetMouseButtonDown(0) && !skillIsEnabled)
			{
				RaycastHit hit;
				
				// Enable the skill, get where the player clicked in world space,
				// clamp the distance to 5m (as per design), calculate target position
				audio.PlayOneShot(indianSkill);
				skillIsEnabled = true;
				
				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.name == "SkillCollider" || hit.collider.name == "canvas")
					{
						activeCreatedPlatform = (GameObject)Network.Instantiate(instantiatedObject, hit.point, Quaternion.identity, 0);
						if(InputManager.kinectActive)
						{	
							InputManager.cursorActive = false;
							InputManager.autoClickOnce = false;
						}
					}
				}
			}
			
			if (skillIsEnabled)
			{				
				skillTimer += Time.deltaTime;
				
				if (skillTimer >= skillDuration)
				{
					ResetSkill();
					GetComponent<JumpColliderCheck>().activeColliders.Remove(activeCreatedPlatform.collider);;
					Network.Destroy(activeCreatedPlatform);
				}
				
				return;
			}
			
			// Power up timer
			powerUpDurationTimer += Time.deltaTime;
			
			if (powerUpDurationTimer >= powerUpDuration)
			{
				powerUpIsEnabled = false;
				skillIsEnabled = false;
				powerUpDurationTimer = 0.0f;
				
				if(InputManager.kinectActive)
				{
					InputManager.cursorActive = false;
					InputManager.autoClickOnce = false;
				}
			}		
		}
	}
	
	void ResetSkill()
	{
		skillIsEnabled = false;
		powerUpIsEnabled = false;
		powerUpOnCooldown = true;
		powerUpDurationTimer = 0.0f;
		skillTimer = 0.0f;
	}
}
