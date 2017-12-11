using UnityEngine;
using System.Collections;

public class TranslationXCycle : MonoBehaviour
{
	public float speed = 0.0f;
	public float maxX = 0.0f;
	public float startTimeOffset = 0.0f;
	public float minMoveDistance = 0.0f;
	public bool isNetworked = false;

	private Transform myTransform = null;
	private bool movingRight = true;
	private float minX = 0.0f;
	private float distanceToMove = 0.0f;

	private float distanceX = 0.0f;
	private float lastNetworkSyncTime = 0.0f;
	private const float NETWORKSYNC_INTERVAL = 1.0f;

	void NetworkSync()
	{
		lastNetworkSyncTime = Time.time - startTimeOffset;

		myTransform.Translate(minX - myTransform.position.x, 0.0f, 0.0f, Space.World);

		if (distanceX > 0.0f)
		{
			float distanceTotal = NetworkTime.Instance.Time * speed;
			int halfCycleCount = (int)(distanceTotal / distanceX);
			float distanceLeft = distanceTotal % distanceX;

			if (halfCycleCount % 2 == 0)
			{
				myTransform.Translate(distanceLeft, 0.0f, 0.0f, Space.World);
				movingRight = true;
			}
			else
			{
				myTransform.Translate(distanceX - distanceLeft, 0.0f, 0.0f, Space.World);
				movingRight = false;
			}
		}
	}

	// Use this for initialization
	void Start() 
	{
		myTransform = this.transform;
		distanceX = maxX;

		maxX += myTransform.position.x;
		minX = myTransform.position.x;

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
			if (movingRight)
			{
				if (myTransform.position.x + distanceToMove >= maxX)
				{
					movingRight = false;
					distanceToMove = maxX - myTransform.position.x;
				}
				myTransform.Translate(distanceToMove, 0.0f, 0.0f, Space.World);
			}
			else
			{
				if (myTransform.position.x - distanceToMove <= minX)
				{
					movingRight = true;
					distanceToMove = myTransform.position.x - minX;
				}
				myTransform.Translate(-distanceToMove, 0.0f, 0.0f, Space.World);
			}

			distanceToMove = 0.0f;
		}
	}
}
