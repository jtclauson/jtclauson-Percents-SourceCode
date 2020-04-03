﻿﻿using UnityEngine;
﻿using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tutorial_ScrollView : MonoBehaviour
{

    public GameObject Button_Template;
    public Text timedText;   

    public void CallStart()
    {
        StartCoroutine(Start());
    }

    IEnumerator Start()
    {
        WWW tagsData = new WWW("http://localhost:81/percentsdata/ShowTags.php");
        yield return tagsData;
        string[] dataArray = tagsData.text.Split(';');

        int numrows = int.Parse(dataArray[0]);

        bool skipFirst=false;

        foreach (string str in dataArray)
        {
            if (skipFirst)
            {
                GameObject go = Instantiate(Button_Template) as GameObject;
                go.SetActive(true);
                Tutorial_Button TB = go.GetComponent<Tutorial_Button>();
                TB.SetName(str);
                go.transform.SetParent(Button_Template.transform.parent);
            }
            skipFirst=true;

        }



    }

    public void ButtonClicked(string str)
    {   
        Debug.Log(str + " button clicked.");
        timedText.text=str;

    }
}