using UnityEngine;
using System.Collections;

public class GlowEffect : MonoBehaviour 
{
	public Texture glowTex = null;
	public float glowScale = 1.0f;
	public Vector3 glowPos = Vector3.zero;

	private GameObject glowObject = null;
	private Vector3 lockedEulerAngles = Vector3.zero;
	private bool isPosDynamic = false;

	void Awake()
	{
		glowObject = GameObjectHelper.CreatePlaneObject("GlowEffect", glowScale,
														this.gameObject.transform.localScale.x / this.gameObject.transform.localScale.y,
														Shader.Find("Particles/Alpha Blended"),
														glowTex, this.transform);
	}

	// Use this for initialization
	void Start()
	{
		glowObject.transform.localPosition = glowPos;
		lockedEulerAngles = glowObject.transform.eulerAngles;

		if (glowPos != Vector3.zero)
			isPosDynamic = true;
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	void LateUpdate()
	{
		if (glowObject.transform.eulerAngles != lockedEulerAngles)
			glowObject.transform.eulerAngles = lockedEulerAngles;

		if (isPosDynamic)
			glowObject.transform.position = this.transform.position + glowPos;
	}

	public void SetColor(Color _color)
	{
		glowObject.renderer.material.SetColor("_TintColor", _color);
	}
}
