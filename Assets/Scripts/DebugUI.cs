using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


/*
************************************************************
*   Debug UI for testing in headset
************************************************************
*/
public class DebugUI : MonoBehaviour
{ 
    private Text _text;
    private StringBuilder builder;

    void Start()
    {
        builder     = new StringBuilder();
        _text       = GetComponent<Text>() as Text;
        _text.text  = "This is an example message!!";
    }

    void Update()
    {
       builder.Clear();
       builder.AppendFormat("Mouse <{0}, {1}, {2}>\n", Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
       builder.AppendFormat("MAxis <{0}, {1}>\n", Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
       builder.AppendFormat("Camera <{0}, {1}, {2}>\n", Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
       builder.AppendFormat("Cam Pos <{0}, {1}, {2}>\n", Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
       _text.text =  builder.ToString();
    }
}
