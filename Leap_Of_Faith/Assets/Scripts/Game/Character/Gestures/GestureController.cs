using UnityEngine;
using System.Collections;

public class GestureController : MonoBehaviour
{
	#region NetworkSync

	private class NetStateBuffer
	{
		public bool isOutdated = false;
		public bool isBufferOutdated = false;
		public bool isSent = false;

		public bool leftMouseDown = false;
		public bool rightMouseDown = false;
		public bool isLeftOfCharacter = false;

		public bool leftActive = false;
		public bool rightActive = false;
		public bool headActive = false;

		public Vector3 mouseWorldPos = Vector3.zero;
	}
	private NetStateBuffer netStateBuffer = new NetStateBuffer();

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			bool _isSendingNetStateBuffer = !netStateBuffer.isSent;
			stream.Serialize(ref _isSendingNetStateBuffer);

			if (_isSendingNetStateBuffer)
			{
				bool _leftMouseDown = netStateBuffer.leftMouseDown;
				stream.Serialize(ref _leftMouseDown);

				bool _rightMouseDown = netStateBuffer.rightMouseDown;
				stream.Serialize(ref _rightMouseDown);

				bool _isLeftOfCharacter = netStateBuffer.isLeftOfCharacter;
				stream.Serialize(ref _isLeftOfCharacter);

				bool _leftActive = netStateBuffer.leftActive;
				stream.Serialize(ref _leftActive);

				bool _rightActive = netStateBuffer.rightActive;
				stream.Serialize(ref _rightActive);

				bool _headActive = netStateBuffer.headActive;
				stream.Serialize(ref _headActive);

				Vector3 _mouseWorldPos = netStateBuffer.mouseWorldPos;
				stream.Serialize(ref _mouseWorldPos);

				netStateBuffer.isSent = true;
			}
			else
			{
				Vector3 _mouseWorldPos = mouseWorldPos;
				stream.Serialize(ref _mouseWorldPos);
			}

