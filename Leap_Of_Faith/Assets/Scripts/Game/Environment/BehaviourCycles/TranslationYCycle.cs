using UnityEngine;
using System.Collections;

public class TranslationYCycle : MonoBehaviour 
{	
	public float speed = 0.0f;
	public float maxY = 0.0f;
	public float startTimeOffset = 0.0f;
	public float minMoveDistance = 0.0f;
	public bool isNetworked = false;

	private Transform myTransform = null;
	[System.NonSerialized]
	public bool movingUp = true;
	private float minY = 0.0f;
	private float distanceToMove = 0.0f;

	private float distanceY = 0.0f;
	private float lastNetworkSyncTime = 0.0f;
	private const float NETWORKSYNC_INTERVAL = 1.0f;

	void NetworkSync()
	{
		lastNetworkSyncTime = Time.time - startTimeOffset;

		myTransform.Translate(0.0f, minY - myTransform.position.y, 0.0f, Space.World);

		if (distanceY > 0.0f)
		{
			float distanceTotal = NetworkTime.Instance.Time * speed;
			int halfCycleCount = (int)(distanceTotal / distanceY);
			float distanceLeft = distanceTotal % distanceY;

			if (halfCycleCount % 2 == 0)
			{
				myTransform.Translate(0.0f, distanceLeft, 0.0f, Space.World);
				movingUp = true;
			}
			else
			{
				myTransform.Translate(0.0f, distanceY - distanceLeft, 0.0f, Space.World);
				movingUp = false;
			}
		}
	}

	// Use this for initialization
	void Start() 
	{
		myTransform = this.transform;
		distanceY = maxY;

		maxY += myTransform.position.y;
		minY = myTransform.position.y;

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
			if (movingUp)
			{
				if (myTransform.position.y + distanceToMove >= maxY)
				{
					movingUp = false;
					distanceToMove = maxY - myTransform.position.y;
				}
				myTransform.Translate(0.0f, distanceToMove, 0.0f, Space.World);
			}
			else
			{
				if (myTransform.position.y - distanceToMove <= minY)
				{
					movingUp = true;
					distanceToMove = myTransform.position.y - minY;
				}
				myTransform.Translate(0.0f, -distanceToMove, 0.0f, Space.World);
			}

			distanceToMove = 0.0f;
		}
	}
}
