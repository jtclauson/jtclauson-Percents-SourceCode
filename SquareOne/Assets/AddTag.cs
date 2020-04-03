using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AddTag : MonoBehaviour {

	public InputField tagNameField;
    public Button submitButton;

   

    public void ReloadLevel()
    {
        SceneManager.LoadScene(2);
    }

    public void CallSubmitTag()
    {
        StartCoroutine(SubmitTag());
        ReloadLevel();
       
        
    }

    IEnumerator SubmitTag()
    {
        WWWForm form = new WWWForm();
        form.AddField("tagname", tagNameField.text);
        WWW www = new WWW("http://localhost:81/percentsdata/submittag.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("Tag created successfully!!");
            
            
        }
        else
        {
            Debug.Log("Something went horribly wrong in the tag creation: Error number "+www.text);
        }
    }

    //public void VerifyInputs()
    //{
    //    submitButton.interactable = (tagNameField.text.Length >= 1);
    //}
}
