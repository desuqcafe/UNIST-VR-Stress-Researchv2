using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class Keyboard : MonoBehaviour
{

    public PhraseChecker phraseChecker;


    public TMP_InputField inputField;
    public TMP_Text promptText;
    private int currentPromptIndex = 0;
    public GameObject normalButtons;
    public GameObject capsButtons;
    private bool caps;
    public bool promptFinished = false;

    public int promptCount = 0;
    TaskManager taskManager;
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    private List<string> phrases;

    public TMP_Text customCaret;
    private float paddingLeft;
    private float paddingTop;
    private TextAlignmentOptions textAlignment;





    void Start()
    {
        inputField.onValueChanged.AddListener(CompareInput);
        caps = true;
        currentPromptIndex = 0;
        // Access the phrases list from the PhraseChecker script
        phrases = phraseChecker.phrases;

        promptText.text = phrases[currentPromptIndex];

        // Make the caret always visible
        inputField.caretBlinkRate = 0;
        inputField.caretWidth = 2;

        paddingLeft = inputField.textComponent.margin.x;
        paddingTop = inputField.textComponent.margin.y;
        textAlignment = inputField.textComponent.alignment;

        StartCoroutine(BlinkCaret());

    }

    public void InsertChar(string c)
    {
        inputField.text += c;
            UpdateCaretPosition();

    }

    void UpdateCaretPosition()
    {
        Vector2 preferredValues = inputField.textComponent.GetPreferredValues(inputField.text);
        float textWidth = preferredValues.x;
        float textHeight = preferredValues.y;

        float xOffset = paddingLeft;
        if (textAlignment == TextAlignmentOptions.Center)
        {
            xOffset += (inputField.textComponent.rectTransform.rect.width - textWidth) / 2;
        }
        else if (textAlignment == TextAlignmentOptions.Right)
        {
            xOffset += inputField.textComponent.rectTransform.rect.width - textWidth;
        }

        customCaret.transform.localPosition = new Vector3(xOffset + textWidth, paddingTop, 0);
    }

    IEnumerator BlinkCaret()
    {
        while (true)
        {
            customCaret.enabled = !customCaret.enabled;
            yield return new WaitForSeconds(0.5f); // Adjust the wait time for desired blink rate
        }
    }


    public void DelChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
        UpdateCaretPosition();

    }

    public void InsertEnter()
    {
        //inputField.text += "\n";
        phraseChecker.CheckInput();
            UpdateCaretPosition();


    }

    public void InsertDelete()
    {
        DelChar();
            UpdateCaretPosition();

    }


    public void InsertSpace()
    {
        inputField.text += " ";
            UpdateCaretPosition();

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

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != prompt[i])
            {
                // If the input does not match the prompt, change the text color to red
                //inputField.textComponent.color = Color.red;
                return;
            }
            else
            {
                inputField.textComponent.color = Color.black;
            }
        }

        if (input.Length == prompt.Length)
        {
            // If the input matches the prompt exactly, change the text color back to black
            inputField.textComponent.color = Color.green;
            promptFinished = true;
        }

        // Update the custom caret position
        Vector2 caretPosition = inputField.textComponent.GetPreferredValues(input);
        customCaret.rectTransform.anchoredPosition = new Vector2(caretPosition.x, 0);
    }
    public void enableRays()
    {
        leftHandRay.enabled = true;
        rightHandRay.enabled = true;
    }

    // public void GenerateNewPrompt()
    // {
    //     if (promptCount == 1) {
    //         TaskManager.Instance.KeyboardDisable();
    //         TaskManager.Instance.FittEnable();
    //         enableRays();
    //     } else if (promptFinished) {
    //         currentPromptIndex++;
    //         if (currentPromptIndex >= promptList.Length)
    //         {
    //             currentPromptIndex = 0;
    //         }
    //         promptText.text = promptList[currentPromptIndex];
    //         inputField.text = "";
    //         promptFinished = false;
    //         promptCount++;
    //     }
    // }

    public void GenerateNewPrompt()
    {
        if (promptCount == 1) {
             TaskManager.Instance.KeyboardDisable();
             TaskManager.Instance.FittEnable();
             enableRays();
        }
        else if (phraseChecker.inputField.textComponent.color == Color.green) // Replaced promptFinished with color check
        {
            currentPromptIndex++;
            if (currentPromptIndex >= phrases.Count)
            {
                currentPromptIndex = 0;
            }
            promptText.text = phrases[currentPromptIndex];
            inputField.text = "";
            // promptFinished = false; // Commented out
            promptCount++;
        }
    }
}
