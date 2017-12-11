using UnityEngine;
using System.Collections;

public class MalaySkillHandler : BaseSkillHandler
{
	PlatformerController myPlatformController;

	// booleans and vector calculation variables
	private bool skillIsEnabled = false;
	private Vector3 skillTargetPosition;
	public float powerUpDistanceToTravel = 5.0f;
	private Vector3 dir;
	private Vector3 clampTargetPosition;

	private Ray ray;
	private Transform myTransform;

	public bool funMode = false;

	public AudioClip powerUpSound;
	public AudioClip malaySkill;
	
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

		// Update ray to cast into game world where the mouse position is
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// If player is the appropriate class (malay), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 2 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			InputManager.skill &&
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			audio.PlayOneShot(powerUpSound);
			powerUpIsEnabled = true;
			
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
				// Enable the skill, get where the player clicked in world space,
				// clamp the distance to 5m (as per design), calculate target position
				audio.PlayOneShot(malaySkill);
				skillIsEnabled = true;

				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.name == "SkillCollider" || hit.collider.name == "canvas")
					{
						if(InputManager.kinectActive)
						{
							InputManager.cursorActive = false;
						}
						skillTargetPosition = hit.point;
						}
				}

				// Normalize the direction to 1 unit, multiply by 5 metres
				dir = (skillTargetPosition - myTransform.position).normalized;
				dir = dir * powerUpDistanceToTravel;
				clampTargetPosition = myTransform.position + dir;
			}

			if (skillIsEnabled)
			{
				// Disable movement during skill
				myPlatformController.enabled = false;

				// Move the player
				myTransform.position = Vector3.Lerp(myTransform.position, clampTargetPosition, Time.deltaTime * powerUpRunSpeed);

				// IF REACHED DESTINATION, SKILL_IS ENABLED = FALSE
				// ELSE RETURN, RESET ALL VARIABLES AND EXIT
				if (Vector3.Distance(myTransform.position, clampTargetPosition) <= 0.2f)
				{
					ResetSkill();
					
					
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
				}
			}
		}
	}

	void ResetSkill()
	{
		skillIsEnabled = false;
		powerUpIsEnabled = false;
		myPlatformController.enabled = true;
		powerUpOnCooldown = true;
	}
}
