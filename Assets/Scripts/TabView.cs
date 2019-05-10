using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/**
Script to attach to button to toggle tab view
 */
public class TabView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MonitARField field;
    private Color _startColor;
    private Renderer _renderer;

    void Start(){
        _renderer = gameObject.GetComponent<Renderer>();
    }

    void OnMouseDown(){
        field.TabView();
    }

    public void OnPointerClick(PointerEventData data){
        field.TabView();
    }

    public void OnMouseEnter(){
        //Highlight object on mouse hover
       _startColor = _renderer.material.color;
       _renderer.material.color = Color.yellow;
    }

    public void OnMouseExit(){
        //Unhighlight object
        _renderer.material.color = _startColor;
    }

    public void OnPointerEnter(PointerEventData data){
        //Highlight object on controller pointer hover
        _startColor = _renderer.material.color;
       _renderer.material.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData data){
        //Unhighlight object
        _renderer.material.color = _startColor;
    }
}
