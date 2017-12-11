using UnityEngine;
using System.Collections;

public class HeadGesturer : MonoBehaviour {
	
	public Transform targetPos;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		this.transform.LookAt(targetPos.position);
		this.transform.Rotate(new Vector3(0, 90, -90));
	}
}
