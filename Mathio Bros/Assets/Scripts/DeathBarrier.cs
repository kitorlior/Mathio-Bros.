using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            GameManager.Instance.ResetLevel();
        }
        else { Destroy(collision.gameObject); }
    }
}
