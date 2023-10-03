using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Credit to Sebastian Lague - https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
//Generates a two-dimensional noise map using the Perlin noise algorithm.
public class Noise
{

    public enum NormalizeMode
    {
        Local,
        Global
    };
    //Called from MapGenerator, returns perlin noise map.
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistanceValue, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //Create a "random" number generator with a seed.
        System.Random prng = new System.Random(seed);
        
        //Stores offset values for each octave
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            //Generate random offset values for reach octave;
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistanceValue;
        }
        if (scale <= 0)
        {
            scale = 0.0001f;   
        }

        //Initialize variables to store min/max noise height
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        
        //Half the width and height, used to ensure our noise scales from the center of the screen instead of the corner.
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth  + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight  + octaveOffsets[i].y) / scale * frequency;
                    
                    //Calculate Perlin noise value and add to noise height
                    float perlinValue =  Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    
                    // Update amplitude and frequency values
                    amplitude *= persistanceValue;
                    frequency *= lacunarity;
                    
                }
                
                
                //Update min/max noise height as needed, used to normalize the values between 0 and 1.
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                //Store the height value in the noise map
                noiseMap[x, y] = noiseHeight;
            }
            
        }


        //Normalize noise map values between 0 and 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
                   
            }
        }

        return noiseMap;
    }
    
    
}