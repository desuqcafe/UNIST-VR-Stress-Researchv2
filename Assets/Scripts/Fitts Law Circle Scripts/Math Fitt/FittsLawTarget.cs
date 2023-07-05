using UnityEngine;
using TMPro;
using System.Collections;

public class FittsLawTarget : MonoBehaviour
{
    public float value;
    public TMP_Text valueText;
    public Canvas valueCanvas;
    public bool correctSphere = false;
    private FittsLawCircleSubtraction fittsLawCircleSubtraction; // reference to FittsLawCircleSubtraction
    private bool isBeingClicked = false;
    public Material originalMaterial;
    public Material correctAnswerMaterial;
    public Material wrongAnswerMaterial;



    void Start()
    {
        valueText = GetComponentInChildren<TMP_Text>();
        fittsLawCircleSubtraction = FittsLawCircleSubtraction.Instance;
    }

    public void onSphereSelected()
    {
        isBeingClicked = true;
        SphereClicked();
    }

    void onSphereReleased()
    {
        isBeingClicked = false;
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
            StartCoroutine(ShowFeedbackAndContinue(correctAnswerMaterial, this.gameObject));
        }
        else
        {
            Debug.Log("Incorrect Choice");
            StartCoroutine(ShowFeedbackAndContinue(wrongAnswerMaterial, null));
        }

        isBeingClicked = false;
    }
    IEnumerator ShowFeedbackAndContinue(Material feedbackMaterial, GameObject correctSphere)
    {
        ChangeAllSpheresMaterial(feedbackMaterial);

        yield return new WaitForSeconds(0.5f);

        ChangeAllSpheresMaterial(originalMaterial);

        if (correctSphere != null)
        {
            fittsLawCircleSubtraction.ContinueGame(correctSphere);
        }
        else
        {
            fittsLawCircleSubtraction.RegisterError(gameObject);  // Call RegisterError when the selected sphere is incorrect
        }
    }

    void ChangeAllSpheresMaterial(Material material)
    {
        foreach (GameObject sphere in fittsLawCircleSubtraction.spheresList)
        {
            sphere.GetComponent<Renderer>().material = material;
        }
    }
}
