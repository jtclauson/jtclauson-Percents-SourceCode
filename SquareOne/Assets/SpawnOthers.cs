using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOthers : MonoBehaviour {

    public GameObject newKirb = null;
    public GameObject tagKirb = null;
    public GameObject recordKirb = null;

    private void OnMouseDown()
    {
        newKirb.SetActive(true);
        tagKirb.SetActive(true);
        recordKirb.SetActive(true);
    }

    private void OnMouseUp()
    {
        newKirb.SetActive(false);
        tagKirb.SetActive(false);
        recordKirb.SetActive(false);
    }

    

}
