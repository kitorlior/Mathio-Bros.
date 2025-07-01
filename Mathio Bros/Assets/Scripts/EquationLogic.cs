using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class EquationLogic : MonoBehaviour
{
    private FlagPole flagPole;

    public TMP_Text firstNumber;
    public TMP_Text symbol;
    public TMP_Text secondNumber;
    public TMP_Text result;
    public string equation;
    public string res;

    public TMP_InputField inputField;

    public bool IsFirstHit { get; private set; } = false;
    public bool IsSecondHit { get; private set; } = false;
    public bool IsIntegralHit { get; private set; } = false;

    public bool IsComplete => IsFirstHit && IsSecondHit;


    private void Start()
    {
        flagPole = GameObject.Find("FlagPole").GetComponent<FlagPole>();
        if (RegexValidator.IsValidEquation(equation)) { SetEquation(); }
    }

    private Boolean SetEquation()
    {
        if (flagPole.isEquationLevel)
        {
            string[] parts = equation.Split(' ');
            firstNumber.text = parts[0];
            symbol.text = parts[1];
            secondNumber.text = parts[2];
            result.text = parts[4];
            return true;
        }

        return false;
    }

    public void CheckHit(int number)
    {
        if (flagPole.isIntegralLevel)
        {
            if (number.ToString() == res) { IsIntegralHit = true; }
            return;
        }
        if (!IsFirstHit && firstNumber.text == number.ToString())
        {
            firstNumber.alpha = 1f;
            IsFirstHit = true;
        }
        else if (!IsSecondHit && secondNumber.text == number.ToString())
        {
            secondNumber.alpha = 1f;
            IsSecondHit = true;
        }
    }

    public void OnClick()
    {
        Debug.Log("onclick");
        if (inputField != null) { equation = inputField.text; }
        if (RegexValidator.IsValidEquation(equation))
        {
            SetEquation(); 
            SetNumberBlocks();
        }
        
    }

    private bool SetNumberBlocks()
    {
        // Find the parent GameObject by name
        GameObject parentObject = GameObject.Find("Mystery");

        if (parentObject == null) { return false; }

        // Get all components of a specific type from children (including the parent)
        BlockHit[] components = parentObject.GetComponentsInChildren<BlockHit>();

        Vector3 firstPosition = Vector3.zero, secondPosition = Vector3.zero;

        foreach (BlockHit block in components)
        {
            if (!block.isNum) { continue; }
            block.isNum = false;
            block.blockNumber = -1;
        }

        while (firstPosition == Vector3.zero || secondPosition == Vector3.zero)
        {
            int randomIndex = UnityEngine.Random.Range(0, components.Length);
            if (components[randomIndex].item == null && components[randomIndex].transform.position != firstPosition && components[randomIndex].transform.position != secondPosition)
            {
                if (firstPosition == Vector3.zero)
                {
                    components[randomIndex].blockNumber = int.Parse(firstNumber.text);
                    components[randomIndex].isNum = true;
                    firstPosition = components[randomIndex].transform.position;
                    Debug.Log("first num is at " + firstPosition.ToString());
                }
                else if (firstPosition != Vector3.zero && components[randomIndex].transform.position != firstPosition)
                {
                    components[randomIndex].blockNumber = int.Parse(secondNumber.text);
                    components[randomIndex].isNum = true;
                    secondPosition = components[randomIndex].transform.position;
                    Debug.Log("second num is at " + secondPosition.ToString());
                }
            }
        }

        return (components != null);
    }

    public class RegexValidator
    {
        public static bool IsValidPattern(string input)
        {
            // Pattern explanation:
            // ^           - Start of string
            // [0-9]       - Single digit (0-9)
            // \\s         - Single whitespace
            // [+\\-*/]    - One of +, -, *, / (note: - needs to be escaped)
            // \\s         - Single whitespace
            // [0-9]       - Another single digit
            // \\s         - Single whitespace
            // =           - Equals sign
            // \\s         - Single whitespace
            // -?          - Optional negative sign
            // \\d+        - One or more digits (the answer)
            // $           - End of string
            string pattern = @"^[0-9]\s[+\-*/]\s[0-9]\s=\s-?\d+$";

            return Regex.IsMatch(input, pattern);
        }

        // Optional: Also validate that the equation is mathematically correct
        public static bool IsValidEquation(string input)
        {
            if (!IsValidPattern(input)) return false;

            try
            {
                // Remove spaces for evaluation
                string noSpaces = input.Replace(" ", "");
                var parts = noSpaces.Split('=');
                string equation = parts[0];
                int expectedAnswer = int.Parse(parts[1]);

                // Evaluate the equation
                int actualAnswer = (int)new System.Data.DataTable().Compute(equation, null);

                return actualAnswer == expectedAnswer;
            }
            catch
            {
                return false;
            }
        }
    }

}
