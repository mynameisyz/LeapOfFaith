using UnityEngine;
using System.Collections;

public class LocalData : MonoBehaviour {
	
	public static bool isKinectEnabled{ get; set;}
	
	void Awake()
	{
		DontDestroyOnLoad(this);
		isKinectEnabled = false;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
}
