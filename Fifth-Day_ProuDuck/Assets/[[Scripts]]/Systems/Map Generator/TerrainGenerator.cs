using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
	
	private PlacementGenerator _generator;
	static MapGenerator mapGenerator;

	const float viewerMoveThresholdForChunkUpdate = 150;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
	private const float colliderGenerationDistanceThreshold = 25;
	private const float navmeshGenerationDistanceThreshold = 5;
	

	public LODInfo[] detailLevels;
	public static float maxViewDst;

	public Transform viewer;
	public Material mapMaterial;

	public static Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	int chunkSize;
	int chunksVisibleInViewDst;
	public int colliderLODIndex = 0;
	public bool debug;
	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
	
	
	
	public event Action OnMapGenerationCompleted;
	public event Action OnNavMeshGenerationCompleted;
	// Call this method once the map generation is complete
	private void NotifyMapGenerationCompleted()
	{
		Debug.Log("Invoking On Map Generation Completed");
		OnMapGenerationCompleted?.Invoke();
	}

	private void NotifyNavMeshGenerationCompleted()
	{
		Debug.Log("Invoking On Nav Mesh Completed");
		OnNavMeshGenerationCompleted?.Invoke();
	}
	public Dictionary<Vector2, TerrainChunk> GetTerrainChunks()
	{
		return terrainChunkDictionary;
	}

	public void ClearTerrainChunks()
	{
		foreach (var terrainChunk in terrainChunkDictionary.Values)
		{
			if (terrainChunk.meshObject != null)
			{
				Destroy(terrainChunk.meshObject);
			}
		}

		foreach (var terrainChunk in terrainChunksVisibleLastUpdate)
		{
			if (terrainChunk.meshObject != null)
			{
				Destroy(terrainChunk.meshObject);
			}
		}
		terrainChunksVisibleLastUpdate.Clear();
		terrainChunkDictionary.Clear();
	}

	private void Awake()
	{
		ClearTerrainChunks();
	}

	void Start()
	{
		_generator = FindObjectOfType<PlacementGenerator>();
		mapGenerator = FindObjectOfType<MapGenerator> ();
		if(DataManager.Instance.selectedSave.seed != -1)
		{
			mapGenerator.noiseData.seed = DataManager.Instance.selectedSave.seed;
		}
		else if(debug)
		{
			Debug.Log("Map seed is set to : " + mapGenerator.noiseData.seed );
		}
		else
		{
			mapGenerator.noiseData.seed = Random.Range(1,100);
		}
		
		
		maxViewDst = detailLevels [^1].visibleDstThreshold;
		chunkSize = mapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
		
		//Start the first generation.
			UpdateVisibleChunks ();
	
		//StartCoroutine(GenerateNavMeshCo());
	}

	void Update() {
		//Update viewer position
		//Debug.Log("terrainData : " + mapGenerator.terrainData);
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z) / mapGenerator.terrainData.uniformscale;


		if (viewerPosition != viewerPositionOld)
		{
			foreach (TerrainChunk chunk in terrainChunksVisibleLastUpdate)	
			{
				chunk.UpdateCollisionMesh();
			}
		}
		//If the viewer has moved more than the threshold, update visible chunks.
		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
			GenerateTerrainObjects();
			//StartCoroutine(GenerateNavMesh());
		}
	}
	
	void UpdateVisibleChunks() {

		//Go through each chunk that was visible last frame.
		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			//Set it invisible
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();
		
		//Get the current chunk the viewer is in.
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);
		
		//Loop through all chunks in the view distance.
		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				//Calculate the chunk coordinates.
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				
				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					//Update the chunk and add it to the list of visible chunks.
					terrainChunkDictionary [viewedChunkCoord].UpdateChunk ();
				} else {
					Vector2 pos = viewedChunkCoord * chunkSize;
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, detailLevels, colliderLODIndex, transform, mapMaterial, _generator));
					
					//StartCoroutine(_generator.SpawnObjects(20, new Vector2(-chunkSize / 2, chunkSize / 2), new Vector2(-chunkSize / 2, chunkSize / 2), pos, viewedChunkCoord));
				}

			}
		}
		
		NotifyMapGenerationCompleted();
		
	}
	
	
	
	
	public void GenerateTerrainObjects()
	{
		foreach (var terrainChunk in terrainChunkDictionary)
		{
			if (!terrainChunk.Value.objectsGenerated)
			{
                
				Debug.Log("Spawning objects at : " + terrainChunk.Key);
				
				terrainChunk.Value.objectsGenerated = true;
			}
		}
	}
	
	
	
	public TerrainChunk FindChunkOfCoord(Vector2 coord)
	{
		return terrainChunkDictionary[coord];
	}
	
	
	//Credit to Sebastian Lague - https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
	public class TerrainChunk
	{

		public PlacementGenerator generator;
		public GameObject meshObject;
		Vector2 position;
		public Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		private MeshCollider _meshCollider;

		public bool objectsGenerated = false;
		public bool navMeshGenerated = false;
		public MapObject[] chunkObjects;
		public Vector2 coord;
		
		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;
		MapData mapData;
		bool mapDataReceived;
		private bool hasSetCollider = false;
		public bool hasGeneratedMapObjects = false;
		int previousLODIndex = -1;
		private int colliderLODIndex = 0;
		
		private int size;


		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Material material, PlacementGenerator generator) {
			this.detailLevels = detailLevels;
			this.coord = coord;
			this.colliderLODIndex = colliderLODIndex;
			this.generator = generator;
			this.size = size;
			
			position = coord * size;
			bounds = new Bounds(position,Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x,0,position.y);
			
			meshObject = new GameObject(coord.x + "," + coord.y);
			meshObject.isStatic = true;
			meshObject.layer = LayerMask.NameToLayer("Ground");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			_meshCollider = meshObject.AddComponent<MeshCollider>();
			meshObject.AddComponent<NavMeshSurface>();
			
			
			meshRenderer.material = material;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformscale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformscale;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			
			

			SetVisible(false);
			//Create the LOD meshes.
			lodMeshes = new LODMesh[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; i++) {
				lodMeshes[i] = new LODMesh(detailLevels[i].lod);
				lodMeshes[i].updateCallback += UpdateChunk;
				if (i == this.colliderLODIndex)
				{
					lodMeshes[i].updateCallback += UpdateCollisionMesh;
				}
			}
			
			//Request the map data for the chunk.
			
			#if !UNITY_WEBGL
			mapGenerator.RequestMapData(position,OnMapDataReceived);
#else
			mapGenerator.MapDataNoThread(position, OnMapDataReceived);
			#endif
			
			//_surface.BuildNavMesh();
		}

		//Called when the map data is received. See callback above.
		void OnMapDataReceived(MapData mapData) {
			
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = TextureGenerator.TextureFromColourMap (mapData.colourMap, mapGenerator.mapChunkSize, mapGenerator.mapChunkSize);
			meshRenderer.material.mainTexture = texture;
			//Update the chunk.
			UpdateChunk ();
		}

	
		//Updating the chunk based on the viewer position.
		public void UpdateChunk() {
			if (mapDataReceived) {
				//Debug.Log("Updating chunk");
				
				// Calculate the distance of the viewer from the bounds of the chunk.
				float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));
				bool visible = viewerDstFromNearestEdge <= maxViewDst;

				if (visible) 
				{
					
					int lodIndex = 0;

					
					//Loop through the LODs and determine which one should be used.
					for (int i = 0; i < detailLevels.Length - 1; i++) 
					{
						if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) 
						{
							lodIndex = i + 1;
						} 
						else 
						{
							break;
						}
					}
					
					//If the LOD has changed, update the mesh.
					if (lodIndex != previousLODIndex) 
					{
						LODMesh lodMesh = lodMeshes [lodIndex];
						if (lodMesh.hasMesh) 
						{
							previousLODIndex = lodIndex;
							meshFilter.mesh = lodMesh.mesh;

						} 
						else if (!lodMesh.hasRequestedMesh) 
						{
#if !UNITY_WEBGL	
							lodMesh.RequestMesh (mapData);
#else
							lodMesh.RequestMeshNoThread(mapData);
							
							#endif

						}
					}

					terrainChunksVisibleLastUpdate.Add (this);
				}

				SetVisible (visible);
			}
		}
		
		public void UpdateCollisionMesh()
		{
			if (hasSetCollider)
				return;
			
			
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

			
			if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
			{
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
				{
					
					#if !UNITY_WEBGL
					lodMeshes[colliderLODIndex].RequestMesh(mapData);
					#else
					lodMeshes[colliderLODIndex].RequestMeshNoThread(mapData);
					
#endif
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
			{
				if (lodMeshes[colliderLODIndex].hasMesh)
				{
					Debug.Log("Assigned mesh");
					_meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
					hasSetCollider = true;
					
					if (!generator.outpostSpawned)
					{
						generator.SpawnOutpost(generator.FindBestSuitableArea(50,10, new Vector2(-size / 2, size / 2), new Vector2(-size / 2, size / 2), 3));
					}
					
					if (!hasGeneratedMapObjects)
					{
						generator.GenerateObjects(15, new Vector2(-size / 2, size / 2), new Vector2(-size / 2, size / 2), coord * size, coord);
						hasGeneratedMapObjects = true;
					}
					
					UpdateNavMesh();
					
				}
				
			}
		}

		public void UpdateNavMesh()
		{
			
			NavMeshSurface surface = meshObject.GetComponent<NavMeshSurface>();
			surface.voxelSize = 2;
				if (!navMeshGenerated)
				{

					if (!meshObject.GetComponent<NavMeshSurface>())
					{
						meshObject.AddComponent<NavMeshSurface>();
					}
					
					surface.collectObjects = CollectObjects.Children;
					
					surface.BuildNavMesh();
					
					navMeshGenerated = true;
					
						
				}
		}
		

		public void SetVisible(bool visible) {
			if(meshObject != null)
				meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}

	class LODMesh {

		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;
		public event System.Action updateCallback;

		//Constructor for the LODMesh. Takes the LOD and the callback function.
		public LODMesh(int lod) {
			this.lod = lod;
		}
		
		//called when the mesh data is received. See callback above.
		void OnMeshDataReceived(MeshData meshData) {
			mesh = meshData.CreateMesh ();
			hasMesh = true;

			updateCallback ();
		}
		
		public void RequestMeshNoThread(MapData mapData) {
			hasRequestedMesh = true;
			mapGenerator.MeshDataNoThread (mapData, lod, OnMeshDataReceived);
		}
		#if !UNITY_WEBGL
		//Request the mesh data for the chunk.
		public void RequestMesh(MapData mapData) {
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData (mapData, lod, OnMeshDataReceived);
		}
		#endif

	}

	//Struct for the LODS. Holds the LOD and the visible distance threshold.
	[System.Serializable]
	public struct LODInfo {
		public int lod;
		public float visibleDstThreshold;


		public float sqrVisibleDstThreshold => visibleDstThreshold * visibleDstThreshold;
	}

}
