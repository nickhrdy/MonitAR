using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/**
Script to attach to button to toggle tab view
 */
public class TabView : MonoBehaviour, IPointerClickHandler
{
    public MonitARField field;

    void OnMouseDown(){
        field.TabView();
    }

    public void OnPointerClick(PointerEventData data){
        field.TabView();
    }
}
