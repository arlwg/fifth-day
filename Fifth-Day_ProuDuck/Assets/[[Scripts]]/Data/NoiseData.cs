using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdateData
{
    
    public float noiseScale;

    //Controls of the number of iterations in the noise function, higher = more detail.
    public int octaves;
    //Persistence determines the relative amplitude of each octave
    [Range(0,1)]
    public float persistance;
    
    //Lacunarity determines the frequency multiplier between each octave
    public float lacunarity;
    
    public int seed;
    
    //Offset for noise function
    public Vector2 offset;


    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
        
        
        base.OnValidate();
    }
}
