using UnityEngine;
using System.Collections;

public class ElevatorFixedMoveTo : MonoBehaviour 
{
	private TranslationYCycle translateYScript;
	public AudioClip elevatorDoor;
	
	// Use this for initialization
	void Start () 
	{
		FunctionCallHelper.Instance.Reset();
		translateYScript = this.GetComponent<TranslationYCycle>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!translateYScript.enabled)
			FunctionCallHelper.Instance.DelayedCallOnce(StartElevation, 3.0f);
		
		if (!translateYScript.movingUp && Network.isServer)
		{
			networkView.RPC ("LiftEventOpen", RPCMode.All);
		}
	}
	
	void StartElevation()
	{
		translateYScript.enabled = true;
	}
	
	[RPC]
	void LiftEventOpen()
	{
		audio.PlayOneShot(elevatorDoor);
		animation.Play ("open");
		Destroy(translateYScript);
		Destroy(this);
	}
}
