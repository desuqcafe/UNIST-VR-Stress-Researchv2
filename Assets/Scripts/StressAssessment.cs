using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StressAssessment : MonoBehaviour
{
    public Button[] question1Buttons;
    public Button[] question2Buttons;
    public Button[] question3Buttons;

    public TMP_Text scoreTracker;

    private int currentQuestion = 1;
    private int score = 0;
    public int value = 0;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            int buttonIndex = i;
            question1Buttons[i].onClick.AddListener(() => Question1ButtonClicked(buttonIndex));
        }
        // add OnClick listener to buttons in question 2
        foreach (Button button in question2Buttons)
        {
            button.onClick.AddListener(() => Question2ButtonClicked(button));
        }

        // add OnClick listener to buttons in question 3
        foreach (Button button in question3Buttons)
        {
            button.onClick.AddListener(() => Question3ButtonClicked(button));
        }
    }

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
            value = int.Parse(question1Buttons[buttonIndex].GetComponentInChildren<Text>().text);
            score += value;

            // Disable the buttons in question 1
            for (int i = 0; i < question1Buttons.Length; i++)
            {
                if (i != buttonIndex)
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

    public void Question2ButtonClicked(Button button_2)
    {
        // retrieve int value from button
        int value = int.Parse(button_2.GetComponentInChildren<Text>().text);

        // add value to score
        score += value;

        // move on to next question
        currentQuestion = 3;

        // Disable the buttons in question 2
        foreach (Button button in question2Buttons)
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.gray;
        }

        // change the color of the button
        button_2.GetComponent<Image>().color = Color.green;

        // Enable the buttons in question 3
        foreach (Button button in question3Buttons)
        {
            button.interactable = true;
        }

        scoreTracker.text = "Score: " + score.ToString();
    }

    public void Question3ButtonClicked(Button button_3)
    {
        // retrieve int value from button
        int value = int.Parse(button_3.GetComponentInChildren<Text>().text);

        // add value to score
        score += value;

        // Disable the buttons in question 3
        foreach (Button button in question3Buttons)
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.gray;
        }

        // change the color of the button
        button_3.GetComponent<Image>().color = Color.green;

        // questionnaire is complete, do something with the score
        scoreTracker.text = "Score: " + score.ToString();
    }
}