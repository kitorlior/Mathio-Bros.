using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagPole : MonoBehaviour
{
    public Transform flag;
    public Transform poleBottom;
    public Transform castle;
    public float speed = 6f;
    public string nextLevelName;
    public bool isEquationLevel;
    public bool isIntegralLevel = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EquationLogic equationLogic = GameObject.Find("Equation Visualizer").GetComponent<EquationLogic>();
        bool complete = false;
        bool equationComplete = isEquationLevel && equationLogic.isComplete;
        bool integralComplete = isIntegralLevel && equationLogic.isIntegralHit;
        complete = equationComplete || integralComplete || !(isEquationLevel && isIntegralLevel);
        Debug.Log("isComplete: "+ complete);
        if (!collision.CompareTag("Player"))
            return;
        if (complete)
        {
            StartCoroutine(MoveTo(flag, poleBottom.position)); // move flag to bottom of flagpole
            StartCoroutine(LevelCompleteSequence(collision.transform)); // move mario to castle in a few stages
        }
        else
        {
            GameManager.Instance.ResetLevel();
        }
        
    }

    private IEnumerator LevelCompleteSequence(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;

        yield return MoveTo(player, poleBottom.position); // move mario to bottom
        yield return MoveTo(player, player.position + Vector3.right); // move one block right
        yield return MoveTo(player, player.position + Vector3.right + Vector3.down); // move one right and one down
        yield return MoveTo(player, castle.position); // move to castle entrence

        player.gameObject.SetActive(false); // "mario enters castle"

        SceneManager.LoadScene(nextLevelName);
        GameManager.Instance.levelName = nextLevelName;
        Debug.Log("next level!");
    }

    private IEnumerator MoveTo(Transform subject, Vector3 dest) // animate change in position
    {
        while (Vector3.Distance(subject.position, dest) > 0.125f)
        {
            subject.position = Vector3.MoveTowards(subject.position, dest, speed * Time.deltaTime);
            yield return null;
        }
        subject.position = dest;
    }
}
