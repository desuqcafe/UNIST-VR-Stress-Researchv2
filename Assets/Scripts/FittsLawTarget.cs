using UnityEngine;
using TMPro;

public class FittsLawTarget : MonoBehaviour
{
     public float value;
     public TMP_Text valueText;
     public Canvas valueCanvas;
     public bool correctSphere = false;

     void Start()
     {
          valueText = GetComponentInChildren<TMP_Text>();
          valueCanvas = GetComponentInChildren<Canvas>();
     }

}