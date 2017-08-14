using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class NodeSelector : MonoBehaviour, IInputClickHandler, IInputHandler
{
    private ModalPanel modalPanel;
    private DisplayManager displayManager;

    private UnityAction myRemoveAction;
    private UnityAction myCancelAction;
    private string url;

    private GameObject[] workers;
    private string workerName;
    private string prvIp;
    private string pubIp;
    private string cpuUsage;
    private Transform gameObject;
    private string gameObjectName;

    void Awake()
    {
        modalPanel = ModalPanel.Instance();
        displayManager = DisplayManager.Instance();

        myRemoveAction = new UnityAction(TestRemoveFunction);
        myCancelAction = new UnityAction(TestCancelFunction);
    }

    //Send to the Modal Panel to set up the Buttons and Functions to call
    public void TestYNC()
    {
        modalPanel.Choice("Host Name", "Private IP", "Public IP", "CPU Usage", TestRemoveFunction, TestCancelFunction);
    }

    //  These are wrapped into UnityActions
    void TestRemoveFunction()
    {
        //displayManager.DisplayMessage("Instance Removed!!");
    }

    void TestCancelFunction()
    {
        displayManager.DisplayMessage("Cancelled!");
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        try
        {
            // AirTap code goes here

            // ** Tap changes the text UI element to display the server name
            var hit = GazeManager.Instance.HitInfo;
            if (hit.transform.gameObject == null) return;
            gameObject = hit.transform.parent;
            gameObjectName = gameObject.name;
            prvIp = gameObject.GetComponentInChildren<Transform>().Find("PrvIp").GetComponent<TextMesh>().text;
            pubIp = gameObject.GetComponentInChildren<Transform>().Find("PubIp").GetComponent<TextMesh>().text;
            cpuUsage = gameObject.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>().text;
            modalPanel.Choice(gameObjectName, prvIp, pubIp, cpuUsage, myRemoveAction, myCancelAction);
            //var textResult = GameObject.Find("ResultTextOld");
            //var pos = hit.transform.position;
            //pos.y += 0.3f;
            //pos.x -= 0.2f;
            //textResult.transform.position = pos;
            //textResult.GetComponent<TextMesh>().text = gameObjectName;
            //textResult.SetActive(true);
            ////Color c = hit.GetComponent<Renderer>().material.color;
            ////c.a = 0.5f;
            ////hit.GetComponent<Renderer>().material.color = c;
        }
        catch (Exception e)
        {
            StopCoroutine(Camera.main.GetComponent<WorkersData>().GetData());
            Debug.LogError("Error in NodeSelector onclick:" + e.Message);
            StartCoroutine(Camera.main.GetComponent<WorkersData>().GetData());
        }
    }
    public void AddInstance()
    {
        url = "http://54.213.229.10:5000/launch";
        // Create a GET web request and store it
        UnityWebRequest apiRequest = UnityWebRequest.Get(url);
        // Wait until the data has been received before continuing the loop
        //yield return apiRequest.Send();
        apiRequest.Send();
        displayManager.DisplayMessage("Instance Added Successfully!!");
        //Debug.Log(apiRequest.downloadHandler.text);
    }
    public void RemoveInstance(GameObject item)
    {
        try
        {
            //StopCoroutine("ShowPanel");

            var ip = item.GetComponent<Text>();
            //Debug.Log(ip.text);
            if (ip.text == "172.31.24.140" || ip.text == "54.213.229.10")
            {
                displayManager.DisplayMessage("Cannot remove master node!!");
                return;
            }
            if (ip.text == "172.31.1.212" || ip.text == "172.31.9.89")
            {
                displayManager.DisplayMessage("Cannot remove the default data nodes!!");
                return;
            }
            string hostname = "ip-" + ip.text.Replace(".", "-");
            var server = GameObject.Find(hostname);
            url = "http://54.213.229.10:5000/destroy?ip=" + ip.text;
            // Create a GET web request and store it
            UnityWebRequest apiRequestDestroy = UnityWebRequest.Get(url);
            //Destroy(server);
            apiRequestDestroy.Send();
            displayManager.DisplayMessage("Destroy Request Received!!");
        }
        catch (Exception e)
        {
            StopCoroutine(Camera.main.GetComponent<WorkersData>().GetData());
            Debug.LogError("Error in NodeSelector removeInstance:" + e.Message);
            StartCoroutine(Camera.main.GetComponent<WorkersData>().GetData());
        }
    }

    public void OnInputDown(InputEventData eventData)
    { }
    public void OnInputUp(InputEventData eventData)
    { }
}
