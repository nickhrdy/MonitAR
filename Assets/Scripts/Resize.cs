using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

/***
Allows monitors to resize
This script should be attached to the corners of a monitor prefab.
 */
public class Resize : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Constants
    private const float _SIZING_FACTOR  = 0.01f;
    private const float _SIZING_FACTOR2 = 0.005f;
    private const float _RADIUS         = 0.45f; //radius from player
    private const float _POSITION_Z     = 10.0f; 
    #endregion Constants

    #region Private Variables
    private MonitARField    field;              //reference to main monitar field
    private gregg           gregg;              //reference to gregg to let him know when the monitor is resizing
    private GameObject      lastSpawn = null;   //holds last known position when resizing
    private float           startSize;          //Start size of the monitor
    private float           startSizeX;         //startSize.x
    private float           startSizeY;         //startSize.y
    private Vector3         _dragStartPosition; //location where drag started
    private float           startX;             //location.x where drag started
    private float           startY;             //location.y where drag started
    private float           _distance;          //distance btwn player and monitor
    private Color           _startColor;        //material start color
    private Renderer        _renderer;          //gameObject renderer
    private Plane           _plane;             //plane use to calculate pointer displacement
    #endregion Private Variables
    
    public GvrControllerReticleVisual pointer;
    public GameObject controller;
    public bool dragging;                       //flag for if the monitor is dragging or not

    void Start(){
        gregg = (gregg)FindObjectOfType(typeof(gregg));
        field = (MonitARField)FindObjectOfType(typeof(MonitARField));
        _renderer = gameObject.GetComponent<Renderer>();
        _plane = new Plane(transform.parent.up, transform.parent.position);
        dragging = false;
    }

    #region Mouse Events

    void OnMouseDown(){
        Vector3 position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y,_POSITION_Z);
        startX = position.x;
        startY = position.y;
        Debug.Log(position.x);
        Debug.Log(position.y);
        position = Camera.main.WorldToScreenPoint(position);
        lastSpawn = transform.parent.gameObject as GameObject;
        startSize = lastSpawn.transform.localScale.z;
        startSizeX = lastSpawn.transform.localScale.x;
        startSizeY = lastSpawn.transform.localScale.y;

        gregg.greggAppear();
    }
    
    public void OnMouseDrag(){
        dragging = true;
        Vector3 size = lastSpawn.transform.localScale;
        Debug.Log(string.Format("{0}, {1}",Input.mousePosition.x,Input.mousePosition.y));

        size.x = startSizeX + (Input.mousePosition.x - startX) * _SIZING_FACTOR;
        size.y = startSizeY + (Input.mousePosition.y - startY) * _SIZING_FACTOR * -1;

        if(size.x < 0) size.x = 0.05f;
        if(size.x > 100) size.x = 100;
        if(size.y < 0) size.y = 0.05f;
        if(size.y > 100) size.y = 100;

        lastSpawn.transform.localScale = size;
    }
    
    public void OnMouseUp(){
        if(dragging){
            lastSpawn.transform.LookAt(Camera.main.transform);
        }
        dragging = false;
             
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

    #endregion Mouse Events

    #region Pointer Events

    //Mirage controller equiv. of OnMouseDown
    public void OnBeginDrag(PointerEventData data){
        pointer = (GvrControllerReticleVisual)FindObjectOfType(typeof(GvrControllerReticleVisual));
        try{
        controller = (GameObject)FindObjectOfType(typeof(GvrTrackedController));
        }catch(Exception e){
            controller = (GameObject)GameObject.Find("GvrControllerPointer");
        }

        
        if(!dragging){
        Debug.Log("Press Position: " +  data.position);
        Vector3 position = data.position;
        _distance = Vector3.Distance(Camera.main.transform.position, position);
    
        float enter;
        var ray = new Ray(controller.transform.position, controller.transform.forward);
        _plane.Raycast(ray, out enter);
        _dragStartPosition = ray.GetPoint(enter);

        startX = position.x;
        startY = position.y;

        position = Camera.main.WorldToScreenPoint(position);
        lastSpawn = transform.parent.gameObject as GameObject;
        startSize = lastSpawn.transform.localScale.z;
        startSizeX = lastSpawn.transform.localScale.x;
        startSizeY = lastSpawn.transform.localScale.y;
        dragging = true;
        gregg.greggAppear();
        
        }
    }
    
    //Mirage controller equiv. of OnMouseDrag
    public void OnDrag(PointerEventData data){
        //Debug.Log(data.position);
        if(data.dragging){
            dragging = true;
            Vector3 size = lastSpawn.transform.localScale;
            Vector3 position = new Vector3(data.position.x, data.position.y, Camera.main.transform.position.z);
            
            _distance = Vector3.Distance(_dragStartPosition, position);

            _plane = new Plane(transform.parent.up, transform.parent.position);
            float enter = 0;
            var ray = new Ray(controller.transform.position, controller.transform.forward);
            Vector3 diffDistance;
            _plane.Raycast(ray, out enter);
            diffDistance =  _dragStartPosition - ray.GetPoint(enter);

            if(diffDistance.x < 30f && diffDistance.x > -30f && diffDistance.z > -30f && diffDistance.z < 30f){

            size.x = startSizeX + ((diffDistance.x) * -0.2f);
            if(size.x < 0) size.x = 0.05f;
            if(size.x > 100) size.x = 100;
            size.y = startSizeY + ((diffDistance.z) * -0.1f);
            if(size.y < 0) size.y = 0.05f;
            if(size.y > 100) size.y = 100;

            lastSpawn.transform.localScale = size;}
        }
    }
    
    public void OnEndDrag(PointerEventData data){
        if(dragging){
            lastSpawn.transform.LookAt(Camera.main.transform);
        }
        dragging = false;
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
    #endregion Pointer Events



}

