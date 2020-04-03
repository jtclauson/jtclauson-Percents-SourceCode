using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphButtons : MonoBehaviour {



    private void OnMouseEnter()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color = new Color(3, 0, 0);
        Debug.Log("Entered");
    }

    private void OnMouseExit()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 171, 255, 255);
        Debug.Log("Exited");
    }




}
