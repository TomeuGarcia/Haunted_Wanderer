using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTextAtLine : MonoBehaviour
{

    public TextAsset theText;

    public int startLine;
    public int endLine;

    public TextBoxManager theTextBox;


    public bool requireButtonPress;
    private bool waitForPress;
    public bool destroyWhenActivated;


    // Start is called before the first frame update
    void Start()
    {
        theTextBox = FindObjectOfType<TextBoxManager>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            theTextBox.ReloadScript(theText);
            theTextBox.currentLine = startLine;
            theTextBox.endAtLine = endLine;
            theTextBox.enableTextBox();


            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            theTextBox.disableTextBox();
        }
    }

    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            theTextBox.disableTextBox();
        }
    }
    */

    /*
    private void OnTriggerEnter2D(Collider other)
    {
        if(other.name == "Player")
        {
            waitForPress = false;
        }
    }
    */

}
