using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JobManager : MonoBehaviour {

    string url;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartJob()
    {
        url = "http://54.213.229.10:5000/startjob";
        // Create a GET web request and store it
        UnityWebRequest apiRequeststart = UnityWebRequest.Get(url);
        apiRequeststart.Send();
    }
    public void StopJob()
    {
        url = "http://54.213.229.10:5000/stopjob";
        // Create a GET web request and store it
        UnityWebRequest apiRequeststop = UnityWebRequest.Get(url);
        apiRequeststop.Send();
    }
}
