using UnityEngine;
using System.Collections;

public class lineRenderer : MonoBehaviour 
{
	FadeBehaviour fader;
	
	// Use this for initialization
	void Start() {
		fader = this.GetComponent<FadeBehaviour>();
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	void OnFadeOutCompleted()
	{
		GameObject.Destroy(this.gameObject);
	}

	public void FadeStart()
	{
		fader.FadeOut(15.0f);
	}
}
