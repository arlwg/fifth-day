using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    public GameObject[,] blocks;
    //public Transform tranform;
    public int terrainType;

    public ChunkData()
    {
        blocks = new GameObject[16, 16];
        terrainType = 0;
    }
}
