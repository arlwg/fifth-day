using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(TerrainGenerator))]
public class PlacementGenerator : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGeneratorGenerator;
    [Header("Outpost")]
    [SerializeField] private GameObject outpostPrefab;
    
    [SerializeField] private MapObject[] mapObjects;

    [Header("Settings")] 
    [SerializeField] private int density;

    [SerializeField]private float minHeight = -500;
    [SerializeField] private float maxHeight;
    
    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 zRange;

    public bool outpostSpawned = false;

    [Header("Variation")]
    [SerializeField] private Vector3 rotation;

    
    public event Action OnOutpostSpawned;
    private void Start()
    {
        terrainGeneratorGenerator = this.GetComponent<TerrainGenerator>();
    }

    public IEnumerator SpawnObjects(int density, Vector2 xRange, Vector2 zRange, Vector3 origin, Vector2 coord)
    {
        yield return new WaitForSeconds(1.0f);
        GenerateObjects(density, xRange, zRange, origin, coord);
    }
    public void SpawnOutpost(Vector3 bestAreaPosition)
    {
        if (outpostSpawned)
            return;

        // Calculate the center position of the best area
        float centerX = bestAreaPosition.x + 50 / 2f;
        float centerZ = bestAreaPosition.z + 10 / 2f;
        Vector3 rayStart = new Vector3(centerX, maxHeight, centerZ);

        int layerMask = ~LayerMask.GetMask("DeathPlane", "Monster", "Player");

        // Raycast at the center position of the best area to get the hit position
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 spawnPosition = hit.point;
            spawnPosition.y = 2.5f;
            Debug.Log("Spawning OUTPOST!! : " + spawnPosition);

            // Instantiate the outpost prefab at the spawn position
            GameObject outpostInstance = Instantiate(outpostPrefab, spawnPosition, Quaternion.identity);

            // Optionally, set the parent for the instantiated outpost (e.g., to keep the scene hierarchy organized)
            outpostInstance.transform.SetParent(transform);
            outpostSpawned = true;
            DialogueTrigger[] triggers = FindObjectsOfType<DialogueTrigger>();
            foreach (var trigger in triggers)
            {
                FindObjectOfType<UIManager>().DialogueTriggers.Add(trigger);
            }
            OnOutpostSpawned?.Invoke();
        }
        else
        {
            Debug.Log("Could not spawn outpost, no hit at the center position");
        }
    }
    //Generate Objects in Chunk.
    public void GenerateObjects(int density, Vector2 xRange, Vector2 zRange, Vector3 origin, Vector2 coord)
    {
        Debug.Log("spawning objects");
        System.Random prng = new System.Random(FindObjectOfType<MapGenerator>().noiseData.seed);
        
        MapObject[] instantiatedPrefabs = new MapObject[density];
        for (int i = 0; i < density; i++)
        {
           // Debug.Log("Generating an object."); TODO : Use the saved seed / prng random here.
            float sampleX = UnityEngine.Random.Range(xRange.x, xRange.y);
            float sampleY = UnityEngine.Random.Range(zRange.x, zRange.y);
            //Debug.Log(origin.x + " : " + origin.y);
            Vector3 rayStart =  new Vector3(origin.x + sampleX, maxHeight, origin.y  + sampleY);
            int deathLayer = LayerMask.NameToLayer("DeathPlane");
             int enemyLayer = LayerMask.NameToLayer("Monster");
             int playerLayer = LayerMask.NameToLayer("Player");
            int layerMask = ~(1 << deathLayer) & ~(1 << enemyLayer) & ~(1 << playerLayer);
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("NO hit in " + origin + "I amount : " + i);
                continue;
            }
                
            if (hit.point.y < minHeight)
                continue;
            
            MapObject instantiatePrefab = Instantiate(mapObjects[prng.Next(0,2)], hit.transform);
            if (instantiatePrefab.meshRenderer != null)
            {
                instantiatePrefab.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }
           
            instantiatedPrefabs[i] = instantiatePrefab;
            instantiatePrefab.gameObject.transform.position = hit.point;
            //Debug.Log("Generating"+ instantiatePrefab.name + " at : " + hit.point + " at chunk coord : " + coord);
            instantiatePrefab.gameObject.transform.localScale = new Vector3( UnityEngine.Random.Range(0.5f, 1f),
                UnityEngine.Random.Range(0.5f, 1f),
                UnityEngine.Random.Range(0.5f, 1f));
           // Debug.Log("Generating"+ instantiatePrefab.name + " at : " + hit.point);

        }


        terrainGeneratorGenerator.FindChunkOfCoord(coord).chunkObjects = instantiatedPrefabs;
    }

    public Vector2 FindBestSuitableArea(int areaWidth, int areaHeight, Vector2 xRange, Vector2 zRange, int resolution)
    {
        int mapWidth = Mathf.FloorToInt((xRange.y - xRange.x) * resolution);
        int mapHeight = Mathf.FloorToInt((zRange.y - zRange.x) * resolution);
        float[,] heightMap = new float[mapWidth, mapHeight];

        // Raycast to get terrain heights
        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float worldX = xRange.x + x / (float)resolution;
                float worldZ = zRange.x + z / (float)resolution;
                Vector3 rayStart = new Vector3(worldX, 10, worldZ);
                int layerMask = ~LayerMask.GetMask("DeathPlane", "Monster", "Player");

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    heightMap[x, z] = hit.point.y;
                }
                else
                {
                    heightMap[x, z] = float.MaxValue;
                }
            }
        }

        // Find the best suitable area
        float bestAverageHeight = float.MaxValue;
        Vector2 bestAreaBottomLeft = Vector2.zero;

        for (int z = 0; z < mapHeight - areaHeight; z++)
        {
            for (int x = 0; x < mapWidth - areaWidth; x++)
            {
                float areaHeightSum = 0;
                for (int areaZ = 0; areaZ < areaHeight; areaZ++)
                {
                    for (int areaX = 0; areaX < areaWidth; areaX++)
                    {
                        areaHeightSum += heightMap[x + areaX, z + areaZ];
                    }
                }

                float averageAreaHeight = areaHeightSum / (areaWidth * areaHeight);
                if (averageAreaHeight < bestAverageHeight)
                {
                    bestAverageHeight = averageAreaHeight;
                    bestAreaBottomLeft = new Vector2(xRange.x + x / (float)resolution, zRange.x + z / (float)resolution);
                }
            }
        }

        return new Vector3(bestAreaBottomLeft.x, bestAverageHeight, bestAreaBottomLeft.y);
    }


}
