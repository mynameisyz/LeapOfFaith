using UnityEngine;
using System.Collections;

public class PlatformerController : MonoBehaviour
{
	#region Network Sync

	// Whether the Update() is actual, or just a calculation
	private float deltaTime = 0.0f;
	private bool isCalculatingNetTimeOffset = false;

	// Offset position over time
	private Vector3 netOwnerPos = Vector3.zero;
	private Vector3 netClientPos = Vector3.zero;
	private Vector3 netPosOffset = Vector3.zero;

	private const float NETPOS_SMOOTHTIME = 0.1f;
	private float netSmoothTimeLeft = 0.0f;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			bool _jumpButtonDown = jumpButtonDown;
			stream.Serialize(ref _jumpButtonDown);
			if (jumpButtonDown)
				jumpButtonDown = false;

			bool _jumpButton = jumpButton;
			stream.Serialize(ref _jumpButton);

			float _h = (float)h;
			stream.Serialize(ref _h);
			
			if (!_jumpButtonDown)
			{
				Vector3 _position = this.gameObject.transform.position;
				stream.Serialize(ref _position);
				
				float _speed = (float)speed;
				stream.Serialize(ref _speed);
				
				float _verticalSpeed = (float)verticalSpeed;
				stream.Serialize(ref _verticalSpeed);
				
				Vector3 _direction = direction;
				stream.Serialize(ref _direction);
				
				Vector3 _velocity = velocity;
				stream.Serialize(ref _velocity);
				
				Vector3 _inAirVelocity = inAirVelocity;
				stream.Serialize(ref _inAirVelocity);
			}
		}
		else
		{
			bool _jumpButtonDown = false;
			stream.Serialize(ref _jumpButtonDown);
			jumpButtonDown = _jumpButtonDown;

			bool _jumpButton = false;
			stream.Serialize(ref _jumpButton);
			jumpButton = _jumpButton;

			float _h = 0.0f;
			stream.Serialize(ref _h);
			h = _h;

			if (!jumpButtonDown)
			{
				netClientPos = this.gameObject.transform.position;
				
				Vector3 _position = Vector3.zero;
				stream.Serialize(ref _position);
				this.gameObject.transform.position = _position;
				
				float _speed = 0.0f;
				stream.Serialize(ref _speed);
				speed = _speed;
				
				float _verticalSpeed = 0.0f;
				stream.Serialize(ref _verticalSpeed);
				verticalSpeed = _verticalSpeed;
				
				Vector3 _direction = Vector3.zero;
				stream.Serialize(ref _direction);
				direction = _direction;
				
				Vector3 _velocity = Vector3.zero;
				stream.Serialize(ref _velocity);
				velocity = _velocity;
				
				Vector3 _inAirVelocity = Vector3.zero;
				stream.Serialize(ref _inAirVelocity);
				inAirVelocity = _inAirVelocity;

				deltaTime = (float)(Network.time - info.timestamp);
				isCalculatingNetTimeOffset = true;
				Update();

				if (this.gameObject.transform.position != netOwnerPos)
				{
					netOwnerPos = this.gameObject.transform.position;
					netPosOffset = netOwnerPos - netClientPos;
					netSmoothTimeLeft = NETPOS_SMOOTHTIME;
					this.gameObject.transform.position = netClientPos;
				}
			}
		}
	}

	void smoothOffsetNetPos()
	{
		if (netSmoothTimeLeft > 0.0f)
		{
			if (netSmoothTimeLeft >= deltaTime)
				this.gameObject.transform.position += netPosOffset * (deltaTime / NETPOS_SMOOTHTIME);
			else
				this.gameObject.transform.position += netPosOffset * (netSmoothTimeLeft / NETPOS_SMOOTHTIME);

			netSmoothTimeLeft -= deltaTime;
		}
	}

	#endregion

	//------------------PlayerInput Variables-----------------

	float h = 0.0f;
	bool jumpButtonDown = false;
	bool jumpButton = false;
	
	//------------------PlayerMovement Variables-----------------
	
	// The speed when walking 
	public double walkSpeed = 4.0;
	
	// when pressing "Fire2" button (control) we start running
	// double runSpeed = 10.0;

	double inAirControlAcceleration = 1.0;

	// The gravity for the character
	double gravity = 60.0;
	double maxFallSpeed = 20.0;

	// How fast does the character change speeds?  Higher is faster.
	double speedSmoothing = 10.0;

	// This controls how fast the graphics of the character "turn around" when the player turns around using the controls.
	double rotationSmoothing = 10.0;

	// The current move direction in x-y.  This will always been (1,0,0) or (-1,0,0)
	Vector3 direction = Vector3.zero;

	// The current speed. This gets smoothed by speedSmoothing.
	double verticalSpeed = 0.0;
	double speed = 0.0;

	// Is the user pressing the left or right movement keys?
	bool isMoving = false;

	// The last collision flags returned from controller.Move
	CollisionFlags collisionFlags;

	// We will keep track of an approximation of the character's current velocity, so that we return it from GetVelocity() for our camera to use for prediction.
	Vector3 velocity;
	
	// This keeps track of our current velocity while we're not grounded?
	Vector3 inAirVelocity = Vector3.zero;

	// This will keep track of how long we have we been in the air (not grounded)
	double hangTime = 0.0;
	
	//-----------------Jumping Variables----------------------------------------------------
	
	//******Exposed*******
	// Can the character jump?
	public bool jumpEnabled = true;
	
	// How high do we jump when pressing jump and letting go immediately
	public double height = 1.0;
	
	// We add extraHeight units on top when holding the button down longer while jumping
	public double extraHeight = 1.0;
	
	//************************
	
	// This prevents inordinarily too quick jumping
	double repeatTime = 0.05;
	double timeout = 0.15;

	// Are we jumping? (Initiated with jump button and not grounded yet)
	bool jumping = false;
	bool reachedApex = false;
  
	// Last time the jump button was clicked down
	double lastButtonTime = -10.0;
	
	// Last time we performed a jump
	double lastTime = -1.0;

	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	double lastStartHeight = 0.0;
	
	//-----------------Other variables-------------

	private bool isMine = false;
	private bool lockZaxis = true;
	private float zAxisValue = 0.0f;

	private CharacterController controller;

	private GameObject currentSpawnPoint;
	public int currentSpawnPointIndex;
	
	private Transform activePlatform;
	private Vector3 activeLocalPlatformPoint;
	private Vector3 activeGlobalPlatformPoint;
	private Vector3 lastPlatformVelocity;
	
	public AudioClip jumping_sound;
	
	//mesh to play animation from
	public GameObject mesh;
	
	//-------------------Functions-----------------
	void Awake()
	{
		direction = transform.TransformDirection(Vector3.forward);
		controller = this.GetComponent<CharacterController>();

		isMine = networkView.isMine;
	}
	
	// Use this for initialization
	void Start() 
	{
		verticalSpeed = 0.0;
		speed = 0.0;
	}

	void UpdateSmoothedMovementDirection()
	{	
		if (isMine)
			//h = Input.GetAxisRaw("Horizontal");
			h = InputManager.horizontal; 
			
		isMoving = Mathf.Abs(h) > 0.1;
			
		if (isMoving)
		{
			direction = new Vector3(h, 0, 0);
			if(controller.isGrounded)
			{
				mesh.animation.CrossFade("walk");
			}
		}
		else
		{
			direction = new Vector3(0, 0, -1);
			if(controller.isGrounded)
			{
				mesh.animation.CrossFade("idle");
			}
		}
		
		// Grounded controls
		if (controller.isGrounded) 
		{		
			// Smooth the speed based on the current target direction
			float curSmooth = (float)speedSmoothing * deltaTime;
			
			// Choose target speed
			float targetSpeed = Mathf.Min(Mathf.Abs(h), (float)1.0);
		
			// Pick speed modifier
			//if (Input.GetButton("Fire2") && isMine)
			//	targetSpeed *= (float)runSpeed;
			//else

			targetSpeed *= (float)walkSpeed;
			
			speed = Mathf.Lerp((float)speed, targetSpeed, curSmooth);
			
			hangTime = 0.0;
		}
		else 
		{
			// In air controls
			hangTime += deltaTime;
			
			if (isMoving)
				inAirVelocity += (new Vector3(Mathf.Sign(h), 0, 0) * deltaTime * (float)inAirControlAcceleration);
		}
	}
	
	void FixedUpdate() 
	{
		if (lockZaxis)
		{
			// Make sure we are absolutely always in the 2D plane.
			Vector3 pos = this.transform.position;
    		pos.z = zAxisValue;
    		this.transform.position = pos;
		}
	}
	
	public void ApplyJumping() 
	{
		if (jumpEnabled)
		{
			// Prevent jumping too fast after each other
			if (lastTime + repeatTime > Time.time)
				return;
		
			if (controller.isGrounded) 
			{
				// Jump
				// - Only when pressing the button down
				// - With a timeout so you can press the button slightly before landing		
				if (Time.time < lastButtonTime + timeout) 
				{
					verticalSpeed = (double)CalculateJumpVerticalSpeed((float)height);
					inAirVelocity = lastPlatformVelocity;
					
					audio.pitch = Random.Range(0.9f, 1.1f);
					audio.PlayOneShot(jumping_sound);
					
					mesh.animation.CrossFade("jump");
					
					//TO-DO: Change to direct call
					DidJump();
				}
			}
		}
	}

	void ApplyGravity() 
	{
		if (isMine)
			//jumpButton = Input.GetButton("Jump");
			jumpButton = (InputManager.jump >= 1);

		// When we reach the apex of the jump we send out a message
		if (jumping && !reachedApex && verticalSpeed <= 0.0)
		{
			reachedApex = true;
		//	SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
		}
		
		// * When jumping up we don't apply gravity for some time when the user is holding the jump button
		//   This gives more control over jump height by pressing the button longer
		bool extraPowerJump =  jumping && verticalSpeed > 0.0 && jumpButton && transform.position.y < lastStartHeight + extraHeight && !IsTouchingCeiling ();
		
		if (extraPowerJump)
			return;
		else if (controller.isGrounded)
			verticalSpeed = -gravity * deltaTime;
		else
			verticalSpeed -= gravity * deltaTime;
			
		// Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
		verticalSpeed = (double)Mathf.Max((float)verticalSpeed, (float)(-maxFallSpeed));
	}
		
	float CalculateJumpVerticalSpeed(float targetJumpHeight) 
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2.0f * targetJumpHeight * (float)gravity);
	}
	
	void DidJump() 
	{
		jumping = true;
		reachedApex = false;
		lastTime = Time.time;
		lastStartHeight = transform.position.y;
		lastButtonTime = -10.0;
	}

	void Update() 
	{
		if (!isCalculatingNetTimeOffset)
			deltaTime = Time.deltaTime;

		// Jump button input
		if (isMine)
		{
			if (InputManager.jump >= 1)
			{
				lastButtonTime = Time.time;
				jumpButtonDown = true;
			}
		}
		else
		{
			if (jumpButtonDown)
				lastButtonTime = Time.time;
		}

		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		ApplyGravity();
	
		// Apply jumping logic
		ApplyJumping();
		
		// Moving platform support
		if (activePlatform != null) 
		{
			Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
			Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
			transform.position = transform.position + moveDistance;
			lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / deltaTime;
		} 
		else 
		{
			lastPlatformVelocity = Vector3.zero;	
		}
		
		activePlatform = null;
		
		// Save lastPosition for velocity calculation.
		Vector3 lastPosition = transform.position;
		
		// Calculate actual motion
		Vector3 currentMovementOffset = direction * (float)speed + new Vector3(0, (float)verticalSpeed, 0) + inAirVelocity;
		
		// We always want the movement to be framerate independent.  Multiplying by deltaTime does this.
		currentMovementOffset *= deltaTime;

		// Bound it to the camera screen area
		if (this.collider.bounds.min.x <= CameraBehaviour.GetWorldBoundX_Min() &&
			currentMovementOffset.x < 0.0f)
			currentMovementOffset.x = 0.0f;

		if (this.collider.bounds.max.x >= CameraBehaviour.GetWorldBoundX_Max() &&
			currentMovementOffset.x > 0.0f)
			currentMovementOffset.x = 0.0f;
		
	   	// Move our character!
		collisionFlags = controller.Move(currentMovementOffset);

		// Calculate the velocity based on the current and previous position.  
		// This means our velocity will only be the amount the character actually moved as a result of collisions.
		velocity = (transform.position - lastPosition) / deltaTime;
		
		// Moving platforms support
		if (activePlatform != null) 
		{
			activeGlobalPlatformPoint = transform.position;
			activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
		}
		
		// Set rotation to the move direction	
		if (direction.sqrMagnitude > 0.01)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-direction), deltaTime * (float)rotationSmoothing);
		
		// We are in jump mode but just became grounded
		if (controller.isGrounded) 
		{
			inAirVelocity = Vector3.zero;
			if (jumping) 
			{
				jumping = false;
	
				Vector3 jumpMoveDirection = direction * (float)speed + inAirVelocity;
				if (jumpMoveDirection.sqrMagnitude > 0.01)
					direction = jumpMoveDirection.normalized;
			}
		}	
	
		// Update special effects like rocket pack particle effects
		//UpdateEffects ();

		if (isCalculatingNetTimeOffset)
			isCalculatingNetTimeOffset = false;
		else
			smoothOffsetNetPos();
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.moveDirection.y > 0.01) 
			return;
		
		// Make sure we are really standing on a straight platform
		// Not on the underside of one and not falling down from it either!
		if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.9)
		{
			activePlatform = hit.collider.transform;	
		}
	}
		
	// Various helper functions below:
	
	public bool IsTouchingCeiling() 
	{
		return (collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	
	[RPC]
	public void ChangeSpawnPoint(int spawnPointIndex)
	{
		currentSpawnPoint = LevelData.Instance.checkpoints[spawnPointIndex];

		currentSpawnPointIndex = spawnPointIndex;
	}
	
	public void RPC_ChangeSpawnPoint(int spawnPointIndex)
	{
		if (networkView.isMine)
			networkView.RPC("ChangeSpawnPoint", RPCMode.All, spawnPointIndex);
	}
	
	[RPC]
	public void Respawn() 
	{
		// reset the character's speed
		this.verticalSpeed = 0.0;
		this.speed = 0.0;

		lastButtonTime = -10.0;
		jumpButtonDown = false;

		// reset network position offset
		netPosOffset = Vector3.zero;
		
		// reset the character's position to the spawnPoint
		this.transform.position = currentSpawnPoint.transform.position;
		this.transform.Translate(0, 2.0f, 0);
	}

	public void RPC_Respawn()
	{
		if (networkView.isMine)
			networkView.RPC("Respawn", RPCMode.All);
	}
}


	
	
	


