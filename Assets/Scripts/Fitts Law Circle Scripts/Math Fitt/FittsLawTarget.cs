using UnityEngine;
using TMPro;

public class FittsLawTarget : MonoBehaviour
{
    public float value;
    public TMP_Text valueText;
    public Canvas valueCanvas;
    public bool correctSphere = false;
    private FittsLawCircleSubtraction fittsLawCircleSubtraction; // reference to FittsLawCircleSubtraction
    private bool isBeingClicked = false;



    void Start()
    {
        valueText = GetComponentInChildren<TMP_Text>();
        fittsLawCircleSubtraction = FittsLawCircleSubtraction.Instance;
    }

    public void SphereClicked()
    {
        if (isBeingClicked) 
        {
            return;
        }

        isBeingClicked = true;

        if (correctSphere)
        {
            fittsLawCircleSubtraction.score++;
            fittsLawCircleSubtraction.ContinueGame(this.gameObject);
        }
        else
        {
            Debug.Log("Incorrect Choice");
            fittsLawCircleSubtraction.ResetGame();  // Call ResetGame when the sphere is not the correct one
        }
        
        isBeingClicked = false;
    }
}
