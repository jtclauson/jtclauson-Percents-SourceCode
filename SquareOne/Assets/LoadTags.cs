using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTags : MonoBehaviour {


    public Text textField;

   

    public void CallShowTags()
    {
        StartCoroutine(ShowTags());
    }

    IEnumerator ShowTags()
    {
       WWW tagsData = new WWW("http://localhost:81/percentsdata/ShowTags.php");
        yield return tagsData;
        //string tagsDataString = tagsData.text;
        string[] dataArray = tagsData.text.Split(';');
        //textField.text=tagsData.text;
        textField.text=dataArray.Length.ToString();
        textField.text+=dataArray[1];
        
    }



}
