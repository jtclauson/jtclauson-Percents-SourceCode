using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDay : MonoBehaviour {

    public Text output;
    public Dropdown dropdown;
    List<string> names = new List<string> { "日", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10","11","12","13","14",
        "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"};


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
