using UnityEngine;
using System.Collections;
using OpenNI;
using NITE;

public class NITESessionManager : MonoBehaviour 
{
	private OpenNIContext Context;
	public GUIStyle myGuiStyle;

	SessionManager sessionManager;
	Broadcaster broadcaster;
	
	private bool isInSession;
	public bool IsInSession
	{
		get { return isInSession; }
	}
	
	public Point3D FocusPoint
	{
		get { return sessionManager.FocusPoint; }
	}
	
	public void AddListener(MessageListener listener)
	{
		if (null == broadcaster) broadcaster = new Broadcaster();
		broadcaster.AddListener(listener);
	}
	
	public void RemoveListener(MessageListener listener)
	{
		if (null == broadcaster) broadcaster = new Broadcaster();
		broadcaster.RemoveListener(listener);
	}
	
	public int QuickRefocusTimeout
	{
		get { return sessionManager.QuickRefocusTimeout ; }
		set { sessionManager.QuickRefocusTimeout = value; }
	}
		
	void Awake()
	{
		if (null == broadcaster) broadcaster = new Broadcaster();
	}
	
	// Use this for initialization
	void Start () {
		// Make sure we have a valid OpenNIContext
		Context = OpenNIContext.Instance;
		if (null == Context)
		{
			print("OpenNI not inited");
			return;
		}

		// init session manager
		sessionManager = new SessionManager(Context.context, "Click", "RaiseHand");
        sessionManager.SessionStart += new System.EventHandler<PositionEventArgs>(sessionManager_SessionStart);
        sessionManager.SessionEnd += new System.EventHandler(sessionManager_SessionEnd);
		
		sessionManager.AddListener(broadcaster);
	}

	void sessionManager_OnContextShutdown(object sender, System.EventArgs e)
	{
		print("Context gone");
		sessionManager.Dispose();
		Context = null;
	}
	
    void sessionManager_SessionEnd(object sender, System.EventArgs e)
    {
        print("Session end");
        isInSession = false;
        GUI.enabled = false;
    }

    void sessionManager_SessionStart(object sender, PositionEventArgs e)
    {
        print("Session start");
        isInSession = true;
        GUI.enabled = true;
    }
	
	// Update is called once per frame
	void Update () 
	{
		if (Context.ValidContext)
		{
            // print("Update - Session Manager");
			Context.Update();
			sessionManager.Update(Context.context);
		}
	}
	void OnApplicationQuit()
	{
		sessionManager.Dispose();
	}
}
