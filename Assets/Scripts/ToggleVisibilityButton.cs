using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Threading;
using TMPro;

public class ToggleVisibilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MonitARField field;
    private gregg gregg;
    private Color _startColor;
    private Renderer _renderer;
    private bool _toggledOn;
         
    void Start(){
        gregg = (gregg)FindObjectOfType(typeof(gregg));
        _renderer = gameObject.GetComponent<Renderer>();
        _toggledOn = false;
        UpdateText();
    }
    
    public void reset(){
        _toggledOn = false;
        UpdateText();
    }

    public void UpdateText(){
        TextMeshPro textMesh = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        if(_toggledOn){
            textMesh.text = "Visibility:\nOn";
        }
        else{
            textMesh.text = "Visibility:\nOff";
        }
    }

    public void OnMouseDown(){
        field.ToggleState();
        if(gregg.inGregg){
            gregg.greggDisappear();
        }
        _toggledOn = !_toggledOn;
        UpdateText();
    }
    
    public void OnPointerClick(PointerEventData data){
        field.ToggleState();
        if(gregg.inGregg){
            gregg.greggDisappear();
        }
        _toggledOn = !_toggledOn;
        UpdateText();
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
