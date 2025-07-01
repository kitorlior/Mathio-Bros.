using UnityEngine;

public class DeleteItemFromLevel : MonoBehaviour
{
    public int ID;
    private LevelEditorManger levelEditorManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelEditorManager = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManger>();
        
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)) // Right mouse button
        {
            Debug.Log("button clicked");
            // Find the item with the specified ID in the level editor manager
            GameObject itemToDelete = gameObject;
            if (itemToDelete != null)
            {
                Destroy(itemToDelete); // Destroy the item
                levelEditorManager.ItemButtons[ID].Quantity++; // Increment the quantity of the item
                levelEditorManager.ItemButtons[ID].QuantityText.text = levelEditorManager.ItemButtons[ID].Quantity.ToString(); // Update the quantity text
                Debug.Log("Item ID: " + ID + " deleted from level.");
            }
            else
            {
                Debug.Log("No item found with ID: " + ID);
            }
        }
    }



    
}
