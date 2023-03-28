using UnityEngine;
using UnityEngine.UI;

public class SliderChangedValue : MonoBehaviour
{
    public int questionNumber; // The number of the question (1-5)

    private Slider slider; // Reference to the slider component

    private void Start()
    {
        slider = GetComponent<Slider>(); // Get the slider component
        slider.onValueChanged.AddListener(SaveSliderValue); // Add an event listener to the OnValueChanged event
    }

    private void SaveSliderValue(float value)
    {
        // Save the slider value to PlayerPrefs using the question number as the key
        //PlayerPrefs.SetInt("Question" + questionNumber.ToString(), (int)value);
        //PlayerPrefs.Save(); // Save the PlayerPrefs data to disk

        // Log the slider value and question number to the console
        Debug.Log("Question " + questionNumber.ToString() + " value: " + value.ToString());

    }

// int question1Value = PlayerPrefs.GetInt("Question1");
// int question2Value = PlayerPrefs.GetInt("Question2");
// int question3Value = PlayerPrefs.GetInt("Question3");
// int question4Value = PlayerPrefs.GetInt("Question4");
// int question5Value = PlayerPrefs.GetInt("Question5");
}
