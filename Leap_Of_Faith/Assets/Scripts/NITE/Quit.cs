using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour {
	
	public bool killProcess = true;
	
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("Quiting");
			//OpenNIContext.Instance.ValidContext = false;
			//GameObject.Destroy(player);
			//GameObject.Destroy(this.gameObject);
			//System.Environment.Exit(0);
			Application.Quit();
			
		}
	}
	
	void OnApplicationQuit()
	{
		if(killProcess)
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}
