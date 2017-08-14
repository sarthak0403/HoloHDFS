using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateColor : MonoBehaviour {

    private Color prevcolor;
    float duration = 1.5f;
    private float t = 0;
    bool isReset = false;

    void OnMouseEnter()
    {
        //prevcolor = gameObject.GetComponent<Renderer>().material.color;
        //gameObject.GetComponent<Renderer>().material.color = Color.red;

    }

    void OnMouseExit()
    {
        //gameObject.GetComponent<Renderer>().material.color = prevcolor;
    }
    // Use this for initialization
    void Start () {
        //prevcolor = gameObject.GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
        //colorChanger();
	}
    void colorChanger()
    {
        //gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
