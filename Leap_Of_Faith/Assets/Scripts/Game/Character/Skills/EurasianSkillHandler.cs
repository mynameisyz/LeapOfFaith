using UnityEngine;
using System.Collections;

public class EurasianSkillHandler : BaseSkillHandler
{
	#region Malay and Chinese specific skill variables

	// Character controllers that control the character's movement
	PlatformerController myPlatformController;
	
	// Cache my transform
	private Transform myTransform;

	// Malay skill variables
	private Vector3 skillTargetPosition;
	public float powerUpDistanceToTravel = 5.0f;
	private Vector3 dir;
	private Vector3 clampTargetPosition;
	
	public float powerMultiply = 1.5f;
	
	#endregion

	#region Indian specfic skill variables

	public GameObject instantiatedObject;
	private GameObject activeCreatedPlatform;

	#endregion

	// booleans and vector calculation variables
	private bool skillIsEnabled = false;
	private float skillTimer = 0.0f;
	private float skillDuration = 5.0f;
	private Ray ray;

	// It means what it says
	public bool funMode = false;
	public AudioClip powerUpSound;
	public AudioClip indian_skill;
	public AudioClip malay_skill;
	
	// ID of the other player's class
	private int otherPlayerClassID = -1;

	// Use this for initialization
	void Start()
	{
		myTransform = this.transform;
		myPlatformController = myTransform.GetComponent<PlatformerController>();
		otherPlayerClassID = PlayerData.classId[PlayerData.peerColor];
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
		
		switch (otherPlayerClassID)
		{
			case PlayerData.CHARACTERCLASS_INDIAN:
				MimicIndianSkill();
				break;

			case PlayerData.CHARACTERCLASS_CHINESE:
				MimicChineseSkill();
				break;

			case PlayerData.CHARACTERCLASS_MALAY:
				MimicMalaySkill();
				break;
		}
	}

	void MimicMalaySkill()
	{
		// Update ray to cast into game world where the mouse position is
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// If player is the appropriate class (malay), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 3 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			InputManager.skill &&
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			Debug.Log("SKILL ACTIVATED");

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
				audio.PlayOneShot(malay_skill);
				skillIsEnabled = true;

				RaycastHit hit;

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider.name == "SkillCollider" || hit.collider.name == "canvas")
					{
						skillTargetPosition = hit.point;
					
						if(InputManager.kinectActive)
						{
							InputManager.cursorActive = false;
						}
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
					skillIsEnabled = false;
					powerUpIsEnabled = false;
					myPlatformController.enabled = true;
					powerUpOnCooldown = true;
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

	void MimicIndianSkill()
	{
		// Update ray to cast into game world where the mouse position is
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		// If player is the appropriate class (Eurasian), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 3 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			//(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && 
			InputManager.skill &&
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			Debug.Log("SKILL ACTIVATED");
			
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
				audio.PlayOneShot(indian_skill);
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
					skillIsEnabled = false;
					powerUpIsEnabled = false;
					powerUpOnCooldown = true;
					powerUpDurationTimer = 0.0f;
					skillTimer = 0.0f;
					
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

	void MimicChineseSkill()
	{
		// If player is the appropriate class (chinese), if press shift, and power up is not already
		// enabled and not on cooldown, turn on for 5 seconds or until player uses skill
		if (PlayerData.classId[PlayerData.color] == 3 &&
			Application.loadedLevel > (int)LevelManager.Scene.Level_1 &&
			InputManager.skill && 
			!powerUpIsEnabled && !powerUpOnCooldown)
		{
			
			Debug.Log("SKILL ACTIVATED");
			
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
				powerUpIsEnabled = false;
				powerUpDurationTimer = 0.0f;
				powerUpOnCooldown = true;
				myPlatformController.height = 2.0;
				myPlatformController.extraHeight = 1.0;
				myPlatformController.walkSpeed = 4.0;
			}		
		}
	}
}
