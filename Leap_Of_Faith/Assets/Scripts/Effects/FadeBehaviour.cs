using UnityEngine;
using System.Collections;

public class FadeBehaviour : MonoBehaviour 
{
	public string shaderColorName = "_Color";

	private float alpha = 1.0f;
	private float fadeSpeed = 0.0f;
	private bool isFadingIn = false;
	private bool isFadingOut = false;
	
	//public AudioClip cat_sound;
	
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (isFadingOut)
		{
			if (alpha > 0.0f)
			{
				alpha -= Time.deltaTime * fadeSpeed;
			
				if (alpha <= 0.0f)
				{
					alpha = 0.0f;
					isFadingOut = false;
					SendMessage("OnFadeOutCompleted", SendMessageOptions.DontRequireReceiver);
				}
			}
			SetAlpha(alpha);
		}
		
		if (isFadingIn)
		{
			if (alpha < 1.0f)
			{
				alpha += Time.deltaTime * fadeSpeed;
				
				if (alpha >= 1.0f)
				{
					alpha = 1.0f;
					isFadingIn = false;
					SendMessage("OnFadeInCompleted", SendMessageOptions.DontRequireReceiver);
					//audio.PlayOneShot(cat_sound);
				}
			}
			SetAlpha(alpha);
		}
	}
	
	public void FadeIn(float fadeTime)
	{
		if (fadeTime <= 0.0f)
		{
			SendMessage("OnFadeInCompleted", SendMessageOptions.DontRequireReceiver);
			SetAlpha(1.0f);
		}
		else
			fadeSpeed = 1.0f / fadeTime;

		isFadingIn = true;
		isFadingOut = false;
	}

	public void FadeOut(float fadeTime)
	{
		if (fadeTime <= 0.0f)
		{
			SendMessage("OnFadeOutCompleted", SendMessageOptions.DontRequireReceiver);
			SetAlpha(0.0f);
		}
		else
			fadeSpeed = 1.0f / fadeTime;

		isFadingOut = true;
		isFadingIn = false;
	}

	public void SetAlpha(float a)
	{
		Color colorTransform = this.renderer.material.GetColor(shaderColorName);
		colorTransform.a = a;
		this.renderer.material.SetColor(shaderColorName, colorTransform);
	}
}
