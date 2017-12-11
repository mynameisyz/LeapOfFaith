using UnityEngine;
using System.Collections;

public class KillerLitterSpawner : MonoBehaviour 
{
	public Transform killerLitterPrefab;
	
	public float spawnTimer = 3.0f;
	public float timeToRespawn = 3.0f;
	
	private Transform myLitter;
	
	private Transform myTransform;

	void Awake()
	{
		if (!Network.isServer)
			this.enabled = false;
	}

	// Use this for initialization
	void Start () 
	{
		myTransform = this.transform;
		spawnTimer = Random.Range(0.0f, 3.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		spawnTimer += Time.deltaTime;
		
		if (spawnTimer >= timeToRespawn)
		{
			spawnTimer = 0;
			if (myLitter != null)
				Network.Destroy(myLitter.gameObject);
			
			myLitter = (Transform)Network.Instantiate(killerLitterPrefab, myTransform.position, new Quaternion(45, 90, 0 ,0), 0);
			myLitter.GetComponent<KillerLitterBehaviour>().RPC_SetGhostColor(this.CompareTag("GhostRed"));
		}
	}
}
