using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StressAssessment : MonoBehaviour
{
    public Button[] question1Buttons;
    public Button[] question2Buttons;
    public Button[] question3Buttons;

    public TMP_Text scoreTracker;
    private int score = 0;
    public int value = 0;

    private void Start()
    {
        // Disable the buttons in question 2 and 3
        foreach (Button button in question2Buttons)
        {
            button.interactable = false;
        }

        foreach (Button button in question3Buttons)
        {
            button.interactable = false;
        }
    }

public void Question1ButtonClicked(int buttonIndex)
{
    if (question1Buttons != null && buttonIndex >= 0 && buttonIndex < question1Buttons.Length && question1Buttons[buttonIndex] != null)
    {
        // value = int.Parse(question1Buttons[buttonIndex].GetComponentInChildren<Text>().text);
        // score += value;

        // Disable the buttons in question 1 except the button clicked
        for (int i = 0; i < question1Buttons.Length; i++)
        {
            if (question1Buttons[i] != question1Buttons[buttonIndex])
            {
                question1Buttons[i].interactable = false;
                question1Buttons[i].GetComponent<Image>().color = Color.gray;
            }
        }

        // change the color of the button
        question1Buttons[buttonIndex].GetComponent<Image>().color = Color.green;


        // Enable the buttons in question 2
        foreach (Button button in question2Buttons)
        {
            button.interactable = true;
        }

        scoreTracker.text = "Score: " + score.ToString();
    }
}

    public void Question2ButtonClicked(int buttonIndex)
    {
    if (question2Buttons != null && buttonIndex >= 0 && buttonIndex < question2Buttons.Length && question2Buttons[buttonIndex] != null)
    {
        // value = int.Parse(question2Buttons[buttonIndex].GetComponentInChildren<Text>().text);
        // score += value;

        // Disable the buttons in question 1 except the button clicked
        for (int i = 0; i < question2Buttons.Length; i++)
        {
            if (question2Buttons[i] != question2Buttons[buttonIndex])
            {
                question2Buttons[i].interactable = false;
                question2Buttons[i].GetComponent<Image>().color = Color.gray;
            }
        }

        // change the color of the button
        question2Buttons[buttonIndex].GetComponent<Image>().color = Color.green;


        // Enable the buttons in question 3
        foreach (Button button in question3Buttons)
        {
            button.interactable = true;
        }

        scoreTracker.text = "Score: " + score.ToString();
    }
    }

    public void Question3ButtonClicked(int buttonIndex)
    {
    if (question3Buttons != null && buttonIndex >= 0 && buttonIndex < question3Buttons.Length && question3Buttons[buttonIndex] != null)
    {
        // value = int.Parse(question3Buttons[buttonIndex].GetComponentInChildren<Text>().text);
        // score += value;

        // Disable the buttons in question 1 except the button clicked
        for (int i = 0; i < question3Buttons.Length; i++)
        {
            if (question3Buttons[i] != question3Buttons[buttonIndex])
            {
                question3Buttons[i].interactable = false;
                question3Buttons[i].GetComponent<Image>().color = Color.gray;
            }
        }

        // change the color of the button
        question3Buttons[buttonIndex].GetComponent<Image>().color = Color.green;

        scoreTracker.text = "Score: " + score.ToString();
    }
    }
}