using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkManager : MonoBehaviour
{
    public ChunkPreset preset;
    public int chunkSize = 20;
    public Transform player;

    private Dictionary<Vector2Int, ChunkData> chunkDictionary = new Dictionary<Vector2Int, ChunkData>();
    private List<Vector2Int> activeChunks = new List<Vector2Int>();
    private Dictionary<int, ObjectPool<GameObject>> prefabPools = new Dictionary<int, ObjectPool<GameObject>>();

    private Vector2Int playerChunkIndex;
    private Vector2Int[] chunkOffsets = new Vector2Int[] {
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

    void Start()
    {
        playerChunkIndex = new Vector2Int(0, 0);
        InitializePools();
        SpawnInitialChunks();
    }

    void Update()
    {
        Vector2Int newChunkIndex = new Vector2Int(Mathf.FloorToInt(player.position.x / chunkSize), Mathf.FloorToInt(player.position.z / chunkSize));
        if (newChunkIndex != playerChunkIndex)
        {
            playerChunkIndex = newChunkIndex;
            UpdateChunks();
        }
        Debug.Log(newChunkIndex);
    }

    void InitializePools()
    {
        for (int i = 0; i < preset.chunkSettings.Count; i++)
        {
            int index = i;
            prefabPools[index] = new ObjectPool<GameObject>(
                () =>
                {
                    GameObject obj = Instantiate(preset.chunkSettings[index].prefab);
                    ResizeChunk(obj); // resize when first created
                    return obj;
                },
                chunk => chunk.SetActive(true),
                chunk => chunk.SetActive(false),
                chunk => Destroy(chunk),
                false, 10, 50
            );
        }
    }

    void SpawnInitialChunks()
    {
        foreach (var offset in chunkOffsets)
        {
            Vector2Int chunkIndex = playerChunkIndex + offset;
            SpawnChunk(chunkIndex);
            activeChunks.Add(chunkIndex);
        }
    }

    void UpdateChunks()
    {
        List<Vector2Int> newActiveChunks = new List<Vector2Int>();
        foreach (var offset in chunkOffsets)
        {
            Vector2Int chunkIndex = playerChunkIndex + offset;
            newActiveChunks.Add(chunkIndex);
            if (!activeChunks.Contains(chunkIndex))
                SpawnChunk(chunkIndex);
        }

        foreach (var oldChunk in activeChunks)
        {
            if (!newActiveChunks.Contains(oldChunk))
                DespawnChunk(oldChunk);
        }

        activeChunks = newActiveChunks;
    }

    void SpawnChunk(Vector2Int chunkIndex)
    {
        if (chunkDictionary.ContainsKey(chunkIndex))
        {
            ChunkData data = chunkDictionary[chunkIndex];
            GameObject chunk = prefabPools[data.prefabIndex].Get();
            chunk.transform.position = new Vector3(chunkIndex.x * chunkSize, 0, chunkIndex.y * chunkSize);
            chunk.transform.rotation = Quaternion.Euler(0, data.rotationY, 0);
            data.chunkInstance = chunk;
            data.RestoreObjects();
        }
        else
        {
            int prefabIndex = GetWeightedRandomIndex();
            int rotationY = 90 * Random.Range(0, 4);
            GameObject chunk = prefabPools[prefabIndex].Get();
            chunk.transform.position = new Vector3(chunkIndex.x * chunkSize, 0, chunkIndex.y * chunkSize);
            chunk.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            ChunkData newData = new ChunkData(chunkIndex, chunk, prefabIndex, rotationY);
            chunkDictionary.Add(chunkIndex, newData);
        }
    }

    void DespawnChunk(Vector2Int chunkIndex)
    {
        if (chunkDictionary.ContainsKey(chunkIndex))
        {
            ChunkData data = chunkDictionary[chunkIndex];
            data.StoreUncollectedObjects();
            prefabPools[data.prefabIndex].Release(data.chunkInstance);
            data.chunkInstance = null;
        }
    }

    int GetWeightedRandomIndex()
    {
        int totalWeight = 0;
        foreach (var setting in preset.chunkSettings)
            totalWeight += setting.weight;

        int rand = Random.Range(0, totalWeight);
        int current = 0;
        for (int i = 0; i < preset.chunkSettings.Count; i++)
        {
            current += preset.chunkSettings[i].weight;
            if (rand < current)
                return i;
        }
        return 0;
    }
    void ResizeChunk(GameObject chunk)
    {
        Bounds combinedBounds = new Bounds();

        bool found = false;
        foreach (Transform child in chunk.transform)
        {
            if (child.tag == "Ground")
            {
                Renderer rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (!found)
                    {
                        combinedBounds = rend.bounds;
                        found = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(rend.bounds);
                    }
                }
            }
        }

        if (!found)
        {
            Debug.LogWarning("No ground tiles found in chunk to resize.");
            return;
        }

        float currentSizeX = combinedBounds.size.x;
        float currentSizeZ = combinedBounds.size.z;

        float scaleX = chunkSize / currentSizeX;
        float scaleZ = chunkSize / currentSizeZ;

        chunk.transform.localScale = new Vector3(scaleX, 1, scaleZ);
    }
}
