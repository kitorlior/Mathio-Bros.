// LevelSerializer.cs
using UnityEngine;
using System.Collections.Generic;

public class LevelSerializer : MonoBehaviour
{
    public LevelEditorManger levelEditor;
    
    public string SerializeLevel(string levelName)
    {
        LevelData levelData = new LevelData();
        levelData.levelName = levelName;
        
        // Save initial quantities
        foreach (var button in levelEditor.ItemButtons)
        {
            levelData.initialQuantities[button.ID] = button.Quantity;
        }
        
        // Find all placed items in the scene
        var placedItems = GameObject.FindGameObjectsWithTag("PlacedItem"); // You'll need to tag your placed items
        
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
        
        return JsonUtility.ToJson(levelData, true);
    }
    
    public void DeserializeLevel(string json)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        
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