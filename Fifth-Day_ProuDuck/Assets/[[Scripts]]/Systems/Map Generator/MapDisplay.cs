using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapDisplay : MonoBehaviour
{

    public Renderer textureRender;

    public MeshFilter MeshFilter;

    public MeshRenderer MeshRenderer;
    //Draws to our plane/textureRender
    public void DrawTexture(Texture2D texture)
    {
        //Used sharedMaterial.mainTexture because material.mainTexture is not instantiated until runtime.. this way we get preview in editor.
        textureRender.sharedMaterial.mainTexture = texture;
        //Change size of plane accordingly.
        textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
    }


    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        MeshFilter.sharedMesh = meshData.CreateMesh();
        MeshRenderer.sharedMaterial.mainTexture = texture;
    }

}

