using UnityEngine;
using System.Collections;

public class SwitchTrigger : MonoBehaviour 
{
	public GameObject[] ghostsToSwitch;
	public bool isReusable = false;

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Character"))
			LevelData.Instance.RPC_DoSwitchTrigger(this.gameObject);
	}
}