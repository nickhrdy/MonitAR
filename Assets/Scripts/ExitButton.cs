using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
************************************************************
*   Script for the exit button on Windows
************************************************************
*/
public class ExitButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private MonitARField    _field;         //main Monitar field
    private Color           _startColor;    //color of button on startup
    private Renderer        _renderer;      //button's renderer

    // Start is called before the first frame update
    void Start()
    {
        _renderer   = gameObject.GetComponent<Renderer>();
        _field      = (MonitARField)FindObjectOfType(typeof(MonitARField));
    }

    #region Mouse Events
    void OnMouseDown()
    {
        //get the top level parent's gameobject via the transform
        _field.RemoveMonitor(gameObject.transform.parent.gameObject);
    }

    public void OnMouseEnter()
    {
        //Highlight object on mouse hover
        _startColor = _renderer.material.color;
        _renderer.material.color = Color.yellow;
    }

    public void OnMouseExit()
    {
        //Unhighlight object
        _renderer.material.color = _startColor;
    }
    #endregion Mouse Events

    #region Pointer Events
    public void OnPointerClick(PointerEventData data)
    {
        //get the top level parent's gameobject via the transform
        _field.RemoveMonitor(gameObject.transform.parent.gameObject);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //Highlight object on controller pointer hover
        _startColor = _renderer.material.color;
        _renderer.material.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData data)
    {
        //Unhighlight object
        _renderer.material.color = _startColor;
    }
    #endregion Pointer Events

}
