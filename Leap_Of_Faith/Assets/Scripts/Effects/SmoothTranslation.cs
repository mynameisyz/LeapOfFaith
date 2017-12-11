/* 
 * Uses graph for 1 - x^2
 * where x = time and y = value.
 * x starts at 1, reducing to 0 over time.
 *
 * 
 */

using UnityEngine;
using System.Collections;

public class SmoothTranslation : MonoBehaviour
{
	private Vector3 _initialPos = Vector3.zero;

	private Vector3 startPos = Vector3.zero;
	private Vector3 deltaPos = Vector3.zero;
	private float timeLeft = 0.0f;
	private float timeTotal = 0.0f;

	// Use this for initialization
	void Start()
	{
		_initialPos = this.gameObject.transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (timeLeft > 0.0f)
		{
			timeLeft -= Time.deltaTime;

			if (timeLeft < 0.0f)
				timeLeft = 0.0f;

			this.gameObject.transform.position = startPos + ((timeTotal - (timeLeft * timeLeft)) * deltaPos);
		}
	}

	public void MoveTo(Vector3 targetPos, float time)
	{
		timeLeft = time;
		timeTotal = timeLeft * timeLeft;

		startPos = this.gameObject.transform.position;
		deltaPos = targetPos - startPos;
		deltaPos /= timeTotal;
	}

	public void MoveToOriginal(float time)
	{
		startPos = this.gameObject.transform.position;
		deltaPos = _initialPos - startPos;
		timeLeft = time;
	}
}
