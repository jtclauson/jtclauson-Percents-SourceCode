using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropMonth : MonoBehaviour {

    public Text output;
    public Dropdown dropdown;
    List<string> names = new List<string> { "月", "01","02","03","04","05","06","07","08","09","10","11","12"};


    public void Dropdown_IndexChanged(int index)
    {
        output.text += "-" + names[index];
    }

    private void Start()
    {
        //dropdown.captionText.text="年";
        insertNames();

    }


    void insertNames()
    {
        dropdown.AddOptions(names);
    }
}
