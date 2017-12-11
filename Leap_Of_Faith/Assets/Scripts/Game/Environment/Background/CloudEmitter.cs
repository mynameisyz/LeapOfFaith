using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudEmitter : MonoBehaviour 
{
	public Texture[] SMALL_CLOUDTEX;

	public float SMALL_CLOUDSCALE = 0.0f;
	public float SMALL_ASPECTRATIO = 0.0f;
	public int SMALL_CLOUDCOUNT = 0;

	public float SMALL_YPOS_MIN = 0.0f;
	public float SMALL_YPOS_MAX = 0.0f;

	public float SMALL_ZPOS_MIN = 0.0f;
	public float SMALL_ZPOS_MAX = 0.0f;

	public float SMALL_SPEED_MIN = 0.0f;
	public float SMALL_SPEED_MAX = 0.0f;

	public Texture[] BIG_CLOUDTEX;

	public float BIG_CLOUDSCALE = 0.0f;
	public float BIG_ASPECTRATIO = 0.0f;
	public int BIG_CLOUDCOUNT = 0;

	public float BIG_YPOS_MIN = 0.0f;
	public float BIG_YPOS_MAX = 0.0f;

	public float BIG_ZPOS_MIN = 0.0f;
	public float BIG_ZPOS_MAX = 0.0f;

	public float BIG_SPEED_MIN = 0.0f;
	public float BIG_SPEED_MAX = 0.0f;

	private class Cloud
	{
		public bool isAlive = false;
		public GameObject obj = null;
		public bool isMovingToRight = false;

		public float extentsX = 0.0f;
		public float extentsY = 0.0f;

		private float speed = 0.0f;
		public float Speed
		{
			get { return speed; }
			set 
			{
				speed = value;
				if (speed >= 0)
					isMovingToRight = true;
				else
					isMovingToRight = false;
			}
		}

		public Cloud(GameObject _obj, float _speed)
		{
			obj = _obj;
			Speed = _speed;
			extentsX = obj.GetComponent<MeshFilter>().mesh.bounds.extents.x;
			extentsY = obj.GetComponent<MeshFilter>().mesh.bounds.extents.y;
			isAlive = true;
		}

		public void Update()
		{
			if (isAlive)
			{
				obj.transform.Translate(speed * Time.deltaTime, 0.0f, 0.0f, Space.World);

				if ((isMovingToRight && obj.transform.position.x > LevelData.Instance.LevelBounds.max.x + extentsX) ||
					(!isMovingToRight && obj.transform.position.x < LevelData.Instance.LevelBounds.min.x - extentsX))
					Kill();
			}
		}

		public void Kill()
		{
			isAlive = false;
		}

		public void Revive(Texture _tex, float _speed)
		{
			obj.renderer.material.SetTexture("_MainTex", _tex);
			obj.renderer.material.mainTextureScale = new Vector2(1.0f, 0.99f);
			Speed = _speed;

			isAlive = true;
		}
	}

	private List<Cloud> smallCloudList = new List<Cloud>();
	private List<Cloud> bigCloudList = new List<Cloud>();

	// Use this for initialization
	void Start() 
	{
		InitClouds();
	}
	
	// Update is called once per frame
	void Update() 
	{
		for (int i = smallCloudList.Count - 1; i >= 0; i--)
		{
			smallCloudList[i].Update();

			if (smallCloudList[i].isAlive == false)
			{
				smallCloudList[i].Revive(SMALL_CLOUDTEX[Random.Range(0, SMALL_CLOUDTEX.Length - 1)],
										Random.Range(SMALL_SPEED_MIN, SMALL_SPEED_MAX));

				Vector3 startPos = Vector3.zero;
				if (smallCloudList[i].isMovingToRight)
					startPos.x = LevelData.Instance.LevelBounds.min.x - smallCloudList[i].extentsX;
				else
					startPos.x = LevelData.Instance.LevelBounds.max.x + smallCloudList[i].extentsX;
				startPos.y = LevelData.Instance.boundingObject.transform.position.y + Random.Range(SMALL_YPOS_MIN, SMALL_YPOS_MAX);
				startPos.z = LevelData.Instance.boundingObject.transform.position.z + Random.Range(SMALL_ZPOS_MIN, SMALL_ZPOS_MAX);
				smallCloudList[i].obj.transform.position = startPos;
			}
		}

		for (int i = bigCloudList.Count - 1; i >= 0; i--)
		{
			bigCloudList[i].Update();

			if (bigCloudList[i].isAlive == false)
			{
				bigCloudList[i].Revive(BIG_CLOUDTEX[Random.Range(0, BIG_CLOUDTEX.Length - 1)],
										Random.Range(BIG_SPEED_MIN, BIG_SPEED_MAX));

				Vector3 startPos = Vector3.zero;
				if (bigCloudList[i].isMovingToRight)
					startPos.x = LevelData.Instance.LevelBounds.min.x - bigCloudList[i].extentsX;
				else
					startPos.x = LevelData.Instance.LevelBounds.max.x + bigCloudList[i].extentsX;
				startPos.y = LevelData.Instance.boundingObject.transform.position.y + Random.Range(BIG_YPOS_MIN, BIG_YPOS_MAX);
				startPos.z = LevelData.Instance.boundingObject.transform.position.z + Random.Range(BIG_ZPOS_MIN, BIG_ZPOS_MAX);
				bigCloudList[i].obj.transform.position = startPos;
			}
		}
	}

	private void InitClouds()
	{
		while (smallCloudList.Count < SMALL_CLOUDCOUNT)
		{
			Cloud cloud = new Cloud(GameObjectHelper.CreatePlaneObject("Small Cloud",
																		SMALL_CLOUDSCALE,
																		SMALL_ASPECTRATIO,
																		Shader.Find("Transparent/Diffuse"),
																		SMALL_CLOUDTEX[Random.Range(0, SMALL_CLOUDTEX.Length - 1)], 
																		this.transform),
									Random.Range(SMALL_SPEED_MIN, SMALL_SPEED_MAX));

			Vector3 startPos = Vector3.zero;
			if (cloud.isMovingToRight)
				startPos.x = Random.Range(LevelData.Instance.LevelBounds.min.x - cloud.extentsX,
										LevelData.Instance.LevelBounds.max.x);
			else
				startPos.x = Random.Range(LevelData.Instance.LevelBounds.min.x,
										LevelData.Instance.LevelBounds.max.x + cloud.extentsX);
			startPos.y = LevelData.Instance.boundingObject.transform.position.y + Random.Range(SMALL_YPOS_MIN, SMALL_YPOS_MAX);
			startPos.z = LevelData.Instance.boundingObject.transform.position.z + Random.Range(SMALL_ZPOS_MIN, SMALL_ZPOS_MAX);
			cloud.obj.transform.position = startPos;

			smallCloudList.Add(cloud);
		}

		while (bigCloudList.Count < BIG_CLOUDCOUNT)
		{
			Cloud cloud = new Cloud(GameObjectHelper.CreatePlaneObject("Big Cloud", 
																		BIG_CLOUDSCALE,
																		BIG_ASPECTRATIO,
																		Shader.Find("Transparent/Diffuse"),
																		BIG_CLOUDTEX[Random.Range(0, BIG_CLOUDTEX.Length - 1)], 
																		this.transform),
									Random.Range(BIG_SPEED_MIN, BIG_SPEED_MAX));

			Vector3 startPos = Vector3.zero;
			if (cloud.isMovingToRight)
				startPos.x = Random.Range(LevelData.Instance.LevelBounds.min.x - cloud.extentsX,
										LevelData.Instance.LevelBounds.max.x);
			else
				startPos.x = Random.Range(LevelData.Instance.LevelBounds.min.x,
										LevelData.Instance.LevelBounds.max.x + cloud.extentsX);
			startPos.y = LevelData.Instance.boundingObject.transform.position.y + Random.Range(BIG_YPOS_MIN, BIG_YPOS_MAX);
			startPos.z = LevelData.Instance.boundingObject.transform.position.z + Random.Range(BIG_ZPOS_MIN, BIG_ZPOS_MAX);
			cloud.obj.transform.position = startPos;

			bigCloudList.Add(cloud);
		}
	}
}
