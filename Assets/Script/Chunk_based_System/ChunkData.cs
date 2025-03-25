using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2 chunkIndex;  // The index of the chunk in the grid.
    public List<GameObject> uncollectedObjects = new List<GameObject>();  // Store uncollected objects in this chunk.

    // Constructor to initialize ChunkData with a given index.
    public ChunkData(Vector2 index)
    {
        chunkIndex = index;
    }

    // Method to add uncollected objects before chunk is destroyed.
    public void StoreUncollectedObject(GameObject obj)
    {
        uncollectedObjects.Add(obj);
    }

    // Method to respawn uncollected objects when the chunk is regenerated.
    public void RestoreObjects()
    {
        foreach (var obj in uncollectedObjects)
        {
            obj.SetActive(true);
            // Additional logic to reset the object position can go here.
        }
    }
}
