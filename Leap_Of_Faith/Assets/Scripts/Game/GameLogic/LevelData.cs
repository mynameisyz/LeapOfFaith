using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData : MonoBehaviour 
{
	// -- Exposed For Unity Editor --
	public GameObject boundingObject;
	public GameObject[] checkpoints;
	public GameObject[] switches;

	private Bounds levelBounds;
	public Bounds LevelBounds
	{
		get { return levelBounds; }
	}

	#region Singleton

	private static LevelData instance = null;
	public static LevelData Instance 
	{ 
		get { return instance; } 
	}

	void Awake()
	{
		instance = this;
		levelBounds = boundingObject.renderer.bounds;
	}

	#endregion

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	#region Helper_Funcs

	public int Checkpoints_IndexOf(GameObject target)
	{
		int index = 0;

		foreach (GameObject obj in checkpoints)
		{
			if (obj == target)
				return index;

			index++;
		}

		return -1;
	}

	#endregion

	#region Switches

	[RPC]
	private void DoSwitchTrigger(int index)
	{
		SwitchTrigger switchScript = switches[index].GetComponent<SwitchTrigger>();

		foreach (GameObject ghost in switchScript.ghostsToSwitch)
		{
			if (ghost.CompareTag("GhostRed"))
				ghost.tag = "GhostBlue";
			else if (ghost.CompareTag("GhostBlue"))
				ghost.tag = "GhostRed";
			else
			{
				Transform ancestorChild = GameObjectHelper.FindAncestorChildWithTag("GhostRed", ghost.transform);
				if (ancestorChild != null)
					ancestorChild.parent = GameObject.Find("GhostBlue").transform;
				else
				{
					ancestorChild = GameObjectHelper.FindAncestorChildWithTag("GhostBlue", ghost.transform);
					if (ancestorChild != null)
						ancestorChild.parent = GameObject.Find("GhostRed").transform;
				}
			}
		}

		GhostBehaviour.UpdateGhostBehaviour();

		if (switchScript.isReusable == false)
		{
			Destroy (switchScript);
		}
	}

	public void RPC_DoSwitchTrigger(GameObject target)
	{
		if (Network.isServer)
			networkView.RPC("DoSwitchTrigger", RPCMode.All, Switches_IndexOf(target));
	}

	private int Switches_IndexOf(GameObject target)
	{
		int index = 0;

		foreach (GameObject obj in switches)
		{
			if (obj == target)
				return index;

			index++;
		}

		return -1;
	}

	#endregion
}
