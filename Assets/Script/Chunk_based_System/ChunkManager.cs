using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject chunkPrefab;  // Prefab of a chunk to instantiate.
    public int chunkSize = 10;      // Size of each chunk (assuming square).
    public Transform player;        // Reference to the player's transform.

    private Dictionary<Vector2Int, ChunkData> chunkDictionary = new Dictionary<Vector2Int, ChunkData>();  // Store all generated chunks.
    private List<Vector2Int> activeChunks = new List<Vector2Int>();  // Track currently active chunks.

    private Vector2Int playerChunkIndex;  // The current chunk the player is in.
    private Vector2Int[] chunkOffsets = new Vector2Int[] {
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };  // Positions of 9 active chunks relative to the player.

    void Start()
    {
        // Initialize player position in chunk (0,0) and spawn surrounding chunks.
        playerChunkIndex = new Vector2Int(0, 0);
        SpawnInitialChunks();
    }

    void Update()
    {
        // Update player's current chunk index based on its position.
        Vector2Int newChunkIndex = new Vector2Int(Mathf.FloorToInt(player.position.x / chunkSize), Mathf.FloorToInt(player.position.z / chunkSize));

        if (newChunkIndex != playerChunkIndex)
        {
            playerChunkIndex = newChunkIndex;
            UpdateChunks();
        }
    }

    // Spawn the initial 9 chunks (player starts at chunk 0,0).
    void SpawnInitialChunks()
    {
        foreach (var offset in chunkOffsets)
        {
            Vector2Int chunkIndex = playerChunkIndex + offset;
            SpawnChunk(chunkIndex);
            activeChunks.Add(chunkIndex);
        }
    }

    // Update active chunks when player moves.
    void UpdateChunks()
    {
        // Find new chunk positions relative to the player's current chunk.
        List<Vector2Int> newActiveChunks = new List<Vector2Int>();

        foreach (var offset in chunkOffsets)
        {
            Vector2Int chunkIndex = playerChunkIndex + offset;
            newActiveChunks.Add(chunkIndex);

            // Spawn new chunks that are not currently active.
            if (!activeChunks.Contains(chunkIndex))
            {
                SpawnChunk(chunkIndex);
            }
        }

        // Despawn chunks that are no longer active.
        foreach (var oldChunk in activeChunks)
        {
            if (!newActiveChunks.Contains(oldChunk))
            {
                DespawnChunk(oldChunk);
            }
        }

        activeChunks = newActiveChunks;  // Update the active chunks list.
    }

    // Method to spawn a chunk based on its index.
    void SpawnChunk(Vector2Int chunkIndex)
    {
        if (chunkDictionary.ContainsKey(chunkIndex))
        {
            // If chunk has been seen before, restore it from chunk data.
            ChunkData chunkData = chunkDictionary[chunkIndex];
            chunkData.RestoreObjects();
        }
        else
        {
            // If chunk is new, generate it randomly and store its data.
            GameObject newChunk = Instantiate(chunkPrefab, new Vector3(chunkIndex.x * chunkSize, 0, chunkIndex.y * chunkSize), Quaternion.identity);
            ChunkData newChunkData = new ChunkData(chunkIndex);
            chunkDictionary.Add(chunkIndex, newChunkData);
        }
    }

    // Method to despawn a chunk and store its uncollected objects.
    void DespawnChunk(Vector2Int chunkIndex)
    {
        if (chunkDictionary.ContainsKey(chunkIndex))
        {
            ChunkData chunkData = chunkDictionary[chunkIndex];

            // Find objects in the chunk that haven't been collected and store them.
            //GameObject[] objectsInChunk = FindObjectsInChunk(chunkIndex);
            //foreach (var obj in objectsInChunk)
            //{
            //    if (!obj.GetComponent<CollectableObject>().isCollected)
            //    {
            //        chunkData.StoreUncollectedObject(obj);
            //        obj.SetActive(false);  // Deactivate the object.
            //    }
            //}
        }
    }
}
