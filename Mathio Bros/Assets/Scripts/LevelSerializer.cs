// LevelSerializer.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LevelSerializer : MonoBehaviour
{
    public LevelEditorManger levelEditor;
    public TMP_InputField levelName;
    public GameObject mario;
    private LevelData returned;

    public void OnClickSave()
    {
        if (levelName.text == "") { return; }
        Debug.Log(levelName.text);

        StartCoroutine(FirebaseAPIManager.Instance.SaveLevel(levelName.text, SerializeLevel(levelName.text), (success, levelId) =>
        {
            {
                if (success)
                {
                    Debug.Log(levelId);
                }
                else
                {
                    Debug.LogError(levelId);
                }
            }
        }));
    }

    public void OnClickLoad()
    {
        if (levelName.text == "") { return ; }

        StartCoroutine(FirebaseAPIManager.Instance.GetLevelByName(levelName.text, (success, level) =>
        {
            if (success)
            {
                Debug.Log(level);
                DeserializeLevel(level);
            }
            else
            {
                Debug.LogError("error fetching level");
                return;
            }
        }));

    }

    public LevelData SerializeLevel(string levelName)
    {
        LevelData levelData = new LevelData();
        
        // Save initial quantities
        foreach (var button in levelEditor.ItemButtons)
        {
            levelData.initialQuantities[button.ID] = button.Quantity;
        }
        
        // Find all placed items in the scene
        var placedItems = GameObject.FindGameObjectsWithTag("PlacedItem"); // You'll need to tag your placed items
        Debug.Log(placedItems);
        foreach (var item in placedItems)
        {
            var deleteScript = item.GetComponent<DeleteItemFromLevel>();
            if (deleteScript != null)
            {
                LevelItemData itemData = new LevelItemData()
                {
                    itemID = deleteScript.ID,
                    position = item.transform.position,
                    rotation = item.transform.rotation
                };
                levelData.items.Add(itemData);
            }
        }

        string json = JsonUtility.ToJson(levelData);
        Debug.Log(json);
        return levelData;
    }
    
    public void DeserializeLevel(LevelData levelData)
    {
        GameObject.Find("Mario").transform.position = new Vector3(2.5f, 20f, 0f);
        Debug.Log("start DeserializeLevel");
        Debug.Log(levelData.items + "\n" + levelData.initialQuantities);
        string print = "";
        foreach (var item in levelData.items) { print += item.ToString() + ", "; }
        Debug.Log(print);
        print = "";
        foreach(var item in levelData.initialQuantities) { print += item.ToString() + ", "; }
        Debug.Log(print);
        // Reset all quantities
        foreach (var button in levelEditor.ItemButtons)
        {
            if (levelData.initialQuantities.ContainsKey(button.ID))
            {
                button.Quantity = levelData.initialQuantities[button.ID];
                button.QuantityText.text = button.Quantity.ToString();
            }
        }
        
        // Clear existing items
        var existingItems = GameObject.FindGameObjectsWithTag("PlacedItem");
        foreach (var item in existingItems)
        {
            Destroy(item);
        }
        
        // Recreate items from data
        foreach (var itemData in levelData.items)
        {
            if (itemData.itemID >= 0 && itemData.itemID < levelEditor.ItemPrefabs.Length)
            {
                var newItem = Instantiate(
                    levelEditor.ItemPrefabs[itemData.itemID],
                    itemData.position,
                    itemData.rotation
                );
                
                // Ensure the DeleteItemFromLevel script has the correct ID
                var deleteScript = newItem.GetComponent<DeleteItemFromLevel>();
                if (deleteScript != null)
                {
                    deleteScript.ID = itemData.itemID;
                }
            }
        }
    }
}