using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{

    public Text hostName;
    public Text prvIp;
    public Text pubIp;
    public Text cpuUsage;
    public Button removeButton;
    public Button cancelButton;
    public GameObject modalPanelObject;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string hostname, string prIp, string pbIp, string cpuusage, UnityAction removeEvent, UnityAction cancelEvent)
    {
        modalPanelObject.SetActive(true);

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(removeEvent);
        removeButton.onClick.AddListener(ClosePanel);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);

        this.hostName.text = hostname;
        this.prvIp.text = prIp;
        this.pubIp.text = pbIp;
        this.cpuUsage.text = cpuusage;

        this.hostName.gameObject.SetActive(true);
        this.prvIp.gameObject.SetActive(true);
        this.pubIp.gameObject.SetActive(true);
        this.cpuUsage.gameObject.SetActive(true);
        removeButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        //Debug.Log("Starting panel coroutine");
        //StartCoroutine(ShowPanel(hostname));
    }
    IEnumerator ShowPanel(string name)
    {
        while (true)
        {
            var gameObject = GameObject.Find(name);
            this.cpuUsage.text = gameObject.GetComponentInChildren<Transform>().Find("cpuUsage").GetComponent<TextMesh>().text;
            //Debug.Log("Updating CPU usage for panel");
            yield return new WaitForSeconds(4);
        }
    }
    void ClosePanel()
    {
        //Debug.Log("Stoping panel coroutine");
        //StopCoroutine("ShowPanel");
        modalPanelObject.SetActive(false);
    }
}