using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text promptText;
    public GameObject normalButtons;
    public GameObject capsButtons;
    private bool caps;

    void Start()
    {
        caps = false;
        inputField.onValueChanged.AddListener(CompareInput);
    }

    public void InsertChar(string c)
    {
        inputField.text += c;
    }

    public void DelChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void InsertSpace()
    {
        inputField.text += " ";
    }

    public void CapsPressed()
    {
        if (!caps)
        {
            normalButtons.SetActive(false);
            capsButtons.SetActive(true);
            caps = true;
        } else {
            capsButtons.SetActive(false);
            normalButtons.SetActive(true);
            caps = false;
        }
    }

    public void CompareInput(string input)
    {
        string prompt = promptText.text;

        if(input.Length > prompt.Length)
        {
            input = input.Substring(0, prompt.Length);
        }

        for(int i = 0; i < input.Length; i++)
        {
            if(input[i] != prompt[i])
            {
                // If the input does not match the prompt, change the text color to red
                inputField.textComponent.color = Color.red;
                return;
            } else {
                inputField.textComponent.color = Color.black;
            }
        }

        if(input.Length == prompt.Length)
        {
            // If the input matches the prompt exactly, change the text color back to black
            inputField.textComponent.color = Color.green;
        }
    }
}
