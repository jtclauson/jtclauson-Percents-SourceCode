using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearText : MonoBehaviour {

	public Text text;

    public void Clear()
    {
        text.text="";
    }
}
