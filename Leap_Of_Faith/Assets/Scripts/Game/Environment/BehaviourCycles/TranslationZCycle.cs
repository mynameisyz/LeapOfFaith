using UnityEngine;
using System.Collections;

public class TranslationZCycle : MonoBehaviour
{
	public float speed = 0.0f;
	public float maxZ = 0.0f;
	public float startTimeOffset = 0.0f;
	public float minMoveDistance = 0.0f;
	public bool isNetworked = false;

	private Transform myTransform = null;
	private bool movingForward = true;
	private float minZ = 0.0f;
	private float distanceToMove = 0.0f;

	private float distanceZ = 0.0f;
	private float lastNetworkSyncTime = 0.0f;
	private const float NETWORKSYNC_INTERVAL = 1.0f;

	void NetworkSync()
	{
		lastNetworkSyncTime = Time.time - startTimeOffset;

		myTransform.Translate(0.0f, 0.0f, minZ - myTransform.position.z, Space.World);

		if (distanceZ > 0.0f)
		{
			float distanceTotal = NetworkTime.Instance.Time * speed;
			int halfCycleCount = (int)(distanceTotal / distanceZ);
			float distanceLeft = distanceTotal % distanceZ;

			if (halfCycleCount % 2 == 0)
			{
				myTransform.Translate(0.0f, 0.0f, distanceLeft, Space.World);
				movingForward = true;
			}
			else
			{
				myTransform.Translate(0.0f, 0.0f, distanceZ - distanceLeft, Space.World);
				movingForward = false;
			}
		}
	}

	// Use this for initialization
	void Start()
	{
		myTransform = this.transform;
		distanceZ = maxZ;

		maxZ += myTransform.position.z;
		minZ = myTransform.position.z;

		if (isNetworked)
			NetworkSync();
	}

	// Update is called once per frame
	void Update()
	{
		if (isNetworked && Time.time > lastNetworkSyncTime + NETWORKSYNC_INTERVAL)
			NetworkSync();

		distanceToMove += speed * Time.deltaTime;

		if (distanceToMove >= minMoveDistance)
		{
			if (movingForward)
			{
				if (myTransform.position.z + distanceToMove >= maxZ)
				{
					movingForward = false;
					distanceToMove = maxZ - myTransform.position.z;
				}
				myTransform.Translate(0.0f, 0.0f, distanceToMove, Space.World);
			}
			else
			{
				if (myTransform.position.z - distanceToMove <= minZ)
				{
					movingForward = true;
					distanceToMove = myTransform.position.z - minZ;
				}
				myTransform.Translate(0.0f, 0.0f, -distanceToMove, Space.World);
			}

			distanceToMove = 0.0f;
		}
	}
}
