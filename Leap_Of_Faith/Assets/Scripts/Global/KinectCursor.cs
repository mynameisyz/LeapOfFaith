using UnityEngine;
using System.Collections;

public class KinectCursor : MonoBehaviour 
{
	public Texture kinectCursorBorder;
	public Texture kinectCursorFill;

	private Rect kinectCursorBorderRect;
	private Rect kinectCursorFillRect;

	private Vector2 kinectCursorBorderRectExtents;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);

	private float cursorValue = 0.0f;
	private Vector3 cursorPos = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		screenRect = AspectUtility.screenRect;

		kinectCursorBorderRect = new Rect(0, 0,
										screenRect.width / INTENDED_RES.width * 78.0f,
										screenRect.height / INTENDED_RES.height * 78.0f);

		kinectCursorBorderRectExtents = new Vector2(kinectCursorBorderRect.width / 2,
													kinectCursorBorderRect.height / 2);

		kinectCursorFillRect = new Rect(0, 0,
										screenRect.width / INTENDED_RES.width * 78.0f,
										screenRect.height / INTENDED_RES.height * 78.0f);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (LocalData.isKinectEnabled)
		{
			cursorPos = Input.mousePosition;

			kinectCursorBorderRect.x = cursorPos.x - kinectCursorBorderRectExtents.x;
			kinectCursorBorderRect.y = AspectUtility.screenHeight - cursorPos.y - kinectCursorBorderRectExtents.y;

			kinectCursorFillRect.x = kinectCursorBorderRect.x;
			kinectCursorFillRect.y = kinectCursorBorderRect.y;
		}
	}

	void OnGUI()
	{
		if (LocalData.isKinectEnabled)
		{
			GUI.depth = -1;

			if (cursorValue > 0.0f)
				GUI.DrawTexture(ScaleRectAtCenter(kinectCursorFillRect, cursorValue), kinectCursorFill);

			GUI.DrawTexture(kinectCursorBorderRect, kinectCursorBorder);

			GUI.depth = 0;
		}
	}

	private Rect ScaleRectAtCenter(Rect rect, float scale)
	{
		if (scale != 1.0f)
		{
			rect.x += ((1.0f - scale) / 2.0f) * rect.width;
			rect.y += ((1.0f - scale) / 2.0f) * rect.height;

			rect.width *= scale;
			rect.height *= scale;
		}

		return rect;
	}
	
	public void setCursorFill(float fill)
	{
		cursorValue = fill;
		if(cursorValue > 1.0f)
			cursorValue = 1.0f;
		
	}
	
	public void resetCursorFill()
	{
		cursorValue = 0.0f;
	}
}
