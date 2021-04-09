using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{

    public GameObject textBox;

    public Text theText;

    public TextAsset textFile;
    public string[] textLines;


    public int currentLine;
    public int endAtLine;

    public PlayerController player;

    public bool isActive;
    


    // Start is called before the first frame update
    void Start()
    {
     
        player = FindObjectOfType<PlayerController>();

        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }
        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        if (isActive)
        {
            enableTextBox();
        }
        else
        {
            disableTextBox();
        }
    }

    //checks this every frame
    void Update()
    {


        if (!isActive)
        {
            return;
        }
        

        theText.text = textLines[currentLine];
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentLine += 1;
        }

        if (currentLine > endAtLine)
        {
            disableTextBox();
        }
    }

    //collider 
    public void enableTextBox()
    {
        textBox.SetActive(true);
        isActive = true;
    }


    public void disableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
    }


    public void ReloadScript(TextAsset theText)
    {
        if(theText != null)
        {
            textLines = new string[1];
            textLines = (theText.text.Split('\n'));
        }
    }

}