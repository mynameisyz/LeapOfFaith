/*
 * 
 * Written by ONG HENG LE
 * 
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BitTextManager : MonoBehaviour
{
	#region BitCharDictionary

	private SortedDictionary<char,byte[,]> bitCharDictionary = new SortedDictionary<char,byte[,]>();

	private void InitBitCharDictionary()
	{
		bitCharDictionary.Add('a', new byte[5, 4] {	{0, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 1, 1, 1},
													{1, 0, 0, 1},
													{1, 0, 0, 1}});

		bitCharDictionary.Add('b', new byte[5, 4] {	{1, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 1, 1, 0},
													{1, 0, 0, 1},
													{1, 1, 1, 0}});

		bitCharDictionary.Add('c', new byte[5, 4] {	{0, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 0, 0, 0},
													{1, 0, 0, 1},
													{0, 1, 1, 0}});

		bitCharDictionary.Add('d', new byte[5, 4] {	{1, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{1, 1, 1, 0}});

		bitCharDictionary.Add('e', new byte[5, 4] {	{1, 1, 1, 1}, 
													{1, 0, 0, 0},
													{1, 1, 1, 0},
													{1, 0, 0, 0},
													{1, 1, 1, 1}});

		bitCharDictionary.Add('f', new byte[5, 4] {	{1, 1, 1, 1}, 
													{1, 0, 0, 0},
													{1, 1, 1, 0},
													{1, 0, 0, 0},
													{1, 0, 0, 0}});

		bitCharDictionary.Add('g', new byte[5, 4] {	{0, 1, 1, 1}, 
													{1, 0, 0, 0},
													{1, 0, 1, 1},
													{1, 0, 0, 1},
													{0, 1, 1, 1}});

		bitCharDictionary.Add('h', new byte[5, 4] {	{1, 0, 0, 1}, 
													{1, 0, 0, 1},
													{1, 1, 1, 1},
													{1, 0, 0, 1},
													{1, 0, 0, 1}});

		bitCharDictionary.Add('i', new byte[5, 3] {	{1, 1, 1}, 
													{0, 1, 0},
													{0, 1, 0},
													{0, 1, 0},
													{1, 1, 1}});

		bitCharDictionary.Add('j', new byte[5, 4] {	{1, 1, 1, 1}, 
													{0, 0, 1, 0},
													{0, 0, 1, 0},
													{1, 0, 1, 0},
													{1, 1, 1, 0}});

		bitCharDictionary.Add('k', new byte[5, 4] {	{1, 0, 0, 1}, 
													{1, 0, 1, 0},
													{1, 1, 0, 0},
													{1, 0, 1, 0},
													{1, 0, 0, 1}});

		bitCharDictionary.Add('l', new byte[5, 4] {	{1, 0, 0, 0}, 
													{1, 0, 0, 0},
													{1, 0, 0, 0},
													{1, 0, 0, 0},
													{1, 1, 1, 1}});

		bitCharDictionary.Add('m', new byte[5, 5] {	{1, 0, 0, 0, 1}, 
													{1, 1, 0, 1, 1},
													{1, 0, 1, 0, 1},
													{1, 0, 0, 0, 1},
													{1, 0, 0, 0, 1}});

		bitCharDictionary.Add('n', new byte[5, 4] {	{1, 0, 0, 1}, 
													{1, 1, 0, 1},
													{1, 1, 1, 1},
													{1, 0, 1, 1},
													{1, 0, 0, 1}});

		bitCharDictionary.Add('o', new byte[5, 4] {	{0, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{0, 1, 1, 0}});

		bitCharDictionary.Add('p', new byte[5, 4] {	{1, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 1, 1, 0},
													{1, 0, 0, 0},
													{1, 0, 0, 0}});

		bitCharDictionary.Add('q', new byte[5, 4] {	{0, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{0, 1, 1, 0},
													{0, 0, 0, 1}});

		bitCharDictionary.Add('r', new byte[5, 4] {	{1, 1, 1, 0}, 
													{1, 0, 0, 1},
													{1, 1, 1, 0},
													{1, 0, 0, 1},
													{1, 0, 0, 1}});

		bitCharDictionary.Add('s', new byte[5, 4] {	{0, 1, 1, 1}, 
													{1, 0, 0, 0},
													{0, 1, 1, 0},
													{0, 0, 0, 1},
													{1, 1, 1, 0}});

		bitCharDictionary.Add('t', new byte[5, 5] {	{1, 1, 1, 1, 1}, 
													{0, 0, 1, 0, 0},
													{0, 0, 1, 0, 0},
													{0, 0, 1, 0, 0},
													{0, 0, 1, 0, 0}});

		bitCharDictionary.Add('u', new byte[5, 4] {	{1, 0, 0, 1}, 
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{1, 0, 0, 1},
													{0, 1, 1, 0}});

		bitCharDictionary.Add('v', new byte[5, 5] {	{1, 0, 0, 0, 1}, 
													{1, 0, 0, 0, 1},
													{1, 0, 0, 0, 1},
													{0, 1, 0, 1, 0},
													{0, 0, 1, 0, 0}});

		bitCharDictionary.Add('w', new byte[5, 5] {	{1, 0, 0, 0, 1}, 
													{1, 0, 0, 0, 1},
													{1, 0, 1, 0, 1},
													{1, 1, 0, 1, 1},
													{1, 0, 0, 0, 1}});

		bitCharDictionary.Add('x', new byte[5, 5] {	{1, 0, 0, 0, 1}, 
													{0, 1, 0, 1, 0},
													{0, 0, 1, 0, 0},
													{0, 1, 0, 1, 0},
													{1, 0, 0, 0, 1}});

		bitCharDictionary.Add('y', new byte[5, 5] {	{1, 0, 0, 0, 1}, 
													{0, 1, 0, 1, 0},
													{0, 0, 1, 0, 0},
													{0, 0, 1, 0, 0},
													{0, 0, 1, 0, 0}});

		bitCharDictionary.Add('z', new byte[5, 5] {	{1, 1, 1, 1, 1}, 
													{0, 0, 0, 1, 0},
													{0, 0, 1, 0, 0},
													{0, 1, 0, 0, 0},
													{1, 1, 1, 1, 1}});

		bitCharDictionary.Add(' ', new byte[5, 2] {	{0, 0}, 
													{0, 0},
													{0, 0},
													{0, 0},
													{0, 0}});

		bitCharDictionary.Add('.', new byte[5, 1] {	{0}, 
													{0},
													{0},
													{0},
													{1}});

		bitCharDictionary.Add(':', new byte[5, 1] {	{0}, 
													{1},
													{0},
													{1},
													{0}});

		bitCharDictionary.Add('[', new byte[5, 2] {	{1, 1}, 
													{1, 0},
													{1, 0},
													{1, 0},
													{1, 1}});

		bitCharDictionary.Add(']', new byte[5, 2] {	{1, 1}, 
													{0, 1},
													{0, 1},
													{0, 1},
													{1, 1}});

		bitCharDictionary.Add('-', new byte[5, 2] {	{0, 0}, 
													{0, 0},
													{1, 1},
													{0, 0},
													{0, 0}});

		bitCharDictionary.Add('!', new byte[5, 1] {	{1}, 
													{1},
													{1},
													{0},
													{1}});
	}

	private int GetBitCharPixelWidth(char _char)
	{
		byte[,] pixels = null;
		if (bitCharDictionary.TryGetValue(_char, out pixels))
			return pixels.GetLength(1);
		return 0;
	}

	private Vector3[] GetBitCharPixels(char _char)
	{
		Vector3[] pixelPositions = null;
		byte[,] pixels = null;
		if (bitCharDictionary.TryGetValue(_char, out pixels))
		{
			int pixelCount = 0;
			foreach (byte pixel in pixels)
				if (pixel == 1)
					pixelCount++;

			pixelPositions = new Vector3[pixelCount];
			int i = 0;
			for (int row = 0; row < pixels.GetLength(0); row++)
				for (int col = 0; col < pixels.GetLength(1); col++)
					if (pixels[row, col] == 1)
					{
						pixelPositions[i] = new Vector3(col, row);
						i++;
					}
		}
		return pixelPositions;
	}

	#endregion
	
	private List<GameObject> bitObjectUnusedList;
	private List<GameObject> bitObjectUsedList;
	private Bounds bitBounds;

	private const int BITCHARPIXEL_HEIGHT = 5;
	private float lastAnimationEndTime = 0.0f;

	private Dictionary<Bounds, List<int>> stringIndexDictionary = new Dictionary<Bounds, List<int>>();

	void Awake()
	{
		InitBitCharDictionary();

		bitObjectUnusedList = new List<GameObject>(GameObject.FindGameObjectsWithTag("BitTextPixel"));
		bitObjectUsedList = new List<GameObject>();
		bitBounds = bitObjectUnusedList[0].renderer.bounds;
	}

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	private int[] SetupBitChar(char _char, Vector3 _topLeftOffset, float _time)
	{
		Vector3[] positions = GetBitCharPixels(_char);

		if (positions.Length > bitObjectUnusedList.Count)
			return null;

		int[] indexArr = new int[positions.Length];

		for (int i = 0; i < positions.Length; i++)
		{
			positions[i].x = _topLeftOffset.x + bitBounds.extents.x + (positions[i].x * bitBounds.size.x);
			positions[i].y = _topLeftOffset.y - bitBounds.extents.y - (positions[i].y * bitBounds.size.y);
			positions[i].z = _topLeftOffset.z;

			int rand = Random.Range(0, bitObjectUnusedList.Count);
			bitObjectUnusedList[rand].GetComponent<SmoothTranslation>().MoveTo(positions[i], _time);
			bitObjectUsedList.Add(bitObjectUnusedList[rand]);
			bitObjectUnusedList.RemoveAt(rand);

			indexArr[i] = bitObjectUsedList.Count - 1;
		}

		return indexArr;
	}

	public Bounds SetupBitString(string _string, Vector3 _position, float _time)
	{
		_string = _string.ToLower();

		List<int> stringIndexList = new List<int>();
		float width = 0.0f;
		foreach (char _char in _string)
			width += ((GetBitCharPixelWidth(_char) + 1) * bitBounds.size.x);
		width -= bitBounds.size.x;
		Bounds bounds = new Bounds(_position, new Vector3(width, BITCHARPIXEL_HEIGHT * bitBounds.size.y));

		_position.x -= bounds.extents.x;
		_position.y += bounds.extents.y;
		foreach (char _char in _string)
		{
			stringIndexList.AddRange(SetupBitChar(_char, _position, _time));
			_position.x += ((GetBitCharPixelWidth(_char) + 1) * bitBounds.size.x);
		}

		stringIndexDictionary.Add(bounds, stringIndexList);

		if (Time.time + _time > lastAnimationEndTime)
			lastAnimationEndTime = Time.time + _time;

		return bounds;
	}

	public void Reset(float _time)
	{
		foreach (GameObject bitObject in bitObjectUsedList)
		{
			bitObject.GetComponent<SmoothTranslation>().MoveToOriginal(_time);
			bitObjectUnusedList.Add(bitObject);
		}
		bitObjectUsedList.Clear();
		stringIndexDictionary.Clear();

		if (Time.time + _time > lastAnimationEndTime)
			lastAnimationEndTime = Time.time + _time;
	}

	public bool IsAnimationCompleted()
	{
		if (Time.time >= lastAnimationEndTime)
			return true;
		return false;
	}

	public void GlowString(Bounds _boundsRef)
	{
		List<int> bitIndexList;
		if (stringIndexDictionary.TryGetValue(_boundsRef, out bitIndexList))
		{
			foreach (int bitIndex in bitIndexList)
			{
				bitObjectUsedList[bitIndex].GetComponent<FadeBehaviour>().SetAlpha(1.0f);
			}
		}
	}
}
