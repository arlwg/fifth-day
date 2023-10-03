using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Block")]
public class Block : ScriptableObject
{
    public int blockID;
    public string blockName;
    public bool isSolid;
    public GameObject prefab;


    private void Awake()
    {
        
    }
}
