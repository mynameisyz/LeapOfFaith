using UnityEngine;
using System.Collections;

public class ChineseSkillHandler : BaseSkillHandler 
{
	public float powerMultiply = 1.5f;
	// Character controllers that control the character's movement
	PlatformerController myPlatformController;
	
	// Cache my transform
	private Transform myTransform;
	
	// It means what it says
	public bool funMode = false;
	
	public AudioClip powerUpSound;
	
	// Use this for initialization
	void Start() 
	{
		myTransform = this.transform;
		myPlatformController = myTransform.GetComponent<PlatformerController>();
	}
	
	// Update is called once per frame
	void Update() 
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
		
		// If player is the appropriate class (chinese), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 1 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			InputManager.skill && 
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			audio.PlayOneShot(powerUpSound);
			powerUpIsEnabled = true;
			myPlatformController.height = myPlatformController.height * powerMultiply;
			myPlatformController.extraHeight = myPlatformController.extraHeight * powerMultiply;
			myPlatformController.walkSpeed = myPlatformController.walkSpeed * powerMultiply;
		}
		
		if (powerUpIsEnabled)
		{			
			// Power up timer
			powerUpDurationTimer += Time.deltaTime;
			
			if (powerUpDurationTimer >= powerUpDuration)
			{
				ResetSkill();
			}		
		}
	}
	
	//void OnControllerColliderHit(ControllerColliderHit hit)
	//{
		// If during the skill the player hits a solid object, reset the skill and make the player fall
		//if (powerUpIsEnabled && hit.collider.tag == "SolidNeutral")
		//{
		//	ResetSkill();
		//}
	//}
	
	void ResetSkill()
	{
		powerUpIsEnabled = false;
		powerUpDurationTimer = 0.0f;
		powerUpOnCooldown = true;
		myPlatformController.height = 2.0;
		myPlatformController.extraHeight = 1.0;
		myPlatformController.walkSpeed = 4.0;
	}
}