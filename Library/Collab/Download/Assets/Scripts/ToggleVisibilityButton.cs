using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleVisibilityButton : MonoBehaviour, IPointerClickHandler
{
    private MonitARField field;
         
    void Start(){
        field = (MonitARField)FindObjectOfType(typeof(MonitARField));
    }
    void OnMouseDown(){
        field.ToggleState();
    }
    
    public void OnPointerClick(PointerEventData data){
        field.ToggleState();
    }
}
