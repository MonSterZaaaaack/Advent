using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chunk Preset", menuName = "ChunkSystem/New Chunk Preset")]
public class ChunkPreset : ScriptableObject
{
    public List<ChunkSetting> chunkSettings;
}
[Serializable]
public class ChunkSetting
{
    public GameObject prefab;
    public int weight;
    public ChunkSetting()
    {
        prefab = null;
        weight = 0;
    }
}
