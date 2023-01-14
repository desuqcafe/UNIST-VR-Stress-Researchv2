using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TypingArea : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject leftTypingHand;
    public GameObject rightHand;
    public GameObject rightTypingHand;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Hand entered");

        GameObject hand = other.GetComponentInParent<HandScript>().gameObject;
        if (hand == null) return;
        if (hand == leftHand)
        {
            leftTypingHand.SetActive(true);
        } else if (hand == rightHand)
        {
            rightTypingHand.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
                Debug.Log("Hand left");

        GameObject hand = other.GetComponentInParent<HandScript>().gameObject;
        if (hand == null) return;
        if (hand == leftHand)
        {
            leftTypingHand.SetActive(false);
        } else if (hand == rightHand)
        {
            rightTypingHand.SetActive(false);
        }       
    }
}
