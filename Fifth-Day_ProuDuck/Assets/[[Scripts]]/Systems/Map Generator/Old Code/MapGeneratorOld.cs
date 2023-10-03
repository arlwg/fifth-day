using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGeneratorOld : MonoBehaviour
{
    // The size of each chunk
    public int chunkSize = 16;
    // The player object
    public GameObject player;
    // The prefab for the ground block
    public GameObject groundBlock;
    // The parent object for all the chunks
    public GameObject chunksParent;
    
    private Dictionary<string, ChunkData> chunksData = new();
    
    private List<string> activeChunks = new();
    private List<string> destroyedChunks = new();
    
    private Vector3 playerPos;
    private Vector3 lastPlayerPos;
    
    
    private int playerChunkX;
    private int playerChunkZ;

    public Block Grassblock;

    private void Start()
    {
        // Generate the initial chunks
        GenerateChunks();
    }

    private void Update()
    {
        // Update the player's position
        playerPos = player.transform.position;
        playerChunkX = (int)Mathf.Floor(playerPos.x / chunkSize);
        playerChunkZ = (int)Mathf.Floor(playerPos.z / chunkSize);
        
        // Check if the player has moved to a new chunk
        if (playerChunkX != (int)Mathf.Floor(lastPlayerPos.x / chunkSize) || playerChunkZ != (int)Mathf.Floor(lastPlayerPos.z / chunkSize))
        {
            Debug.Log("Generating Chunks");
            GenerateChunks();
        }

        lastPlayerPos = playerPos;
    }

    private void GenerateChunks()
    {
        
        // Generate new chunks
        for (int x = playerChunkX - 1; x <= playerChunkX + 1; x++)
        {
            for (int z = playerChunkZ - 1; z <= playerChunkZ + 1; z++)
            {
                
                string chunkKey = x + "_" + z;
                if (!chunksData.ContainsKey(chunkKey))
                {
                    
                    // Create a new chunk
                    GameObject newChunk = new GameObject
                    {
                        transform =
                        {
                            parent = chunksParent.transform,
                            position = new Vector3(x * chunkSize, 0, z * chunkSize)
                        },
                        name = "Chunk " + x + ", " + z
                    };
                    // Generate the ground blocks for the new chunk
                    //Create the new chunk data for (the parent of each block).
                    ChunkData chunkData = newChunk.AddComponent<ChunkData>();
                    
                    //TODO: This area here needs some work, the blocks should be more random, and possibly have some structures placed occasionally.
                    for (int i = 0; i < chunkSize; i++)
                    {
                        for (int j = 0; j < chunkSize; j++)
                        {
                            
                            //TODO: Instead of Grassblock.prefab, will randomize according to an algorithm...
                            GameObject newGroundBlock = Instantiate(Grassblock.prefab, new Vector3(x * chunkSize + i, 0, z * chunkSize + j), Quaternion.identity);
                            newGroundBlock.transform.parent = newChunk.transform;
                            chunkData.blocks[i, j] = newGroundBlock;

                        }
                    }
                    
                    chunksData.Add(chunkKey, chunkData);
                    if (!activeChunks.Contains(chunkKey)) {
                        activeChunks.Add(chunkKey);
                    }
                }
                else
                {
                    //TODO : Load the data for the chunk and use it to generate the chunk
                    ChunkData chunkData = chunksData[chunkKey];
                    // Use chunkData to generate the chunk
                    for (int i = 0; i < chunkSize; i++)
                    {
                        for (int j = 0; j < chunkSize; j++)
                        {
                            GameObject newGroundBlock = Instantiate(groundBlock, new Vector3(x * chunkSize + i, 0, z * chunkSize + j), Quaternion.identity);
                            newGroundBlock.transform.parent = chunkData.transform;
                        }
                    }
                }
            }
            
        }
        
        
        // Destroy any chunks that are too far from the player
        string[] keys = chunksData.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            string[] splitKey = key.Split('_');
            int chunkX = int.Parse(splitKey[0]);
            int chunkZ = int.Parse(splitKey[1]);

            if (Mathf.Abs(chunkX - playerChunkX) > 3 || Mathf.Abs(chunkZ - playerChunkZ) > 3)
            {
               Debug.Log("Destroying!! : " + key);
                ChunkData chunkData = chunksData[key];
                if (chunkData.gameObject != null)
                {
                    Destroy(chunkData.gameObject);
                }

                chunksData.Remove(key);

                if (!destroyedChunks.Contains(key)) {
                    destroyedChunks.Add(key);
                }
            }
        }

    }
}
