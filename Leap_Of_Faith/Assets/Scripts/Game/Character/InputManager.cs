using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenNI;
using System.Runtime.InteropServices;

public class InputManager : MonoBehaviour {
	
	//STATIC VARAIBLES
	
	public static GameObject niteController;
	
	public static float horizontal{ get; set; }
	public static float jump{ get;set; }
	public static bool skill{ get;set; }
	public static bool pause{ get;set; }
	
	public static bool isUserCalibrated = false;
	public static bool kinectActive = false;
	public static bool cursorActive = false;
	public static bool autoClickOnce = false;
	public static KinectCursor staticCursor;
	
	//EXPOSED FIELDS
	
	public GameObject nitePrefab;
	
	public bool enableKinectCursor = true;
	
	public float motionSensitivity = 0.15f;
	public float xSensitivity = 0.04f;
	public float ySensitivity = 0.02f;
	public float poseDetectionSensitivity = 0.01f;
	
	public float cursorSpeed = 1.0f;
	public float antiJitter = 0.05f;
	public float cursorDetectionBoxOffsetX = 0.0f;
	public float cursorDetectionBoxOffsetY = 0.0f;
	public float timeBeforeAutoClick = 2.0f;
	public KinectCursor kinectCursor;
	private Queue<float> rawDataX;
	private Queue<float> rawDataY;
	public int CursorSmoothing;
	
	
	//PRIVATE VARIABLES
	
	private static bool isKinectInitialised = false;
	private static GameObject staticPrefab;
	private static Vector3 lShoulderPos;
	private static Vector3 rShoulderPos;
	private static Vector3 lElbowPos;
	private static Vector3 rElbowPos;
	private static Vector3 lHandPos;
	private static Vector3 rHandPos;
	private static Vector3 lHipPos;
	private float lastRootY;
		
	private float motionTimer;
	private float poseTimer;
	private float gestureTimer;
	
	private float clickTimer;
	
	private int currentPose;
	private int lastPose;
	private enum pose{ none, skill, pause };
	
	private Vector2 lastCursorPositon;
	
	[DllImport("user32.dll")]
	static extern bool SetCursorPos(int X, int Y);
	[DllImport("user32.dll")]
	public static extern void mouse_event
	(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
	
	public enum MouseEventType : int
	{
    	LeftDown = 0x02,
    	LeftUp = 0x04,
    	RightDown = 0x08,
    	RightUp = 0x10
	}
	
	// Use this for initialization
	void Start () {
		lastRootY = 0.0f;
		motionTimer = 0.0f;
		poseTimer = 0.0f;
		gestureTimer = 0.0f;
		clickTimer = 0.0f;
		lastCursorPositon = Vector2.zero;
		currentPose = (int)pose.none;
		staticCursor = kinectCursor;
		staticPrefab = nitePrefab;
		rawDataX = new Queue<float>();
		rawDataY = new Queue<float>();
		CursorSmoothing = 5;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(LocalData.isKinectEnabled && !isKinectInitialised && kinectActive)
		{
			InitKinect();
		}
		
		if(!kinectActive)
			HandleKeyboardInput();
		else
			HandleMovementGesture();
		DetectPose();
		HandleCursor();
		
		if(!LocalData.isKinectEnabled && niteController != null)
		{
			Destroy(niteController);
			isKinectInitialised = false;
		}			
	 }
		
	public static void InitKinect()
	{
		if(isKinectInitialised)
			return;
		
		niteController = (GameObject)Instantiate(staticPrefab);
		isKinectInitialised = true;
	}
	
	private bool HandleMovementGesture()
	{
		if( !isUserCalibrated || !LocalData.isKinectEnabled || !kinectActive)
			return false;
		
		
		float difference = lShoulderPos.y - rShoulderPos.y;
			
		//Compute horizontal
		if(Mathf.Abs(difference) > xSensitivity)
		{
			if(difference > 0)
			{
				horizontal = -1;
			}
			if(difference < 0)
			{
				horizontal = 1;
			}
		}
		else
		{
			horizontal = 0;
		}
			
		//Compute jump
		float midPointY = ( lShoulderPos.y + rShoulderPos.y ) / 2;
		
		if(lastRootY != 0)
		{
			if( (midPointY - lastRootY) > ySensitivity )
			{
				jump = 1;
			}
			else
			{
				jump = 0;
			}
		}
		else
		{
			jump = 0;
		}
			
		if(UpdateMotionTimer())
		{
			lastRootY = midPointY;
		}
		
		return true;
	}
	
	private void HandleKeyboardInput()
	{	
		horizontal = Input.GetAxis("Horizontal");
		jump = Input.GetAxis("Jump");
			
		if( Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) )
		{
			skill = true;
		}
		else
		{
			skill = false;
		}
			
		if( Input.GetKeyDown(KeyCode.Escape) )
		{
			pause = true;
		}
		else
		{
			pause = false;
		}
	}
	