			if (netStateBuffer.isBufferOutdated)
			{
				netStateBuffer.isBufferOutdated = false;
				netStateBuffer.isOutdated = true;
			}
		}
		else
		{
			bool _isReceivingNetStateBuffer = false;
			stream.Serialize(ref _isReceivingNetStateBuffer);

			if (_isReceivingNetStateBuffer)
			{
				bool _leftMouseDown = false;
				stream.Serialize(ref _leftMouseDown);
				leftMouseDown = _leftMouseDown;

				bool _rightMouseDown = false;
				stream.Serialize(ref _rightMouseDown);
				rightMouseDown = _rightMouseDown;

				bool _isLeftOfCharacter = false;
				stream.Serialize(ref _isLeftOfCharacter);
				isLeftOfCharacter = _isLeftOfCharacter;

				bool _leftActive = false;
				stream.Serialize(ref _leftActive);
				leftActive = _leftActive;

				bool _rightActive = false;
				stream.Serialize(ref _rightActive);
				rightActive = _rightActive;

				bool _headActive = false;
				stream.Serialize(ref _headActive);
				headActive = _headActive;

				Vector3 _mouseWorldPos = Vector3.zero;
				stream.Serialize(ref _mouseWorldPos);
				mouseWorldPos = _mouseWorldPos;
			}
			else
			{
				Vector3 _mouseWorldPos = Vector3.zero;
				stream.Serialize(ref _mouseWorldPos);
				mouseWorldPos = _mouseWorldPos;
			}
		}
	}

	#endregion
	
	public Transform lTarget;
	public Transform lElbowTarget;
	public Transform rTarget;
	public Transform rElbowTarget;

	public Transform leftUpperArmPos;
	public Transform rightUpperArmPos;

	public Transform defaultLeftPosition;
	public Transform defaultRightPosition;
	public Transform defaultHeadPosition;

	public Transform head;
	public Transform headTarget;

	public Camera objCamera;
	public float smoothing = 0.0f;
	public float maxTargetDistance = 1.0f;
	public float minTargetToOrigin = 0.1f;
	
	public float elbowOffset = 0.1f;
	public float elbowOffsetFactor = 2.3f;

	private bool leftMouseDown = false;
	private bool rightMouseDown = false;
	private bool isLeftOfCharacter = false;
	
	private bool leftActive = false;
	private bool rightActive = false;
	private bool headActive = false;

	private Vector3 mouseScreenPos = Vector3.zero;
	private Vector3 mouseWorldPos = Vector3.zero;
	
	

	// Use this for initialization
	void Start()
	{
		leftMouseDown = false;
		rightMouseDown = false;
		objCamera = Camera.main;
	}

	void LateUpdate()
	{
		if (rightMouseDown)
		{
			leftActive = true;
			rightActive = true;

			if (isLeftOfCharacter)
			{
				lTarget.position += movementSmoothing(lTarget.position, clampTargetPosition(mouseWorldPos, leftUpperArmPos.position));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, true);

				//rTarget.position = mirrorPosition(lTarget, this.gameObject.transform);
				rTarget.position += movementSmoothing(rTarget.position, mirrorPosition(lTarget, this.gameObject.transform));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, false);
			}
			else
			{
				rTarget.position += movementSmoothing(rTarget.position, clampTargetPosition(mouseWorldPos, rightUpperArmPos.position));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, false);

				//lTarget.position = mirrorPosition(rTarget, this.gameObject.transform);
				lTarget.position += movementSmoothing(lTarget.position, mirrorPosition(rTarget, this.gameObject.transform));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, true);
			}
		}
		else if (leftMouseDown)
		{
			//update position of ik chain target and elbow target
			if (leftActive)
			{
				lTarget.position += movementSmoothing(lTarget.position, clampTargetPosition(mouseWorldPos, leftUpperArmPos.position));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, true);
				Vector3 difference = lTarget.position - leftUpperArmPos.position;
				if (difference.magnitude >= 0.4f)
				{
					headActive = true;
				}
				else
				{
					headActive = false;
				}
			}

			if (rightActive)
			{
				rTarget.position += movementSmoothing(rTarget.position, clampTargetPosition(mouseWorldPos, rightUpperArmPos.position));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, false);
				Vector3 difference = rTarget.position - rightUpperArmPos.position;
				if (difference.magnitude >= 0.4f)
				{
					headActive = true;
				}
				else
				{
					headActive = false;
				}
			}

			if (headActive)
			{
				//head.GetComponent<HeadGesturer>().active = true;				
				headTarget.position += movementSmoothing(headTarget.position, mouseWorldPos);
				head.LookAt(headTarget);
				head.Rotate(0, 90, -90);
			}
		}

		//return target to default positions
		if (!leftActive)
		{
			lTarget.position += movementSmoothing(lTarget.position, defaultLeftPosition.position);
			correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, true);
		}
		if (!rightActive)
		{
			rTarget.position += movementSmoothing(rTarget.position, defaultRightPosition.position);
			correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, false);
		}
		if (!headActive)
		{
			//head.GetComponent<HeadGesturer>().active = true;
			headTarget.position += movementSmoothing(headTarget.position, defaultHeadPosition.position);
			head.LookAt(headTarget);
			head.Rotate(0, 90, -90);
		}
		
		
		
		if (netStateBuffer.isOutdated)
		{
			netStateBuffer.isOutdated = false;
			netStateBuffer.isSent = false;

			netStateBuffer.leftMouseDown = leftMouseDown;
			netStateBuffer.rightMouseDown = rightMouseDown;
			netStateBuffer.isLeftOfCharacter = isLeftOfCharacter;

			netStateBuffer.leftActive = leftActive;
			netStateBuffer.rightActive = rightActive;
			netStateBuffer.headActive = headActive;

			netStateBuffer.mouseWorldPos = mouseWorldPos;
		}
	}

	// Update is called once per frame
	void Update()
	{
		
		
		// Get input for myself
		if (networkView.isMine)
		{
			if (Input.GetMouseButtonUp(0))
			{
				leftMouseDown = false;
				rightMouseDown = false;
				leftActive = false;
				rightActive = false;
				headActive = false;
				netStateBuffer.isBufferOutdated = true;
			}
			if (Input.GetMouseButtonUp(1))
			{
				leftMouseDown = false;
				rightMouseDown = false;
				leftActive = false;
				rightActive = false;
				headActive = false;
				netStateBuffer.isBufferOutdated = true;
			}

			//On mouse click left button
			if (Input.GetMouseButton(0))
			{
				
				//check position of the mouse click
				mouseScreenPos = Input.mousePosition;
				mouseWorldPos = objCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y,
																Mathf.Abs(objCamera.transform.position.z - this.gameObject.transform.position.z)));

				//mouseWorldPos.z += 5.0f / Mathf.Abs(this.gameObject.transform.position.x - mouseWorldPos.x);
				mouseWorldPos.z -= 0.2f;

				if (!leftMouseDown)
				{
					leftMouseDown = true;
					netStateBuffer.isOutdated = true;

					Ray ray;
					RaycastHit hit;
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					if(Physics.Raycast(ray, out hit, 100.0f))
					{
						if (hit.collider.gameObject.name == "HeadCollider" && hit.collider.transform.root.gameObject.networkView.isMine)
						{
							headActive = true;
						}
					}
					
					if(!headActive)
					{
						//check X position of mouse click to the character position
						Vector3 difference = mouseWorldPos - this.gameObject.transform.position;
						if ((difference.x) >= 0)
						{
							//left
							leftActive = true;
						}
						else
						{
							//right
							rightActive = true;
						}
					}
				}
			}

			//On mouse click right button mirror mode
			if (Input.GetMouseButton(1))
			{				
				//check position of the mouse click
				mouseScreenPos = Input.mousePosition;
				mouseWorldPos = objCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(objCamera.transform.position.z - this.gameObject.transform.position.z)));
				mouseWorldPos.z -= 0.2f;

				if (!rightMouseDown)
				{
					rightMouseDown = true;
					netStateBuffer.isOutdated = true;

					//check X position of mouse click to the character position
					if ((mouseWorldPos.x - this.gameObject.transform.position.x) >= 0)
					{
						//left
						isLeftOfCharacter = true;
					}
					else
					{
						///right
						isLeftOfCharacter = false;
					}
					
					headActive = false;
				}
			}
		}
		
		/*
		if (rightMouseDown)
		{
			leftActive = true;
			rightActive = true;

			if (isLeftOfCharacter)
			{
				lTarget.position += movementSmoothing(lTarget.position, clampTargetPosition(mouseWorldPos, leftUpperArmPos.position));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, elbowOffset, true);

				//rTarget.position = mirrorPosition(lTarget, this.gameObject.transform);
				rTarget.position += movementSmoothing(rTarget.position, mirrorPosition(lTarget, this.gameObject.transform));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, elbowOffset, false);
			}
			else
			{
				rTarget.position += movementSmoothing(rTarget.position, clampTargetPosition(mouseWorldPos, rightUpperArmPos.position));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, elbowOffset, false);

				//lTarget.position = mirrorPosition(rTarget, this.gameObject.transform);
				lTarget.position += movementSmoothing(lTarget.position, mirrorPosition(rTarget, this.gameObject.transform));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, elbowOffset, true);
			}
		}
		else if (leftMouseDown)
		{
			//update position of ik chain target and elbow target
			if (leftActive)
			{
				lTarget.position += movementSmoothing(lTarget.position, clampTargetPosition(mouseWorldPos, leftUpperArmPos.position));
				correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, elbowOffset, true);
				Vector3 difference = lTarget.position - leftUpperArmPos.position;
				if (difference.magnitude >= 0.4f)
				{
					headActive = true;
				}
				else
				{
					headActive = false;
				}
			}

			if (rightActive)
			{
				rTarget.position += movementSmoothing(rTarget.position, clampTargetPosition(mouseWorldPos, rightUpperArmPos.position));
				correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, elbowOffset, false);
				Vector3 difference = rTarget.position - rightUpperArmPos.position;
				if (difference.magnitude >= 0.4f)
				{
					headActive = true;
				}
				else
				{
					headActive = false;
				}
			}

			if (headActive)
			{
				headTarget.position += movementSmoothing(headTarget.position, mouseWorldPos);
				//head.LookAt(headTarget);
				//head.Rotate(0, -90, -90);
			}
		}

		//return target to default positions
		if (!leftActive)
		{
			lTarget.position += movementSmoothing(lTarget.position, defaultLeftPosition.position);
			correctElbowPosition(leftUpperArmPos.localPosition, lTarget.localPosition, lElbowTarget, elbowOffset, true);
		}
		if (!rightActive)
		{
			rTarget.position += movementSmoothing(rTarget.position, defaultRightPosition.position);
			correctElbowPosition(rightUpperArmPos.localPosition, rTarget.localPosition, rElbowTarget, elbowOffset, false);
		}
		if (!headActive)
		{
			headTarget.position += movementSmoothing(headTarget.position, defaultHeadPosition.position);
			//head.LookAt(headTarget);
			//head.Rotate(0, -90, -90);
		}
		*/
	}

	Vector3 movementSmoothing(Vector3 targetPos, Vector3 finalPos)
	{
		Vector3 difference = finalPos - targetPos;
		if (difference.magnitude > smoothing)
		{
			return difference * smoothing;
		}
		else
		{
			//return Vector3.zero;
			return difference;
		}
	}

	void correctElbowPosition(Vector3 upperArmPos, Vector3 targetPos, Transform elbowTarget, bool isLeft)
	{
		Vector2 difference = new Vector2(upperArmPos.x, upperArmPos.y) -
											new Vector2(targetPos.x, targetPos.y);
		/*
		float angle;
		if (isLeft)
		{
			angle = Mathf.Abs(Mathf.Atan2(difference.y, difference.x));
		}
		else
		{
			angle = Mathf.Abs(Mathf.Atan2(-difference.y, -difference.x));
		}
		
		Vector3 position = elbowTarget.localPosition;
		position.z = angle * offset;
		elbowTarget.localPosition = position;
		*/
		
		
		Vector3 position = elbowTarget.localPosition;
		position.z = elbowOffsetFactor*difference.magnitude - elbowOffset;
		elbowTarget.localPosition = position;
	
		

	}

	Vector3 mirrorPosition(Transform target, Transform midPoint)
	{
		Vector3 tempVector = midPoint.position - target.position;
		tempVector.y *= -1;
		tempVector.z *= -1;
		return tempVector + midPoint.position;

	}

	Vector3 clampTargetPosition(Vector3 targetPos, Vector3 origin)
	{
		Vector3 vector = targetPos - origin;
		if (vector.magnitude >= maxTargetDistance)
		{
			vector = Vector3.ClampMagnitude(vector, maxTargetDistance);
			return origin + vector;
		}
		else
		{
			return targetPos;
		}
	}
}
