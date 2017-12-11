using UnityEngine;
using System.Collections;
using OpenNI;

public class OpenNISingleSkeletonController : MonoBehaviour 
{
	public OpenNIUserTracker UserTracker;
	public OpenNISkeleton Skeleton;
	
	public OpenNIDepthmapViewer depthViewer;
	public OpenNIImagemapViewer imageViewer;
	public OpenNIUsersRadar usersRadar;
	
	public Texture graphicTex;
	private Rect graphicRect;
	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);
	private float graphicValue = 0.0f;
	private float graphicSpeed = 4.0f;
		
	private int userId;
	
	// Use this for initialization
	void OnEnable () 
	{
		if (null == UserTracker) return;
		if (!UserTracker.enabled) UserTracker.enabled = true;
		UserTracker.MaxCalibratedUsers = 1;
	}
	
	void Start()
	{
		screenRect = AspectUtility.screenRect;
		
		graphicRect = new Rect((screenRect.width / 2) - (screenRect.width / INTENDED_RES.width * 295.0f),
									(screenRect.height / 2) - (screenRect.height / INTENDED_RES.height * 328.0f),
									screenRect.width / INTENDED_RES.width * 590.0f,
									screenRect.height / INTENDED_RES.height * 656.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// do we have a valid calibrated user?
		if (0 != userId)
		{
			// is the user still valid?
			if (!UserTracker.CalibratedUsers.Contains(userId))
			{
				userId = 0;
				Skeleton.RotateToCalibrationPose();

			}
		}
		
		// look for a new userId if we dont have one
		if (0 == userId)
		{
			// just take the first calibrated user
			if (UserTracker.CalibratedUsers.Count > 0)
			{
				userId = UserTracker.CalibratedUsers[0];
				
			}
		}
		
		
		// update our skeleton based on active user id
		if (0 != userId)
		{
			InputManager.isUserCalibrated = true;
		}
		else
		{
			InputManager.isUserCalibrated = false;
		}
		
		if (!InputManager.isUserCalibrated && graphicValue < 1.0f)
		{
			graphicValue += graphicSpeed * Time.deltaTime;
			if (graphicValue > 1.0f)
			{
				graphicValue = 1.0f;
				depthViewer.enabled = true;
				imageViewer.enabled = true;
				usersRadar.enabled = true;
			}
		}
		else if (InputManager.isUserCalibrated && graphicValue > 0.0f)
		{
			graphicValue -= graphicSpeed * Time.deltaTime;
			if (graphicValue <= 0.0f)
			{
				graphicValue = 0.0f;
				depthViewer.enabled = false;
				imageViewer.enabled = false;
				usersRadar.enabled = false;
			}
		}
		
	}
	
	void LateUpdate()
	{
		if (0 != userId)
		{
			UserTracker.UpdateSkeleton(userId, Skeleton);
		}
	}
	
	void OnGUI()
	{
		GUI.depth = -5;
			
		if (userId == 0)
		{
			if (UserTracker.CalibratingUsers.Count > 0)
			{
				// Calibrating
				GUILayout.Box(string.Format("Calibrating: {0}", UserTracker.CalibratingUsers[0]));
			}
			else
			{
				// Looking
				GUILayout.BeginArea (new Rect (Screen.width/2 - 150, Screen.height/2 - 150, 300, 300));
				GUILayout.Box("Waiting for single player to calibrate");
				GUILayout.EndArea();
			}
		}
		else
		{
			// Calibrated
			GUILayout.Box(string.Format("Calibrated: {0}", userId));
		}
		
		if(graphicValue > 0.0f)
		{
			GUI.DrawTexture(RectMultiply(graphicRect, graphicValue), graphicTex);
		}
		
		
	}
	
	private Rect RectMultiply(Rect rect, float multiplier)
	{
		if (multiplier != 1.0f)
		{
			rect.x *= multiplier;
			rect.y *= multiplier;
			rect.width *= multiplier;
			rect.height *= multiplier;
		}

		rect.x += screenRect.x;
		rect.y += screenRect.y;

		return rect;
	}
	
}
