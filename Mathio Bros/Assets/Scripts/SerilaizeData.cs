// LevelData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelItemData
{
    public int itemID;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class LevelData
{
    public List<LevelItemData> items = new List<LevelItemData>();
    public Dictionary<int, int> initialQuantities = new Dictionary<int, int>();
}