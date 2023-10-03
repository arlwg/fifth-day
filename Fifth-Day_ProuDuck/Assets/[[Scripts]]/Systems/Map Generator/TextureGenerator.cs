using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator 
{
    //generates a Texture2D object from a height map.
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        //Get the width and height of the height map (each dimension of the array)
        int width = heightMap.GetLength (0);
        int height = heightMap.GetLength (1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                
                //Converts the height value to a color using a gradient.
                colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
    
    //returns a Texture2D object from a color map.
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        //Set to point in order to avoid smoothing pixels.
        texture.filterMode = FilterMode.Point;
        //Clamp prevents texture from repeating.
        texture.wrapMode = TextureWrapMode.Clamp;
        //Set the pixels of the texture to the colour map passed in.
        texture.SetPixels(colourMap);
        texture.Apply();
        
        
        //returns the generated texture
        return texture;
    }
    
    
    
    
}
