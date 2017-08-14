using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeHandler : Singleton<GazeHandler>
{
    private Color startColor;
    void OnGazeEnter()
    {
        var com = gameObject.GetComponent<Renderer>();
        startColor = com.material.color;
        com.material.color = Color.yellow;
    }

    void OnGazeExit()
    {
        var com = gameObject.GetComponent<Renderer>();
        com.material.color = startColor;
    }
}