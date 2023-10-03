using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarCanvas : MonoBehaviour
{
    private Canvas _canvas;
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = GameObject.Find("First_Person_Camera").GetComponent<Camera>();
        _canvas.planeDistance = 5;
    }
}
