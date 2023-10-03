using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class TerrainData : UpdateData
{
    public AnimationCurve animCurve;


   

    public float meshHeightMultiplier;
    //randomizes noise map
   
    public float uniformscale = 1f;

    public bool useFalloff;


    public bool useFlatShading;
}
