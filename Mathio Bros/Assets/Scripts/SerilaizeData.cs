// LevelData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelItemData
{
    public int itemID;
    public Vector3 position;
    public Quaternion rotation;
}

[Serializable]
public class LevelData
{
    public string levelName;
    public List<LevelItemData> items = new List<LevelItemData>();
    public Dictionary<int, int> initialQuantities = new Dictionary<int, int>();
}