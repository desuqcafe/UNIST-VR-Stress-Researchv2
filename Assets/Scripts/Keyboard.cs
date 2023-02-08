using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text promptText;
    private string[] promptList = {"I AM NOT PLANNING ON DOING ANYTHING", "Plan ahead for the future", "The Zebra dances in a plane"};
    private int currentPromptIndex = 0;
    public GameObject normalButtons;
    public GameObject capsButtons;
    private bool caps;
    public bool promptFinished = false;

    public int promptCount = 0;
    TaskManager taskManager;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    void Start()
    {
        inputField.onValueChanged.AddListener(CompareInput);
        caps = true;
        currentPromptIndex = 0;
        promptText.text = promptList[currentPromptIndex];
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
            promptFinished = true;
        }
    }
    public void enableRays()
    {
        leftHandRay.enabled = true;
        rightHandRay.enabled = true;
    }

    public void GenerateNewPrompt()
    {
        if (promptCount == 1) {
            TaskManager.Instance.KeyboardDisable();
            TaskManager.Instance.FittEnable();
            enableRays();
        } else if (promptFinished) {
            currentPromptIndex++;
            if (currentPromptIndex >= promptList.Length)
            {
                currentPromptIndex = 0;
            }
            promptText.text = promptList[currentPromptIndex];
            inputField.text = "";
            promptFinished = false;
            promptCount++;
        }
    }
}
