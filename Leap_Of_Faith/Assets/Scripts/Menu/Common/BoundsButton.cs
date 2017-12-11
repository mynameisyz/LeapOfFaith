using UnityEngine;
using System.Collections;

public class BoundsButton 
{
	public delegate void Del();
	public Bounds bounds;
	private Del onClickFunc;

	private bool isConstructed = false;
	private bool isEnabled = false;
	private float timeBeforeEnable = 0.0f;

	public void Reconstruct(Bounds _bounds, Del _onClickFunc)
	{
		isConstructed = true;

		bounds = _bounds;
		onClickFunc = _onClickFunc;
		isEnabled = false;
		timeBeforeEnable = 0.0f;
	}

	public void Update()
	{
		if (timeBeforeEnable > 0.0f)
		{
			timeBeforeEnable -= Time.deltaTime;
			if (timeBeforeEnable <= 0.0f)
				isEnabled = !isEnabled;
		}

		if (isConstructed && isEnabled)
			HandleMouseInput();
	}

	public void SetEnabled(bool _enabled, float _delay)
	{
		if (_enabled != isEnabled)
		{
			if (_delay <= 0.0f)
				isEnabled = !isEnabled;
			else
				timeBeforeEnable = _delay;
		}
	}

	private void HandleMouseInput()
	{
		if (BoundsContainsScreenPoint(bounds, Input.mousePosition) &&
			Input.GetMouseButtonDown(0))
		{
			onClickFunc();
		}
	}

	public static bool BoundsContainsScreenPoint(Bounds _bounds, Vector3 _screenPoint)
	{
		_screenPoint.z = _bounds.center.z - Camera.main.transform.position.z;
		if (_bounds.Contains(Camera.main.ScreenToWorldPoint(_screenPoint)))
			return true;

		return false;
	}
}
