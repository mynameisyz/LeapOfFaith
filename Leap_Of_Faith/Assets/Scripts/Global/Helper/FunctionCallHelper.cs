/*
 * 
 * Written by ONG HENG LE
 * 
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FunctionCallHelper
{
	#region Singleton

	private static FunctionCallHelper instance = null;
	public static FunctionCallHelper Instance
	{
		get 
		{
 			if (instance == null)
				instance = new FunctionCallHelper();

			return instance; 
		}
	}

	#endregion

	public delegate void Del();

	public void Reset()
	{
		Reset_CallOnce();
		Reset_DelayedCallOnce();
	}

	#region CallOnce

	private List<Del> callOnceList = new List<Del>();

	public void CallOnce(Del _func)
	{
		if (callOnceList.Contains(_func))
			return;

		callOnceList.Add(_func);
		_func();
	}

	public void Reset_CallOnce()
	{
		callOnceList.Clear();
	}

	#endregion

	#region DelayedCallOnce

	private Dictionary<Del, float> delayedCallOnceDictionary = new Dictionary<Del,float>();
	private float delayedCallOnceTimestamp = 0.0f;

	public void DelayedCallOnce(Del _func, float _delay)
	{
		float _value = 0.0f;
		if (delayedCallOnceDictionary.TryGetValue(_func, out _value))
		{
			if (delayedCallOnceTimestamp < _value &&
				Time.time >= _value)
			{
				delayedCallOnceTimestamp = Time.time;
				_func();
			}
		}
		else
		{
			delayedCallOnceDictionary.Add(_func, Time.time + _delay);
		}
	}

	public void Reset_DelayedCallOnce()
	{
		delayedCallOnceDictionary.Clear();
		delayedCallOnceTimestamp = 0.0f;
	}

	#endregion
}
