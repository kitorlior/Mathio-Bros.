using UnityEngine;
using TMPro;

public class ItemController : MonoBehaviour
{
    public int ID;
    public int Quantity;
    public TextMeshProUGUI QuantityText;
    public bool Clicked=false;
    private LevelEditorManger levelEditorManager;

    void Start()
    {
        QuantityText.text = Quantity.ToString();
        levelEditorManager = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManger>();
    }
    public void ButtonClicked()
    {
        if(Quantity>0)
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Clicked = true;
            GameObject obj = Instantiate(levelEditorManager.ItemImage[ID], new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
            Quantity--;
            QuantityText.text = Quantity.ToString();
            levelEditorManager.CurrentButtonPressed = ID; // Update the current button pressed in the level editor manager
            Debug.Log("Item ID: " + ID + " clicked. Remaining quantity: " + Quantity);
        }
        else
        {
            Debug.Log("Item ID: " + ID + " is out of stock.");
        }
    }

   
}
