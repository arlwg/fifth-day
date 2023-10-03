using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

//Inspiration + Credit to Sebastian Lague for Map Generation.

//Sends Map Data to TerrainGenerator.
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        Mesh,
        FalloffMap

    };

    public TerrainData terrainData;
    public NoiseData noiseData;
    public DrawMode drawMode;

    
    //Controls the size of the map, must be -2 from +1 off an even size. e.g 119 + 2 = 121.. other class subtracts 1 from this to make it even.
    //public const int mapChunkSize = 95;

    public bool useFlatshading;
    [Range(0, 6)] public int editorPrevLOD;

    public bool autoUpdate;

    //Array of terrain types and their properties (height, colour)
    public TerrainType[] regions;

    private static MapGenerator instance;
    private float[,] falloffMap;
    private List<Thread> threads = new List<Thread>();
    public Noise.NormalizeMode NormalizeMode;

#if !UNITY_WEBGL
    //Queue to store map data from worker threads
    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

    //Queue to store mesh data from worker threads
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
#endif

    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }


    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }
    
    public int mapChunkSize {
        get
        {


            if (terrainData == null)
                return 95;
            
            
            if (terrainData.useFlatShading) {
                return 95;
            } else {
                return 239;
            }
        }
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        //Draw the texture based on the draw mode selected
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(
                MeshGenerator.GenerateMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.animCurve, editorPrevLOD,
                    terrainData.useFlatShading),
                TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(
                TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
        }
    }



    public void MapDataNoThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        callback(mapData);
    }

    //Implement a method to retrieve mesh data without threading.
    public void MeshDataNoThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData =
            MeshGenerator.GenerateMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.animCurve, lod, terrainData.useFlatShading);
        callback(meshData);
    }

#if !UNITY_WEBGL
    // Method to request map data asynchronously in a worker thread
    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(centre, callback); };
        Thread newThread = new Thread(threadStart);
        newThread.Start();
        threads.Add(newThread);
    }


    //Method executed by the worker thread to generate and store map data
    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        //Ensure that when a thread reaches this point and is executing this code, no other thread can execute it at the same time.
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }



    //Method to request mesh data asynchronously in worker thread
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, lod, callback); };
        Thread newThread = new Thread(threadStart);
        newThread.Start();
        threads.Add(newThread);
    }

    public void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData =
            MeshGenerator.GenerateMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.animCurve, lod, terrainData.useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    private void OnDisable()
    {
        foreach (Thread thread in threads)
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread.Join();
            }
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }


#endif

    //Generates the noise map and color map and calls DrawTexture in MapDisplay component.
    MapData GenerateMapData(Vector2 centre)
    {
        //Generate the noise map
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2,  noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, centre + noiseData.offset, NormalizeMode);
        
        
        falloffMap ??= FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
        
        //Generate the color map based on the height values in the noise map.
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (terrainData.useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                
                //Find the region (terrain type) with a height value greater than or equal to the currentHeight
                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight >= regions [i].height) {
                        colourMap [y * mapChunkSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);
    }

    public float GetTerrainHeight(Vector3 position, float[,] heightMap)
    {
        // Convert position to heightMap coordinates
        int x = Mathf.FloorToInt((position.x / (float)mapChunkSize) * (mapChunkSize + 2));
        int y = Mathf.FloorToInt((position.z / (float)mapChunkSize) * (mapChunkSize + 2));

        // Clamp the coordinates to be within the bounds of the heightMap
        x = Mathf.Clamp(x, 0, mapChunkSize + 1);
        y = Mathf.Clamp(y, 0, mapChunkSize + 1);

        // Get the height from the heightMap
        float height = heightMap[x, y];

        // Apply terrain height transformations
        height *= terrainData.meshHeightMultiplier;
        height = terrainData.animCurve.Evaluate(height);

        return height;
    }
    
    private void OnValidate()
    {

        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }

        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    //Generic so it can handle MapData and MeshData
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
