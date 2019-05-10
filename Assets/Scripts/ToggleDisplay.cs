using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

/*
Toggles the SystemTray and the MonitARField
This is attached to a red button used for testing
*/
public class ToggleDisplay : MonoBehaviour, IPointerClickHandler
{
    #region Public Variables

    public MonitARField field;
    public GameObject sysTray;

    #endregion Public Variables

    #region Private Variables

    private bool buttonPressed;
    private bool isOpen;
        
    #endregion Private Variables
    
    
    // Start is called before the first frame update
    void Start(){
        gameObject.SetActive(false);
        buttonPressed = false;
        isOpen = false;
    }

    public void OnPointerClick(PointerEventData data){
        ToggleButton();
    }
    
    void OnMouseDown(){
        ToggleButton();
    }
    
    void ToggleButton(){       
        Debug.Log("Toggling entire display");
        buttonPressed = !buttonPressed;
        
        if (buttonPressed)
        {
            sysTray.gameObject.SetActive(!sysTray.gameObject.activeSelf);
            if (isOpen)
            {
                field.ToggleState();
            }
            isOpen = !isOpen;
        }
    }
}
