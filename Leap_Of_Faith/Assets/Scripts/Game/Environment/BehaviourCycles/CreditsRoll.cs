using UnityEngine;
using System.Collections;

public class CreditsRoll : MonoBehaviour
{
	public Material creditsRoll;
	private Vector2 offset = Vector2.zero;
	private float yOffset = 1.0f;
	public float yOffsetDecrement = 0.0005f;

	private float completeCycleTime = 0.0f;
	private float currentTimeOffset = 0.0f;

	private float lastNetworkSyncTime = 0.0f;
	private const float NETWORKSYNC_INTERVAL = 1.0f;

	private void NetworkSync()
	{
		currentTimeOffset = NetworkTime.Instance.Time % completeCycleTime;
		yOffset = 1.0f - (currentTimeOffset * yOffsetDecrement);
		offset.y = yOffset;
		creditsRoll.mainTextureOffset = offset;
	}

	// Use this for initialization
	void Start()
	{
		completeCycleTime = 1.0f / yOffsetDecrement;
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > lastNetworkSyncTime + NETWORKSYNC_INTERVAL)
		{
			lastNetworkSyncTime = Time.time;
			NetworkSync();
		}

		yOffset -= yOffsetDecrement * Time.deltaTime;
		offset.y = yOffset;
		creditsRoll.mainTextureOffset = offset;

		if (yOffset <= 0)
		{
			yOffset = 1.0f;
		}
	}
}