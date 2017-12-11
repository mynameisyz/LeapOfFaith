using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
	public float smooth = 1.0f;
	public float zoomDistance_min = 0.0f;
	public float zoomDistance_max = 100.0f;

	private Transform myTransform = null;

	public static Vector3 midpointTarget = Vector3.zero;
	public static float zoomDistance = 5.0f;
	public static float worldBoundExtentX = 0.0f;

	private Vector3 bgViewportBound_min = new Vector3(0.0f, 0.0f, 0.0f);
	private Vector3 bgViewportBound_max = new Vector3(1.0f, 1.0f, 0.0f);
	private Vector3 bgWorldBound_min = Vector3.zero;
	private Vector3 bgWorldBound_max = Vector3.zero;
	private Vector3 bgBoundingOffset = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		myTransform = transform;

		camera.transparencySortMode = TransparencySortMode.Orthographic;

		// Init World Bounds
		Vector3 worldBoundCenter = this.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, zoomDistance_max * 1.2f));
		Rect screenRect = AspectUtility.screenRect;
		Vector3 worldBoundMax = this.camera.ScreenToWorldPoint(new Vector3(screenRect.xMax, screenRect.yMax, zoomDistance_max * 1.2f));
		worldBoundExtentX = worldBoundMax.x - worldBoundCenter.x;
	}

	// Update is called once per frame
	void Update()
	{
		if (PlayerData.characters[PlayerData.PLAYER_RED] == null ||
			PlayerData.characters[PlayerData.PLAYER_BLUE] == null)
		{
			return;
		}

		// Find the distance between both players
		float distance = Vector3.Distance(PlayerData.characters[PlayerData.PLAYER_RED].transform.position,
										PlayerData.characters[PlayerData.PLAYER_BLUE].transform.position);

		// Find the midpoint between the players' position
		Vector3 midpoint = (PlayerData.characters[PlayerData.PLAYER_RED].transform.position -
							PlayerData.characters[PlayerData.PLAYER_BLUE].transform.position) / 2;

		// Use the midpoint and find the position between the two players
		midpointTarget = PlayerData.characters[PlayerData.PLAYER_RED].transform.position - midpoint;

		// How far to zoom the camera away based on distance
		zoomDistance = distance;

		FollowTarget(midpointTarget);
	}

	void FollowTarget(Vector3 _target)
	{
		// If the zoomDistance is <= zoomDistance_min, which means the players are VERY
		// close to each other. Dont zoom the camera too near.
		if (zoomDistance < zoomDistance_min)
			zoomDistance = zoomDistance_min;
		else if (zoomDistance > zoomDistance_max)
			zoomDistance = zoomDistance_max;

		// Zoom the camera based on distance
		_target.z = _target.z - zoomDistance * 1.2f;
		transform.position = Vector3.Lerp(myTransform.position, _target, Time.deltaTime * smooth);

		// Clamping camera to background
		bgViewportBound_min.z = LevelData.Instance.LevelBounds.center.z - transform.position.z;
		bgViewportBound_max.z = bgViewportBound_min.z;

		bgBoundingOffset = Vector3.zero;
		bgWorldBound_min = camera.ViewportToWorldPoint(bgViewportBound_min);
		bgWorldBound_max = camera.ViewportToWorldPoint(bgViewportBound_max);

		if (bgWorldBound_min.x < LevelData.Instance.LevelBounds.min.x)
			bgBoundingOffset.x += LevelData.Instance.LevelBounds.min.x - bgWorldBound_min.x;

		if (bgWorldBound_min.y < LevelData.Instance.LevelBounds.min.y)
			bgBoundingOffset.y += LevelData.Instance.LevelBounds.min.y - bgWorldBound_min.y;

		if (bgWorldBound_max.x > LevelData.Instance.LevelBounds.max.x)
			bgBoundingOffset.x += LevelData.Instance.LevelBounds.max.x - bgWorldBound_max.x;

		if (bgWorldBound_max.y > LevelData.Instance.LevelBounds.max.y)
			bgBoundingOffset.y += LevelData.Instance.LevelBounds.max.y - bgWorldBound_max.y;

		transform.Translate(bgBoundingOffset);
	}

	public static float GetWorldBoundX_Min()
	{
		return midpointTarget.x - worldBoundExtentX;
	}

	public static float GetWorldBoundX_Max()
	{
		return midpointTarget.x + worldBoundExtentX;
	}
}
