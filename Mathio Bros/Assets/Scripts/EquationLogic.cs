using UnityEngine;
using TMPro;

public class EquationLogic : MonoBehaviour
{
    private FlagPole flagPole;

    public TMP_Text firstNumber;
    public TMP_Text symbol;
    public TMP_Text secondNumber;
    public TMP_Text result;
    public string equation;
    public string res;

    public bool isFirstHit {  get; private set; } = false;
    public bool isSecondHit { get; private set; } = false;
    public bool isIntegralHit { get; private set; } = false;

    public bool isComplete => isFirstHit && isSecondHit;


    private void Start()
    {
        flagPole = GameObject.Find("FlagPole").GetComponent<FlagPole>();
        if (flagPole.isEquationLevel)
        {
            string[] parts = equation.Split(' ');
            firstNumber.text = parts[0];
            symbol.text = parts[1];
            secondNumber.text = parts[2];
            result.text = parts[4];
        }
    }

    public void CheckHit(int number)
    {
        if (flagPole.isIntegralLevel && number.ToString() == res)
        {
            isIntegralHit = true;
            return;
        }
        if (!isFirstHit && firstNumber.text == number.ToString())
        {
            firstNumber.alpha = 1f;
            isFirstHit = true;
        }
        else if (!isSecondHit && secondNumber.text == number.ToString())
        {
            secondNumber.alpha = 1f;
            isSecondHit = true;
        }
    }
}
