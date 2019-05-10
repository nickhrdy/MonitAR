using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

public class ToggleMonitors : MonoBehaviour, IPointerClickHandler
{
    private bool buttonPressed;
    private bool isOpen;
    public MonitARDonut md;
    public GameObject st;
    
    // Start is called before the first frame update
    void Start()
    {
        buttonPressed = false;
        isOpen = false;
    }

    public void OnPointerClick(PointerEventData data)
    {
        ToggleButton();
    }
    
    void OnMouseDown()
    {
        ToggleButton();
    }
    
    void ToggleButton(){       
        buttonPressed = !buttonPressed;
        
         if (buttonPressed)
        {
            st.gameObject.SetActive(!st.gameObject.activeSelf);
            if (isOpen)
            {
                md.ToggleState();
            }
            isOpen = !isOpen;
        }
    }
}
