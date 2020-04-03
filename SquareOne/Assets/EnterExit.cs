using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterExit : MonoBehaviour {

    public int toScene;

    //void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        SceneManager.LoadScene(toScene);
    //    }
    //}

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){
            SceneManager.LoadScene(toScene);
        }
    }

    private void OnMouseEnter()
    {

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color= new Color(3,0,0);
        Debug.Log("Entered");
    }

    private void OnMouseExit()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color = new Color(255, 255, 255);
        Debug.Log("Exited");
    }


}
