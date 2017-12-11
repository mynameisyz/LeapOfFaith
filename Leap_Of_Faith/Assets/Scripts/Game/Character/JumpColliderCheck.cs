using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpColliderCheck : MonoBehaviour
{
	private Transform myTransform;
	public List<Collider> activeColliders = new List<Collider>();

	public float posXOffset = 0.5f;
	public float posYOffset = 0.25f;

	private Vector3 rayStartPosLeft = Vector3.zero;
	private Vector3 rayStartPosRight = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		myTransform = transform;
	}

	// Update is called once per frame
	void Update()
	{
		// Prepare raycasthit to store info
		RaycastHit hit;

		// Sets up the ray
		SetRayPosition();

		if (Physics.Raycast(rayStartPosLeft, myTransform.TransformDirection(Vector3.up), out hit, 1.5f) ||
			Physics.Raycast(rayStartPosRight, myTransform.TransformDirection(Vector3.up), out hit, 1.5f))
		{
			/* 	If something is above the player, turn off its collider
				and store it in a temp variable so that it can be accessed
				in future if the raycast accidentally collides with another object 	*/
			if (GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostRed", hit.transform))
			{
				if (this.gameObject == PlayerData.characters[PlayerData.PLAYER_RED])
					AddActiveCollider(hit.collider);
			}
			else if (GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostBlue", hit.transform))
			{
				if (this.gameObject == PlayerData.characters[PlayerData.PLAYER_BLUE])
					AddActiveCollider(hit.collider);
			}
			else
			{
				if (hit.collider.tag != "DeathOnTouch")
					AddActiveCollider(hit.collider);
			}
		}
		else
		{
			// Enable the previous collider if raycast is not colliding with anything
			// which means nothing is above the player
			if (activeColliders.Count > 0)
			{
				foreach (Collider col in activeColliders)
				{
					if (col != null)
						Physics.IgnoreCollision(this.gameObject.collider, col, false);
				}
				activeColliders.Clear();
			}
		}
	}

	private bool AddActiveCollider(Collider _collider)
	{
		if (activeColliders.Contains(_collider))
			return false;

		Physics.IgnoreCollision(this.gameObject.collider, _collider, true);
		activeColliders.Add(_collider);

		return true;
	}

	void SetRayPosition()
	{
		// Store the player's position and offset it by its width/2 to raycast from the feet up
		rayStartPosLeft = myTransform.position;
		rayStartPosRight = myTransform.position;

		rayStartPosLeft.y = rayStartPosLeft.y - posYOffset;
		rayStartPosLeft.x = rayStartPosLeft.x - posXOffset;

		rayStartPosRight.y = rayStartPosRight.y - posYOffset;
		rayStartPosRight.x = rayStartPosRight.x + posXOffset;
	}
}
