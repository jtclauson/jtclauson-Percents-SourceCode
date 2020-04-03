using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropYear : MonoBehaviour {

    public Text output;
    public Dropdown dropdown;
    List<string> names = new List<string> { "年", "2019", "2020"};


    public void Dropdown_IndexChanged(int index)
    {
        output.text=names[index];
    }

    private void Start()
    {
        //dropdown.captionText.text="年";
        insertNames();
        
    }


    void insertNames  () {
		dropdown.AddOptions(names);
    }	
}
