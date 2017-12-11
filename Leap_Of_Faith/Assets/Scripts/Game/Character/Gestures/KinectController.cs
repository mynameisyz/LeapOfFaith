using UnityEngine;
using System.Collections;

public class KinectController : MonoBehaviour {
	
	public Transform mesh;
	public GameObject niteController;
	
	public GestureController gestureController;
	public IK leftIKController;
	public IK rightIKCOntroller;
	
	public Transform head;
	public Transform lShoulderPoint;
	public Transform rShoulderPoint;
	
	public Transform lShoulder;
	public Transform rShoulder;
	
	public Transform lElbow;
	public Transform rElbow;
	
	public static Transform sMesh;
	public static Transform sHead;
	public static Transform sLShoulderPoint;
	public static Transform sRShoulderPoint;
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Quaternion _head = head.rotation;
			stream.Serialize(ref _head);
			
			Quaternion _lShoulder = lShoulder.rotation;
			stream.Serialize(ref _lShoulder);
			
			Quaternion _rShoulder = rShoulder.rotation;
			stream.Serialize(ref _rShoulder);
			
			Quaternion _lElbow = lElbow.rotation;
			stream.Serialize(ref _lElbow);
			
			Quaternion _rElbow = rElbow.rotation;
			stream.Serialize(ref _rElbow);
		}
		else
		{
			Quaternion _head = Quaternion.identity;
			stream.Serialize(ref _head);
			head.rotation = _head;
			
			Quaternion _lShoulder = Quaternion.identity;
			stream.Serialize(ref _lShoulder);
			lShoulder.rotation = _lShoulder;
			
			Quaternion _rShoulder = Quaternion.identity;
			stream.Serialize(ref _rShoulder);
			rShoulder.rotation = _rShoulder;
			
			Quaternion _lElbow = Quaternion.identity;
			stream.Serialize(ref _lElbow);
			lElbow.rotation = _lElbow;
			
			Quaternion _rElbow = Quaternion.identity;
			stream.Serialize(ref _rElbow);
			rElbow.rotation = _rElbow;
		}
	}
	
	// Use this for initialization
	void Awake () 
	{
		if(this.networkView.isMine)
		{
			networkView.RPC ("ToEnableKinect", RPCMode.All, LocalData.isKinectEnabled);
		}
	}
	
	[RPC]
	private void ToEnableKinect(bool enabled)
	{
		if (enabled)
		{
			Debug.Log(PlayerData.color.ToString() + ": Enabled");
			if(networkView.isMine)
			{
				InputManager.niteController.GetComponent<OpenNISingleSkeletonController>().Skeleton = this.GetComponent<OpenNISkeleton>();			
				InputManager.kinectActive = true;
				InputManager.cursorActive = false;
			}
			this.GetComponent<OpenNISkeleton>().enabled = true;
			
			sMesh = mesh;
			sHead = head;
			sLShoulderPoint = lShoulderPoint;
			sRShoulderPoint = rShoulderPoint;			
			
			leftIKController.enabled = false;
			rightIKCOntroller.enabled = false;
			gestureController.enabled = false;
			//mesh.animation.enabled = false;
			
			networkView.observed = this;
		}
		else
		{
			Debug.Log(PlayerData.color.ToString() + ": Disabled");
			this.GetComponent<OpenNISkeleton>().enabled = false;
			
			leftIKController.enabled = true;
			rightIKCOntroller.enabled = true;
			gestureController.enabled = true;
			
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void LateUpdate()
	{
		
	}
	
	public static void SetDefaultPose()
	{
		if(sMesh == null)
			return;
		sMesh.Rotate(0, 180, 0);
	}
	
	public static void SetFinalRotation()
	{
		if(sMesh == null)
			return;
		sMesh.Rotate(0, 180, 0);
	}
}
