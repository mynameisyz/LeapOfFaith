using UnityEngine;
using System.Collections;

public class GameObjectHelper : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	public static GameObject CreatePlaneObject(string _name, float _scale, float _aspectRatio, Shader _shader, Texture _tex, Transform _parent)
	{
		Mesh m = new Mesh();
		m.name = "Object_Mesh";
		m.vertices = new Vector3[] {new Vector3(-_scale/_aspectRatio, -_scale, 0.0f), 
									new Vector3(_scale/_aspectRatio, -_scale, 0.0f), 
									new Vector3(_scale/_aspectRatio, _scale, 0.0f), 
									new Vector3(-_scale/_aspectRatio, _scale, 0.0f)};
		m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
		m.RecalculateNormals();
		GameObject obj = new GameObject(_name, typeof(MeshRenderer), typeof(MeshFilter));
		obj.GetComponent<MeshFilter>().mesh = m;
		obj.renderer.material = new Material(_shader);
		obj.renderer.material.SetTexture("_MainTex", _tex);
		obj.renderer.material.mainTextureScale = new Vector2(1.0f, 0.99f);
		obj.transform.Rotate(Vector3.up * 180);
		obj.transform.Rotate(Vector3.forward * 90);
		obj.transform.parent = _parent;

		return obj;
	}

	public static bool IsTagExistsInAncestorsOrSelf(string tagToFind, Transform obj)
	{
		while (obj != null)
		{
			if (obj.CompareTag(tagToFind))
				return true;

			obj = obj.parent;
		}
		return false;
	}

	public static Transform FindAncestorChildWithTag(string tagToFind, Transform obj)
	{
		Transform child = obj;
		obj = obj.parent;

		while (obj != null)
		{
			if (obj.CompareTag(tagToFind))
				return child;

			child = obj;
			obj = obj.parent;
		}
		return null;
	}
}