	private void DetectPose()
	{
		if(!isUserCalibrated || !LocalData.isKinectEnabled || !kinectActive)
			return;
		
		//detect skill pose
		if( Mathf.Abs( lShoulderPos.y - lElbowPos.y) < poseDetectionSensitivity && 
			Mathf.Abs( lElbowPos.x - lHandPos.x) < poseDetectionSensitivity &&
			Mathf.Abs( rShoulderPos.y - rElbowPos.y) < poseDetectionSensitivity && 
			Mathf.Abs( rElbowPos.x - rHandPos.x ) < poseDetectionSensitivity &&
			currentPose != (int)pose.skill)
		{
			poseTimer += Time.deltaTime;
			
			if(poseTimer >= 1.0f && currentPose == (int)pose.none && !cursorActive)
			{
				skill = true;
				poseTimer = 0.0f;
				currentPose = (int)pose.skill;
			}
			
			gestureTimer = 0.0f;
			
		}
		else if( Mathf.Abs( lShoulderPos.x - lElbowPos.x) < poseDetectionSensitivity &&
			lShoulderPos.y > lElbowPos.y &&
			Mathf.Abs( lElbowPos.x - lHandPos.x) < poseDetectionSensitivity &&
			lElbowPos.y > lHandPos.y &&
			Mathf.Abs(rShoulderPos.y - rElbowPos.y) / Mathf.Abs(rShoulderPos.x - rElbowPos.x) < 1.45f &&
			Mathf.Abs(rShoulderPos.y - rElbowPos.y) / Mathf.Abs(rShoulderPos.x - rElbowPos.x) > 0.55f &&
			Mathf.Abs(rElbowPos.y - rHandPos.y) / Mathf.Abs(rElbowPos.x - rHandPos.x) < 1.45f &&
			Mathf.Abs(rElbowPos.y - rHandPos.y) / Mathf.Abs(rElbowPos.x - rHandPos.x) > 0.55f &&
			currentPose != (int)pose.pause)
					
		{
			poseTimer += Time.deltaTime;
			
			if(poseTimer >= 1.0f && currentPose == (int)pose.none && !cursorActive)
			{
				pause = true;
				poseTimer = 0.0f;
				currentPose = (int)pose.pause;
			}
			
			gestureTimer = 0.0f;
		}
		else
		{
			poseTimer = 0.0f;
			pause = false;
			skill = false;

			gestureTimer += Time.deltaTime;
			if(gestureTimer >= 0.2f)
			{
				currentPose = (int)pose.none;
			}
		}
	}
	
