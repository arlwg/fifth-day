using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorOld : MonoBehaviour
{
    private Mesh mesh;
    

    private Vector3[] vertices;
    private int[] triangles;

    public int xSize = 20;
    public int zSize = 20;


    public int textureWidth = 1024;
    public int textureHeight = 1024;

    public float noise01Scale = 2f;
    public float noise01Amp = 2f;


    public float noise02Scale = 4f;
    public float noise02Amp = 4f;
    

    [Range(0,5)]
    public float perlinNoiseFactor = 2;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        StartCoroutine(CreateShape());
    }

    private void FixedUpdate()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
       
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * perlinNoiseFactor; 
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        
        
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
            
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;


                vert++;
                tris += 6;
                
            }

            yield return new WaitForSeconds(0.05f);
            vert++;
        }
        
        
    }
    
}
