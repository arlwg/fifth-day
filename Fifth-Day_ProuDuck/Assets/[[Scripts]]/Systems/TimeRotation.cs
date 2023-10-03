using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeRotation : MonoBehaviour
{
    [SerializeField]
    private TimeController timeController;

    [SerializeField]
    private Image image;

    private float rotationTime;

    // Start is called before the first frame update
    void Start()
    {
        rotationTime = timeController.sunLightRotation;
    }

    // Update is called once per frame
    void Update()
    {
        rotationTime = timeController.sunLightRotation;
        image.transform.rotation = Quaternion.AngleAxis(rotationTime, Vector3.forward);
    }
}
