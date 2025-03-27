using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkIndex;
    public GameObject chunkInstance;
    public int prefabIndex;
    public int rotationY;
    public List<GameObject> uncollectedObjects = new List<GameObject>();

    public ChunkData(Vector2Int index, GameObject instance, int prefabIdx, int rotation)
    {
        chunkIndex = index;
        chunkInstance = instance;
        prefabIndex = prefabIdx;
        rotationY = rotation;
    }

    public void StoreUncollectedObjects()
    {
        //uncollectedObjects.Clear();
        //foreach (var obj in GameObject.FindGameObjectsWithTag("Collectable"))
        //{
        //    if (obj.activeInHierarchy)
        //        uncollectedObjects.Add(obj);
        //}
    }

    public void RestoreObjects()
    {
        //foreach (var obj in uncollectedObjects)
        //{
        //    if (obj != null)
        //        obj.SetActive(true);
        //}
    }
}

