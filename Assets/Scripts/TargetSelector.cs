using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public GameObject highlightedTarget;
    public float radius = 0.65f;
    public float scaleValue = 0.5f;
    private int selectionsCount = 0;

    public void CircleTouched(GameObject touchedObject)
    {
        if (touchedObject == highlightedTarget)
        {
            selectionsCount++;
            if (selectionsCount % 5 == 0)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    transform.GetChild(j).transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
                }
            }
            if (selectionsCount % 3 == 0)
            {
                radius += 0.15f;
            }
            highlightedTarget.GetComponent<FittsLawCircle>().isHighlighted = false;
            highlightedTarget.GetComponent<FittsLawCircle>().renderer.material = highlightedTarget.GetComponent<FittsLawCircle>().originalMaterial;
            highlightedTarget.GetComponent<FittsLawCircle>().runOnce = false;
            int nextIndex = Random.Range(0, transform.childCount);
            highlightedTarget = transform.GetChild(nextIndex).gameObject;
            highlightedTarget.GetComponent<FittsLawCircle>().isHighlighted = true;
        }
    }
}
