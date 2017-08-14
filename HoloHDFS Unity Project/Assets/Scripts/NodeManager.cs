using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using UnityEngine.Networking;

public class NodeManager : MonoBehaviour {

    string url;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AddInstance()
    {
        /******Replace the IP in the following URL with the IP of your hadoop-master node.*****/
        url = "http://0.0.0.0:5000/launch";
        // Create a GET web request and store it
        UnityWebRequest apiRequestadd = UnityWebRequest.Get(url);
        // Wait until the data has been received before continuing the loop
        //yield return apiRequest.Send();
        apiRequestadd.Send();
        //displayManager.DisplayMessage("Instance Added Successfully!!");
        //Debug.Log(apiRequest.downloadHandler.text);
    }
}
