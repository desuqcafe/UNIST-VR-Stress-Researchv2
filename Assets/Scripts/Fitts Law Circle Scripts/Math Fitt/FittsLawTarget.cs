using UnityEngine;
using TMPro;

public class FittsLawTarget : MonoBehaviour
{
    public float value;
    public TMP_Text valueText;
    public Canvas valueCanvas;
    public bool correctSphere = false;
    private FittsLawCircleSubtraction fittsLawCircleSubtraction; // reference to FittsLawCircleSubtraction

    void Start()
    {
        valueText = GetComponentInChildren<TMP_Text>();
        fittsLawCircleSubtraction = FittsLawCircleSubtraction.Instance;
    }

    public void SphereClicked()
    {
        if (correctSphere)
        {
            fittsLawCircleSubtraction.score++;
            fittsLawCircleSubtraction.ContinueGame(this.gameObject);
        }
        else
        {
            Debug.Log("Incorrect Choice");
        }
    }
}