	private void HandleCursor()
	{
		if(!cursorActive || !LocalData.isKinectEnabled || !kinectActive || !enableKinectCursor)
			return;
			
		Vector2 position;
		
		Vector2 origin = new Vector2(rShoulderPos.x + cursorDetectionBoxOffsetX * 100, rShoulderPos.y + cursorDetectionBoxOffsetY * 100);
		float width = Mathf.Abs(lShoulderPos.x - rShoulderPos.x) * 2;
		float height = Mathf.Abs(lHipPos.y - lShoulderPos.y);
		
		if(lHandPos.x < origin.x)
		{
			position.x = Mathf.Abs( (lHandPos.x - origin.x) ) / width;
		}
		else
		{
			position.x = 0.0f;
		}
		
		if(lHandPos.y < origin.y)
		{
      		position.y = Mathf.Abs( (lHandPos.y - origin.y) ) / height;
		}
		else
		{
			position.y = 0.0f;
		}
		
		if(position.x > 1.0f)
			position.x = 1.0f;
		if(position.y > 1.0f)
			position.y = 1.0f;
		
		/*
                // the hand has moved enough to update screen position (jitter control / smoothing)
                if (Math.Abs(rightHand.Position.X - xPrevious) > MoveThreshold || Math.Abs(rightHand.Position.Y - yPrevious) > MoveThreshold)
                {
                    RightHandX = xScaled;
                    RightHandY = yScaled;

                    xPrevious = rightHand.Position.X;
                    yPrevious = rightHand.Position.Y;
		
		*/
		
		rawDataX.Enqueue(position.x);
		rawDataY.Enqueue(position.y);
		
		if(rawDataX.Count > CursorSmoothing)
		{
			rawDataX.Dequeue();
			rawDataY.Dequeue();
		}
		
		Vector2 movePosition;
		
		movePosition.x = ExponentialMovingAverage(rawDataX.ToArray(), 0.9f);
		movePosition.y = ExponentialMovingAverage(rawDataY.ToArray(), 0.9f);
			
		movePosition = Vector2.Lerp(lastCursorPositon, movePosition, Time.deltaTime * cursorSpeed); 
	
		//Debug.Log(movePosition.x + ", " + movePosition.y);
		SetCursorPos( (int)(movePosition.x * AspectUtility.screenWidth), (int)(movePosition.y * AspectUtility.screenHeight));
		
		lastCursorPositon = movePosition;
		
		//handle auto click once within timer
		if(autoClickOnce)
		{
			clickTimer += Time.deltaTime;
			
			kinectCursor.setCursorFill( clickTimer / timeBeforeAutoClick);
			
			if(clickTimer >= timeBeforeAutoClick)
			{
				kinectCursor.setCursorFill(0.0f);
				//activate mouse click
				mouse_event((int)MouseEventType.LeftDown, (int)(movePosition.x * AspectUtility.screenWidth), (int)(movePosition.y * AspectUtility.screenHeight), 0, 0);
				mouse_event((int)MouseEventType.LeftUp, (int)(movePosition.x * AspectUtility.screenWidth), (int)(movePosition.y * AspectUtility.screenHeight), 0, 0);
				clickTimer = 0.0f;
				autoClickOnce = false;

			}
		}
	}
	
	private bool UpdateMotionTimer()
	{
		if(motionTimer >= motionSensitivity )
		{
			motionTimer = 0.0f;
			return true;
		}
		else
		{
			motionTimer += Time.deltaTime;
			return false;
		}
	}
	
	public static void SetKinectInput(int joint, SkeletonJointTransformation skelTrans)
	{
		switch(joint)
		{
			case 6:
			{
				lShoulderPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}
			
			case 7:
			{
				lElbowPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}
			
			case 9:
			{
				lHandPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}
			
			case 12:
			{
				rShoulderPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}	
			
			case 13:
			{
				rElbowPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}	
			
			case 15:
			{
				rHandPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}	
			
			case 17:
			{
				lHipPos = new Vector3(skelTrans.Position.Position.X, 
											skelTrans.Position.Position.Y, 
											skelTrans.Position.Position.Z);
				break;
			}
		}
	}
	
	public static void clickOnce()
	{
		mouse_event((int)MouseEventType.LeftDown, (int)Input.mousePosition.x, (int)Input.mousePosition.y, 0, 0);
		mouse_event((int)MouseEventType.LeftUp, (int)Input.mousePosition.x, (int)Input.mousePosition.y, 0, 0);
				
	}
	
	public static void setCursorFill(float fill)
	{
		staticCursor.setCursorFill(fill);
	}
	
	public float ExponentialMovingAverage( float[] data, float baseValue )
	{
		float numerator = 0;
		float denominator = 0;
 
	    float average = 0.0f;
		for(int i=0; i<data.Length; i++)
		{
			average += data[i];
		}
	    average /= data.Length;
 
    	for ( int i = 0; i < data.Length; ++i )
    	{
        	numerator += data[i] * Mathf.Pow( baseValue, data.Length - i - 1 );
        	denominator += Mathf.Pow( baseValue, data.Length - i - 1 );
    	}
 
   		numerator += average * Mathf.Pow( baseValue, data.Length );
    	denominator += Mathf.Pow( baseValue, data.Length );
 
    	return numerator / denominator;
	}
	
}
