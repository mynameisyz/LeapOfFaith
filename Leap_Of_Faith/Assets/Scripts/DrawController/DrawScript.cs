using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawScript : MonoBehaviour
{
	public Camera gameCamera;
	public GameObject renderPrefab;
	public int maxControlPoints = 100;

	private Ray ray;
	private RaycastHit hit;
	private BezierPath bezierPath;

	private Color[] colors;
	private List<Vector3>[] points;
	private GameObject[] renderObj;
	private int[] baseDrawingPoints;

	private Vector3 lastPosition;

	// Use this for initialization
	void Start()
	{
		bezierPath = new BezierPath();

		colors = new Color[2];
		colors[PlayerData.PLAYER_RED] = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		colors[PlayerData.PLAYER_BLUE] = new Color(0.0f, 0.0f, 1.0f, 1.0f);

		points = new List<Vector3>[2];
		renderObj = new GameObject[2];
		baseDrawingPoints = new int[2];

		for (int i = 0; i < colors.Length; i++)
		{
			points[i] = new List<Vector3>();
			renderObj[i] = (GameObject)GameObject.Instantiate(renderPrefab, Vector3.zero, this.transform.rotation);
			renderObj[i].renderer.material.SetColor("_TintColor", colors[i]);
			baseDrawingPoints[i] = 0;
		}

		lastPosition = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		ProcessInput();
	}

	private void ProcessInput()
	{
		if (Input.GetMouseButton(0))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (hit.collider.gameObject.name == "canvas")
				{
					/*
					Debug.Log(hit.textureCoord.ToString());
					//Renderer renderer = hit.collider.renderer;
					
					Renderer renderer = hit.collider.renderer;
					//MeshCollider meshCollider = (MeshCollider)hit.collider;
   					 
					Texture2D tex = (Texture2D)renderer.material.mainTexture;
					
					
					Vector3 uvCoord = hit.textureCoord;
				
					uvCoord.x *= tex.width;
					uvCoord.y *= tex.height;
					*/
					//Vector3 screenPosition = Input.mousePosition;
					Vector3 worldPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
																			Mathf.Abs(gameCamera.transform.position.z)
																			+ hit.collider.transform.position.z));
					worldPosition.z -= 0.01f;
					if (worldPosition == lastPosition)
					{
						worldPosition.y += 0.1f;
					}
					networkView.RPC("RPC_AddPoint", RPCMode.All, PlayerData.color, worldPosition);
					lastPosition = worldPosition;
				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			networkView.RPC("RPC_EndDrawing", RPCMode.All, PlayerData.color);
		}
	}

	///Note: this file merely illustrate the algorithms.
	///Generally, they should NOT be called each frame!
	private void RenderLine(int _playerColorId)
	{
		bezierPath.SetControlPoints(points[_playerColorId]);
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();

		LineRenderer lineRenderer = renderObj[_playerColorId].GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(baseDrawingPoints[_playerColorId] + drawingPoints.Count);

		for (int i = 0; i < drawingPoints.Count; i++)
			lineRenderer.SetPosition(baseDrawingPoints[_playerColorId] + i, drawingPoints[i]);

		//Exceeded max control points, freeze render line state and make new points
		if (points[_playerColorId].Count >= maxControlPoints)
		{
			points[_playerColorId].Clear();
			baseDrawingPoints[_playerColorId] += drawingPoints.Count;
		}
	}

	[RPC]
	private void RPC_EndDrawing(int _playerColorId)
	{
		points[_playerColorId].Clear();
		baseDrawingPoints[_playerColorId] = 0;

		renderObj[_playerColorId].GetComponent<lineRenderer>().FadeStart();
		renderObj[_playerColorId] = (GameObject)GameObject.Instantiate(renderPrefab, Vector3.zero, this.transform.rotation);
		renderObj[_playerColorId].renderer.material.SetColor("_TintColor", colors[_playerColorId]);
	}

	[RPC]
	private void RPC_AddPoint(int _playerColorId, Vector3 _point)
	{
		points[_playerColorId].Add(_point);
		RenderLine(_playerColorId);
	}
}
