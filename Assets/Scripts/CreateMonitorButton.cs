using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
************************************************************
*   Script for the system tray button that creates monitors.
************************************************************
*/
public class CreateMonitorButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private MonitARField    _field;         //main Monitar field
    private Color           _startColor;    //color of button on startup
    private Renderer        _renderer;      //button's renderer

    void Start()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        _field = (MonitARField)FindObjectOfType(typeof(MonitARField));
    }

    #region Mouse Events
    void OnMouseDown()
    {
        //tell monitarField to create monitor
        _field = (MonitARField)FindObjectOfType(typeof(MonitARField));
        _field.CreateMonitor();
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
        //tell monitarField to create monitor
        _field = (MonitARField)FindObjectOfType(typeof(MonitARField));
        _field.CreateMonitor();
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
