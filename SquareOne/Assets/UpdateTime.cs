using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTime : MonoBehaviour {

    public Text jikan;
    public Text tagName;
    public Button SubmitBtn;
    public GameObject Panel;
        


    public void CallSubmitTag()
    {
        StartCoroutine(SubmitTag());
    }

    public void CallSubmitRecord()
    {
        StartCoroutine(SubmitRecord());
    }

    IEnumerator SubmitRecord()
    {
        string jikanString = jikan.text;
        double toSeconds = TimeSpan.Parse(jikanString).TotalSeconds;
        string seconds = toSeconds.ToString();

        string endTime = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
        DateTime start = DateTime.Now.AddSeconds(-3600);
        string startTime = start.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

        WWWForm form = new WWWForm();
        //string jikanString = jikan.text;
        //double toSeconds = TimeSpan.Parse(jikanString).TotalSeconds;
        //string seconds = toSeconds.ToString();

        form.AddField("jikan", seconds);
        form.AddField("tagName", tagName.text);
        form.AddField("startTime", startTime);
        form.AddField("endTime", endTime);
        WWW www = new WWW("http://localhost:81/percentsdata/submitrecord.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("Record submitted successfully!!");
            Panel.SetActive(true);
        }
        else
        {
            Debug.Log("Something went horribly wrong when updating the record: Error number " + www.text);
        }
    }


    IEnumerator SubmitTag()
    {
        WWWForm form = new WWWForm();
        string jikanString = jikan.text;
        double toSeconds = TimeSpan.Parse(jikanString).TotalSeconds;
        string seconds = toSeconds.ToString();

        form.AddField("jikan", seconds);
        form.AddField("tagName", tagName.text);
        WWW www = new WWW("http://localhost:81/percentsdata/updatetime.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("Time updated successfully!!");
            Panel.SetActive(true);
        }
        else
        {
            Debug.Log("Something went horribly wrong in the time updating: Error number " + www.text);
        }      

    }
}
