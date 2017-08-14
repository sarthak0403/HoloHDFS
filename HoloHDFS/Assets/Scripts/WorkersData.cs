using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkersData : MonoBehaviour {

    // Create variables 
    private string url;
    private bool isRunning;
    private string workerName;
    private string prvIp;
    private string pubIp;
    private string cpuUsage;
    private string overUtil;
    private string underUtil;
    //private Transform cubeTransform;
    private Vector3 tempScale;
    public GameObject serverPrefab;
    private GameObject[] workers;
    public GameObject master;
    private List<string> serverWorkers;
    private List<workerDataType> apiWorkers;

    private DisplayManager displayManager;
    private Vector3 pos;

    // Use this for initialization
    void Start()
    {
        // Get and store the cube's transform
        //cubeTransform = gameObject.transform;
        // Assign the URI to a variable so that it's easier to handle
        url = "http://54.213.229.10:5000/workers";
        pos = new Vector3(0, 0, 0);
        if (workers == null)
        {
            workers = GameObject.FindGameObjectsWithTag("Worker");
        }
        displayManager = DisplayManager.Instance();
        // Start coroutine to get Particle light data 
        Debug.Log("Starting coroutine");
        StartCoroutine(GetData());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            StopCoroutine(GetData());
            Debug.Log("Starting coroutine again");
            StartCoroutine(GetData());
        }
    }

    // Create coroutine
    public IEnumerator GetData()
    {
        // This while loop will repeat as long as our application runs
        while (true)
        {
            isRunning = true;
            // Create a GET web request and store it
            UnityWebRequest apiRequest = UnityWebRequest.Get(url);
            // Wait until the data has been received before continuing the loop
            yield return apiRequest.Send();
            //Debug.Log(apiRequest.downloadHandler.text);

            // Create a new JSON object from the text we receive back from the request
            JSONObject data = new JSONObject(apiRequest.downloadHandler.text);
            //Debug.Log(data);
            master = GameObject.FindGameObjectWithTag("Master");
            try
            {
                if (data.type != JSONObject.Type.NULL)
                {
                    setBlockReport(data);
                    masterCPUUsage(data);
                    enableHalo(master);
                    getApiWorkers(data);
                    checkNewWorker(data);
                }
            }
            catch (Exception e)
            {
                StopCoroutine(GetData());
                Debug.LogError("Error in workersdata:" + e.Message);
                //Debug.Log(apiRequest.downloadHandler.text);
                //Debug.Log(data);
                StartCoroutine(GetData());
            }

            // Wait for 1 second before restarting the loop 
            yield return new WaitForSeconds(1);
        }
        isRunning = false;
    }
    void checkNewWorker(JSONObject data)
    {
        workers = GameObject.FindGameObjectsWithTag("Worker");
        if (workers.Length == 0) {
            pos = master.transform.position;
            pos.x -= 3.0f;
            pos.z = -0.5f;
            pos.y -= 1.0f;
        }
        else {
            pos = workers[(workers.Length - 1)].transform.position;
            pos.z = -0.5f;
            if (workers.Length % 3 == 0) {
                pos.x -= 6.0f;
                pos.y -= 1.0f;
            }
            else {
                pos.x += 3.0f;
            }
        }

        //Destroying old workers from the scene
        foreach (GameObject w in workers)
        {
            var apiW = apiWorkers.Where(x => x.hostname == w.name).FirstOrDefault();
            if (apiW != null) {
                w.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>().text = apiW.cpuUsage;// + "%";
                enableHalo(w);
            }
            if (apiW == null) {
                Debug.Log("Destroying Object with name: "+w.name);
                Destroy(w);
                displayManager.DisplayMessage("Instance Removed!!");
            }
        }

        //Adding new workers to the scene
        workers = GameObject.FindGameObjectsWithTag("Worker");
        foreach (var apiWorker in apiWorkers)
        {
            GameObject temp = GameObject.Find(apiWorker.hostname);
            if (temp == null)
            {
                var newobj = Instantiate(serverPrefab, pos, Quaternion.Euler(0, 220, 0));
                pos.x += 3.0f;
                if (GameObject.FindGameObjectsWithTag("Worker").Length % 3 == 0)
                {
                    pos.y -= 1.5f;
                    pos.x -= 9.0f;
                }
                var ptransform = GameObject.Find("ClusterCollection").transform;
                newobj.transform.SetParent(ptransform, false);
                newobj.gameObject.tag = "Worker";
                newobj.name = apiWorker.hostname;
                newobj.transform.localScale += new Vector3(0.0008f, 0.0008f, 0.0008f);

                var t = newobj.GetComponentInChildren<Transform>().Find("PrvIp").GetComponent<TextMesh>();
                t.text = apiWorker.prvIp;
                t = newobj.GetComponentInChildren<Transform>().Find("PubIp").GetComponent<TextMesh>();
                t.text = apiWorker.pubIp;
                t = newobj.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>();
                t.text = apiWorker.cpuUsage;// + "%";
                enableHalo(newobj);
                Debug.Log("Newly loaded server: " + newobj.name + " " + newobj.tag);
            }
        }
    }

    void masterCPUUsage(JSONObject data)
    {
        //Set Master Node CPU Usage
        var masterJSON = data.GetField("master");
        var mcpu = master.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>();
        mcpu.text = masterJSON.ToString().Replace("\"", "");// + "%";
        //Debug.Log("Master CPU Usage: "+masterJSON.ToString().Replace("\"", ""));
    }

    void enableHalo(GameObject obj)
    {
        var halo = obj.GetComponentInChildren<Transform>().Find("Cube").GetComponent("Halo") as Behaviour;
        var usage = obj.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>().text.Replace("%", "");
        if (float.Parse(usage) >= 2.0)
        {
            halo.enabled = true;
            //Debug.Log("Inside Halo if");
        }
        else
        {
            halo.enabled = false;
            //Debug.Log("Inside Halo else");
        }
    }

    void setBlockReport(JSONObject data)
    {
        //Setting the block replication data
        var overrep = data.GetField("over");
        var underrep = data.GetField("under");
        var overvalue = GameObject.Find("OverValue").GetComponent<Text>();
        var undervalue = GameObject.Find("UnderValue").GetComponent<Text>();
        overvalue.text = overrep.ToString().Replace("\"", "");
        undervalue.text = underrep.ToString().Replace("\"", "");
        //Debug.Log("After setting the block replication data");
    }
    void getApiWorkers(JSONObject data)
    {
        var workersJSON = data.GetField("workers");
        apiWorkers = new List<workerDataType>();
        for (int i = 0; i < workersJSON.list.Count; i++)
        {
            JSONObject workerData = (JSONObject)workersJSON.list[i];
            apiWorkers.Add(new workerDataType
            {
                hostname = workersJSON.keys[i].ToString(),
                prvIp = workerData.GetField("prvIp").ToString().Replace("\"", ""),
                pubIp = workerData.GetField("pubIp").ToString().Replace("\"", ""),
                cpuUsage = workerData.GetField("CPU").ToString().Replace("\"", "")
            });
        }
    }
}
public class workerDataType
{
    public string hostname { get; set; }
    public string prvIp { get; set; }
    public string pubIp { get; set; }
    public string cpuUsage { get; set; }
}
