using UnityEngine;
using System.Collections;

public class ObjectFadeCycle : MonoBehaviour
{
	public float fadeInterval_min = 0.0f;
	public float fadeInterval_max = 0.0f;
	public float fadeInTime_min = 0.0f;
	public float fadeInTime_max = 0.0f;
	public float fadeOutTime_min = 0.0f;
	public float fadeOutTime_max = 0.0f;
	public float startTimeOffset_min = 0.0f;
	public float startTimeOffset_max = 0.0f;

	private float fadeInterval = 0.0f;
	private float fadeInTime = 0.0f;
	private float fadeOutTime = 0.0f;
	private float startTimeOffset = 0.0f;
		
	private float startTimer = 0.0f;
	private bool isFadedIn = false;

	private int ignoredColorIndex = -1;
	private bool isColliderAttached = false;

	// Use this for initialization
	void Start() 
	{
		InitRandomValues();
		startTimeOffset = Random.Range(startTimeOffset_min, startTimeOffset_max);
		startTimer = Time.time + startTimeOffset;
		isFadedIn = true;

		if (GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostRed", this.gameObject.transform))
			ignoredColorIndex = PlayerData.PLAYER_BLUE;
		else if (GameObjectHelper.IsTagExistsInAncestorsOrSelf("GhostBlue", this.gameObject.transform))
			ignoredColorIndex = PlayerData.PLAYER_RED;

		isColliderAttached = (this.gameObject.collider != null);
	}
	
	// Update is called once per frame
	void Update() 
	{
		// Check if the time is up to begin fading
		if (Time.time - startTimer >= fadeInterval)
		{
			if (isFadedIn)
				// Start fading out
				this.gameObject.GetComponent<FadeBehaviour>().FadeOut(fadeOutTime);
			else
			{
				// Start fading in
				if (isColliderAttached)
				{
					this.gameObject.collider.enabled = true;
					// Remember to ignore collision for other color player
					if (ignoredColorIndex >= 0)
						Physics.IgnoreCollision(this.gameObject.collider, PlayerData.characters[ignoredColorIndex].collider, true);
				}
				this.gameObject.GetComponent<FadeBehaviour>().FadeIn(fadeInTime);
			}
		}
	}

	void OnFadeInCompleted()
	{
		startTimer = Time.time;
		isFadedIn = true;
		InitRandomValues();
	}

	void OnFadeOutCompleted()
	{
		startTimer = Time.time;
		if (isColliderAttached)
			this.gameObject.collider.enabled = false;
		isFadedIn = false;
		InitRandomValues();
	}

	private void InitRandomValues()
	{
		fadeInterval = Random.Range(fadeInterval_min, fadeInterval_max);
		fadeInTime = Random.Range(fadeInTime_min, fadeInTime_max);
		fadeOutTime = Random.Range(fadeOutTime_min, fadeOutTime_max);
	}
}
