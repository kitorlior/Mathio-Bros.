using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum Type
    {
        Coin,
        ExtraLife,
        MagicMushroom,
        Starpower,
    }

    public Type type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision.gameObject); // hangle collecting powerups
        }
    }

    private void Collect(GameObject player)
    {
        switch (type)
        {
            case Type.Coin:
                GameManager.Instance.AddCoin(); // adds coin
                break;
             
            case Type.ExtraLife:
                GameManager.Instance.AddLife(); // adds life
                break;
            
            case Type.MagicMushroom:
                player.GetComponent<Player>().Grow(); // change to big mario
                break;
            
            case Type.Starpower:
                player.GetComponent<Player>().Starpower(); // change to startpower mode
                break;
        }

        Destroy(gameObject);
    }
}
