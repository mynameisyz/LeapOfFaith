using UnityEngine;
using System;
using System.Collections;
using OpenNI;


// ncp fixes

// 1. Set num of bone weights to 4 in quality settings - optional.
// 2. Removed  'transform.rotation' from Quaternion newRotation.
// Set shoulders to use 'arm' bone.


public class OpenNISkeleton : MonoBehaviour 
{
	public Transform Head;
	public Transform Neck;
	public Transform Torso;
	public Transform Waist;

	public Transform LeftCollar;
	public Transform LeftShoulder;
	public Transform LeftElbow;
	public Transform LeftWrist;
	public Transform LeftHand;
	public Transform LeftFingertip;

	public Transform RightCollar;
	public Transform RightShoulder;
	public Transform RightElbow;
	public Transform RightWrist;
	public Transform RightHand;
	public Transform RightFingertip;

	public Transform LeftHip;
	public Transform LeftKnee;
	public Transform LeftAnkle;
	public Transform LeftFoot;

	public Transform RightHip;
	public Transform RightKnee;
	public Transform RightAnkle;
	public Transform RightFoot;
	
	public bool UpdateJointPositions = false;
	public bool UpdateRootPosition = false;
	public bool UpdateOrientation = true;
	public bool DisableRootPostionY = true;
	
	public float RotationDamping = 15.0f;
	public float Scale = 0.001f; 
	
	private Transform[] transforms;
	private Quaternion[] initialRotations;
	private Vector3 rootPosition;
	private Quaternion[] lastRotation;
	
	public void Start()
	{
		int jointCount      = Enum.GetNames(typeof(SkeletonJoint)).Length + 1; // Enum starts at 1
		transforms          = new Transform[jointCount];
		lastRotation  		= new Quaternion[jointCount];
		initialRotations    = new Quaternion[jointCount];
		
		transforms[(int)SkeletonJoint.Head]             = Head;
		transforms[(int)SkeletonJoint.Neck]             = Neck;
		transforms[(int)SkeletonJoint.Torso]            = Torso;
		transforms[(int)SkeletonJoint.Waist]            = Waist;
		transforms[(int)SkeletonJoint.LeftCollar]       = LeftCollar;
		transforms[(int)SkeletonJoint.LeftShoulder]     = LeftShoulder;
		transforms[(int)SkeletonJoint.LeftElbow]        = LeftElbow;
		transforms[(int)SkeletonJoint.LeftWrist]        = LeftWrist;
		transforms[(int)SkeletonJoint.LeftHand]         = LeftHand;
		transforms[(int)SkeletonJoint.LeftFingertip]    = LeftFingertip;
		transforms[(int)SkeletonJoint.RightCollar]      = RightCollar;
		transforms[(int)SkeletonJoint.RightShoulder]    = RightShoulder;
		transforms[(int)SkeletonJoint.RightElbow]       = RightElbow;
		transforms[(int)SkeletonJoint.RightWrist]       = RightWrist;
		transforms[(int)SkeletonJoint.RightHand]        = RightHand;
		transforms[(int)SkeletonJoint.RightFingertip]   = RightFingertip;
		transforms[(int)SkeletonJoint.LeftHip]          = LeftHip;
		transforms[(int)SkeletonJoint.LeftKnee]         = LeftKnee;
		transforms[(int)SkeletonJoint.LeftAnkle]        = LeftAnkle;
		transforms[(int)SkeletonJoint.LeftFoot]         = LeftFoot;
		transforms[(int)SkeletonJoint.RightHip]         = RightHip;
		transforms[(int)SkeletonJoint.RightKnee]        = RightKnee;
	    transforms[(int)SkeletonJoint.RightAnkle]       = RightAnkle;
		transforms[(int)SkeletonJoint.RightFoot]        = RightFoot;
		
		// save all initial rotations
		// NOTE: Assumes skeleton model is in "T" pose since all rotations are relative to that pose
		foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
		{
			if (transforms[(int)j])
			{
				// we will store the relative rotation of each joint from the gameobject rotation
				// we need this since we will be setting the joint's rotation (not localRotation) but we 
				// still want the rotations to be relative to our game object
				initialRotations[(int)j] = Quaternion.Inverse(transform.rotation) * transforms[(int)j].rotation;
			}
		}
		
		// start out in calibration pose
		RotateToCalibrationPose();
		
		foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
		{
			if(transforms[(int)j])
				lastRotation[(int)j] = transforms[(int)j].rotation;
		}
	}
	
	public void UpdateRoot(Point3D skelRoot)
	{
		rootPosition = new Vector3(skelRoot.X, skelRoot.Y, -skelRoot.Z) * Scale;
		if (DisableRootPostionY) rootPosition.y = 0.0f;
		
		if (UpdateRootPosition)
		{
			transform.localPosition = rootPosition;  //  transform.rotation * rootPosition;
		}
	}
	
	public void UpdateJoint(SkeletonJoint joint, SkeletonJointTransformation skelTrans)
	{
		// make sure something is hooked up to this joint
		if (!transforms[(int)joint])
		{
			return;
		}
		
		// modify orientation (if confidence is high enough)
        if (UpdateOrientation && skelTrans.Orientation.Confidence > 0.5)
        {
			// Z coordinate in OpenNI is opposite from Unity
			// Convert the OpenNI 3x3 rotation matrix to unity quaternion while reversing the Z axis
			Vector3 worldZVec = new Vector3(-skelTrans.Orientation.Z1, -skelTrans.Orientation.Z2, skelTrans.Orientation.Z3);
			Vector3 worldYVec = new Vector3(skelTrans.Orientation.Y1, skelTrans.Orientation.Y2, -skelTrans.Orientation.Y3);
			Quaternion jointRotation = Quaternion.LookRotation(worldZVec, worldYVec);
			// Quaternion newRotation = transform.rotation * jointRotation * initialRotations[(int)joint];
			Quaternion newRotation = jointRotation * initialRotations[(int)joint];
			
			//Apply rotation using new Rotation and last frame Rotation
			transforms[(int)joint].rotation = Quaternion.Slerp(lastRotation[(int)joint], newRotation, Time.deltaTime * RotationDamping);
       		
			
		}
		
		// modify position (if needed, and confidence is high enough)
		if (UpdateJointPositions)
		{
            Vector3 v3pos = new Vector3(skelTrans.Position.Position.X, skelTrans.Position.Position.Y, -skelTrans.Position.Position.Z);
			transforms[(int)joint].localPosition = (v3pos * Scale) - rootPosition;
		}
		
		//Update last frame Rotation after apply rotation
		lastRotation[(int)joint] = transforms[(int)joint].rotation;
	}

	public void RotateToCalibrationPose()
	{
		foreach (SkeletonJoint j in Enum.GetValues(typeof(SkeletonJoint)))
		{
			if (null != transforms[(int)j])
			{
				transforms[(int)j].rotation = transform.rotation * initialRotations[(int)j];
			}
		}
		
		// calibration pose is skeleton base pose ("T") with both elbows bent in 90 degrees
		RightElbow.rotation = transform.rotation * Quaternion.Euler(0, -90, 90) * initialRotations[(int)SkeletonJoint.RightElbow];
        LeftElbow.rotation = transform.rotation * Quaternion.Euler(0, 90, -90) * initialRotations[(int)SkeletonJoint.LeftElbow];
	}
	
}
