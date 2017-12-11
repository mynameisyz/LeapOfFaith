using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenNI;

public class OpenNIUserTracker : MonoBehaviour 
{
	private OpenNIContext Context;
	public int MaxCalibratedUsers;
    public float m_SmoothingFactor = 0.5f;


	public UserGenerator userGenerator;
	private SkeletonCapability skeletonCapbility;
	private PoseDetectionCapability poseDetectionCapability;
	private string calibPose;

	private List<int> allUsers;
	private List<int> calibratedUsers;
	private List<int> calibratingUsers;
	
	public IList<int> AllUsers
	{
		get { return allUsers.AsReadOnly(); }
	}
	public IList<int> CalibratedUsers
	{
		get { return calibratedUsers.AsReadOnly(); }
	}
	public IList<int> CalibratingUsers
	{
		get {return calibratingUsers.AsReadOnly(); }
	}
	
	public bool Mirror
	{
		get { return Context.Mirror; }
		set { Context.Mirror = value; }
	}
	
	bool AttemptToCalibrate
	{
		get { return calibratedUsers.Count < MaxCalibratedUsers; }
	}
	
	void Start() 
	{
		// Make sure we have a valid OpenNIContext
		Context = OpenNIContext.Instance;
		if (null == Context)
		{
			print("OpenNI not inited");
			return;
		}
	
		calibratedUsers = new List<int>();
		calibratingUsers = new List<int>();
		allUsers = new List<int>();
		this.userGenerator = new UserGenerator(this.Context.context);
		this.skeletonCapbility = this.userGenerator.SkeletonCapability;
		this.poseDetectionCapability = this.userGenerator.PoseDetectionCapability;
		this.calibPose = this.skeletonCapbility.CalibrationPose;
		this.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);

		// ncp Added
        this.skeletonCapbility.SetSmoothing(m_SmoothingFactor);
		
		
        this.userGenerator.NewUser += new EventHandler<NewUserEventArgs>(userGenerator_NewUser);
        this.userGenerator.LostUser += new EventHandler<UserLostEventArgs>(userGenerator_LostUser);
        this.poseDetectionCapability.PoseDetected += new EventHandler<PoseDetectedEventArgs>(poseDetectionCapability_PoseDetected);
        this.skeletonCapbility.CalibrationEnd += new EventHandler<CalibrationEndEventArgs>(skeletonCapbility_CalibrationEnd);
	}

	void Update () 
	{
		if (Context.ValidContext)
		{
			// print("Update - UserTracker");
			Context.Update();
		}
	}
    void skeletonCapbility_CalibrationEnd(object sender, CalibrationEndEventArgs e)
    {
        if (e.Success)
        {
            if (AttemptToCalibrate)
            {
                print("Starting tracking");
                this.skeletonCapbility.StartTracking(e.ID);
                calibratedUsers.Add(e.ID);
            }
        }
        else
        {
            if (AttemptToCalibrate)
            {
                this.poseDetectionCapability.StartPoseDetection(calibPose, e.ID);
            }
        }
		calibratingUsers.Remove(e.ID);
    }

    void poseDetectionCapability_PoseDetected(object sender, PoseDetectedEventArgs e)
    {
        print("Pose detected");
        this.poseDetectionCapability.StopPoseDetection(e.ID);
        if (AttemptToCalibrate)
        {
            print("Starting calibration");
            this.skeletonCapbility.RequestCalibration(e.ID, true);
			calibratingUsers.Add(e.ID);
        }
    }

    void userGenerator_LostUser(object sender, UserLostEventArgs e)
    {
        allUsers.Remove(e.ID);
        if (calibratedUsers.Contains(e.ID))
        {
            calibratedUsers.Remove(e.ID);
        }
		if (calibratingUsers.Contains(e.ID))
		{
			calibratingUsers.Remove(e.ID);
		}

        if (AttemptToCalibrate)
        {
            AttemptCalibrationForAllUsers();
        }
    }

    void userGenerator_NewUser(object sender, NewUserEventArgs e)
    {
        allUsers.Add(e.ID);
        if (AttemptToCalibrate)
        {
            print("Starting pose detection");
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);
        }
    }
	
	void AttemptCalibrationForAllUsers()
	{
		foreach (int id in userGenerator.GetUsers())
		{
			if (!skeletonCapbility.IsCalibrating(id) && !skeletonCapbility.IsTracking(id))
			{
				this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
			}
		}
	}
	
	public void UpdateSkeleton(int userId, OpenNISkeleton skeleton)
	{
		// make sure we have skeleton data for this user
		if (!skeletonCapbility.IsTracking(userId))
		{
			return;
		}

		KinectController.SetDefaultPose();

		// Use torso as root
		SkeletonJointTransformation skelTrans = new SkeletonJointTransformation();
		skelTrans = skeletonCapbility.GetSkeletonJoint(userId, SkeletonJoint.Torso);
		if(skeleton != null)
			skeleton.UpdateRoot(skelTrans.Position.Position);
		// update each joint with data from OpenNI
		foreach (SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
		{
			if (skeletonCapbility.IsJointAvailable(joint))
			{
				skelTrans = skeletonCapbility.GetSkeletonJoint(userId, joint);
				if(skeleton != null)
				{
					
					skeleton.UpdateJoint(joint, skelTrans);
				}
				
				InputManager.SetKinectInput((int)joint, skelTrans);
			}
		}
		
		KinectController.SetFinalRotation();
	}
	
	public Vector3 GetUserCenterOfMass(int userId)
	{
		Point3D com = userGenerator.GetCoM(userId);
		return new Vector3(com.X, com.Y, -com.Z);
	}
	
	
}
