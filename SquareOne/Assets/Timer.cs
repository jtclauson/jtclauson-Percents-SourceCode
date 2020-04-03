using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class Timer : MonoBehaviour {


    public Text jikan;
    public Text tagName;
    public Text subPanelText;
    float theTime;
    public float speed = 1;
    bool playing;

    public GameObject Panel; 
    public GameObject SuccessPanel;

    string hours;
    string minutes;
    string seconds;

    //public Text scrollText;

    // Use this for initialization
    void Start () {
		//text = GetComponent<Text>();

        //scrollText = GetComponent<Text>();
	}

    // Update is called once per frame

    

    void Update () {

        if (playing)
        {
            theTime += Time.deltaTime * speed;
            //hours = Mathf.Floor((theTime % 21600) / 3600).ToString("00");
            hours = Mathf.Floor((theTime % 356400) / 3600).ToString("00");
            minutes = Mathf.Floor((theTime % 3600) / 60).ToString("00");
            seconds = (theTime % 60).ToString("00");
            jikan.text = hours + ":" + minutes + ":" + seconds;
        }
	}


    public void PlayPause()
    {
        if (playing == true)
        {
            playing = false;
        }
        else if (playing == false){
            playing = true;
        }
    }

    public void Stop()
    {
        playing=false;

       // scrollText.text=text.text;

        //text.text="00:00:00";
       // hours="00";
        //minutes = "00";
        //seconds  = "00";
        //theTime = 0;
        OpenPanel();
    }

    public void OpenPanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(true);
            subPanelText.text= "Add <color=darkblue>" + tagName.text + "</color> record?";
        }
    }

    public void ClosePanel()
    {
        Panel.SetActive(false);
    }

    public void RecordReset()
    {        
        //Panel.SetActive(false);
        //SuccessPanel.SetActive(false);
        //theTime=Time.timeSinceLevelLoad;
        //jikan.text = "00:00:00";
        //tagName.text = "";   
        SceneManager.LoadScene(1);        
    }

    

    


}
