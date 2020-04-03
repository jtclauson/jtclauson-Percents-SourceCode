using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggles : MonoBehaviour
{

    public GameObject monthToggle;
    

    public GameObject monthDropdown;
    public GameObject dayDropdown;

    bool yearOn = true;
    bool monthOn = true;
    

    public void yearOnly()
    {
        if (yearOn)
        {
            monthToggle.SetActive(false);
            monthDropdown.SetActive(false);

            
            dayDropdown.SetActive(false);
        }
        else
        {
            monthToggle.SetActive(true);
            monthDropdown.SetActive(true);

            
            dayDropdown.SetActive(true);
        }

        yearOn = !yearOn;

    }

    public void monthOnly()
    {
        if (monthOn)
        {
           
            dayDropdown.SetActive(false);
        }
        else
        {
            
            dayDropdown.SetActive(true);
        }

        monthOn = !monthOn;
    }

   

}

   