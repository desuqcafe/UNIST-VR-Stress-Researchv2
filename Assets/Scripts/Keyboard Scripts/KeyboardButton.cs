using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class KeyboardButton : MonoBehaviour
{
    Keyboard keyboard;
    TextMeshProUGUI buttonText;

void Start()
{
    keyboard = GetComponentInParent<Keyboard>();
    buttonText = GetComponentInChildren<TextMeshProUGUI>();
    if (buttonText.text.Length == 1)
    {
        NameToButtonText();
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
    else if (gameObject.name == "Delete")
    {
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
    else if (gameObject.name == "Enter")
    {
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
    else if (gameObject.name == "Space")
    {
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnSelectEntered);
    }
}

public void OnSelectEntered(SelectEnterEventArgs args)
{
    if (buttonText.text.Length == 1)
    {
        keyboard.InsertChar(buttonText.text);
    }
    else if (gameObject.name == "Delete")
    {
        keyboard.InsertDelete();
    }
    else if (gameObject.name == "Enter")
    {
        keyboard.InsertEnter();
    }
    else if (gameObject.name == "Space")
    {
        keyboard.InsertSpace();
    }
}

    public void keyRayInsert()
    {
        keyboard.InsertChar(buttonText.text);
    }

    public void NameToButtonText()
    {
        buttonText.text = gameObject.name;
    }


}
