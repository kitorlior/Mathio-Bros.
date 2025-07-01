using UnityEngine;

public class LevelEditorManger : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public GameObject[] ItemImage;
    public int CurrentButtonPressed;


    private void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked) // Left mouse button
        {
            ItemButtons[CurrentButtonPressed].Clicked=false;
            GameObject obj = Instantiate(ItemPrefabs[CurrentButtonPressed], new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
            obj.tag = "PlacedItem";
            Destroy(GameObject.FindGameObjectWithTag("ItemImage")); // Destroy the item image if it exists
            Debug.Log("Item ID: " + CurrentButtonPressed + " placed at position: " + worldPosition);
        }
    }
}
