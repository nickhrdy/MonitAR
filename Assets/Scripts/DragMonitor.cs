#define DEMO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
Script for Window prefab. The window is moved via a box collider represented by the task bar.
*/
public class DragMonitor : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    #region Constants
    private const float _RADIUS = 0.60f; //radius from player
    private const int   _DEFAULT_RENDER_VAL = 3000;
    #endregion Constants

    #region Private variables
    private float           _mZCoord;            //mouse's Z coordinate
    private float           _distance;           //distance btwn player and monitor
    private float           _radiusModifier;     //modifier used when setting distance btwn monitors and player. Prevents z-fighting
    private bool            _isColliding;        //Used to tell when the monitors are colliding
    private Vector3         _mOffset;            //gameObject's offset
    private Vector3         _storedPosition;     //stores current position
    private Vector3         _storedLocalScale;   //stores current scale
    private MonitARField    _field;              //MonitARfield
    private UnityEngine.Video.VideoClip _videoClip;     //reference to videoclip associated w/ this monitor
    private UnityEngine.Video.VideoPlayer _videoPlayer; //reference to videoplayer associated w/ this monitor
    #endregion Private variables

    #region Public Methods
    public Resize leftResize;   //currently unused. 
    public Resize rightResize;  //currently unused.
    public int    _renderVal;   //RenderQueue value
    public bool   _canDrag;     //Stops the drag event from occuring when exiting tab mode
    #endregion Public Methods

    void Start()
    {
        _radiusModifier = 0f;
        _renderVal      = _DEFAULT_RENDER_VAL;
        _canDrag        = true;
        _isColliding    = false;
        _field          = (MonitARField)FindObjectOfType(typeof(MonitARField));

        #if (!DEMO) //assign a random texture if the demo flag isn't specified
        setRandomTexture();
        #endif
    }

    /**
        Set an image texture for the monitor
     */
    public void setImage(string imgName)
    {
        GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>(imgName) as Texture;
    }

    /**
        Set a videoClip for the monitor
     */
    public void setVideoClip(string movName){
        _videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        _videoPlayer.playOnAwake = true;
        _videoPlayer.isLooping = true;
        _videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        _videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        _videoPlayer.targetMaterialProperty = "_MainTex";
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        _videoPlayer.clip = Resources.Load<VideoClip>(movName) as VideoClip;
        _videoPlayer.Play();
    }

    /**
        Set a random video or image for the monitor
     */
    public void setRandomTexture(){
        Texture texture = null;
        int choice      = Random.Range(1, 15);
        switch(choice){
            case 1: setImage("Desktop");
                break;
            case 2: setImage("stack_overflow");
                break;
            case 3: setImage("spreadsheet");
                break;
            case 4: setImage("Sample_Screen");
                break;
            case 5: setImage("doggo_search");
                break;
            case 6: setImage("code");
                break;
            case 7: setVideoClip("ml_coding");
                break;
            case 8: setVideoClip("youtube_video");
                break;
            case 9: setVideoClip("word");
                break;
            case 10: setImage("project_spec");
                break;
            case 11: setImage("facebook");
                break;
            case 12: setImage("GoT");
                break;
            case 13: setImage("piazza");
                break;
            case 14: setImage("Spotify");
                break;
            default: setImage("doggo_search");
                break;
        } 
    }

    #region Mouse Events
    private Vector3 GetMouseWorldPos()
    {
        //pixel coordinates(x, y)
        Vector3 mousePoint  = Input.mousePosition;
        mousePoint.z        = _mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDown()
    {
        if (_field.inTabView)
        {
            _canDrag = false;
            Debug.Log("Exiting TabView");
            _field.ExitTabView(gameObject);
            _canDrag = true;
            return;
        }

        _canDrag = true;
        IncreaseRenderQueue();
        _mZCoord    = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        _mOffset     = gameObject.transform.position - GetMouseWorldPos(); //offset <- gameObject world pos - mouse world pos
    }

    void OnMouseDrag()
    {
        if (!_canDrag || _isColliding)
        {
            return;
        }
        Debug.Log("MouseDrag");
        transform.position = GetMouseWorldPos() + _mOffset;
        _distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);

        //correct the distnce if needed
        UpdateDistance();

        transform.LookAt(Camera.main.transform);
    }

    void OnMouseUp(){
        _canDrag = false;
        _radiusModifier = 0f;
        //DecreaseRenderQueue();

        UpdateDistance();

        transform.LookAt(Camera.main.transform);
    }
    #endregion MouseEvents


    #region Pointer Events
    //Pointer equiv. of GetMouseWorldPos()
    private Vector3 GetPointerPos(Vector2 v)
    {
        //pixel coordinates(x, y)
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _mZCoord;

        return Camera.main.WorldToScreenPoint(mousePoint);
    }

    //Mirage controller equiv. of OnMouseDown
    public void OnBeginDrag(PointerEventData data)
    {
        if (_field.inTabView)
        {
            _canDrag = false;
            Debug.Log("Exiting TabView");
            _field.ExitTabView(gameObject);
            _canDrag = true;
            return;
        }
        _canDrag = true;
        _mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset - gameObject world pos - mouse world pos
        _mOffset = gameObject.transform.position - data.pointerCurrentRaycast.worldPosition;

    }

    //Mirage controller equiv. of OnMouseDrag
    public void OnDrag(PointerEventData data)
    {
        if (!_canDrag || _isColliding || rightResize.dragging)
        {
            return;
        }
        transform.position = data.pointerCurrentRaycast.worldPosition + _mOffset;
        _distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);

        //correct the distance if needed
        UpdateDistance();

        transform.LookAt(Camera.main.transform);
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (_field.inTabView)
        {
            Debug.Log("Exiting TabView");
            _field.ExitTabView(gameObject);
        }
    }
    #endregion Pointer Events

    #region Collision Methods
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Start!");
        if(_canDrag){
            _radiusModifier = -0.001f;
            DecreaseRenderQueue(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision Exit!");
    }
    #endregion Collision Methods

    //update distance btwn player and monitor
    private void UpdateDistance()
    {
        transform.position = (transform.position - Camera.main.transform.position).normalized *  (_RADIUS + _radiusModifier) + Camera.main.transform.position;
    }

    //increase the renderqueue of this monitor
    public void IncreaseRenderQueue()
    {
        gameObject.GetComponent<Renderer>().material.renderQueue = 3001;
        foreach (Transform child in transform)
        {
            Renderer temp = child.gameObject.GetComponent<Renderer>();
            temp.material.renderQueue = 3001;
        } 
    }

    //increase the renderqueue of the monitor this collided with 
    public void IncreaseRenderQueue(Collider other)
    {
        GameObject t = other.gameObject;
        t.GetComponent<Renderer>().material.renderQueue = 3001;
        foreach (Transform child in t.transform)
        {
            Renderer temp = child.gameObject.GetComponent<Renderer>();
            temp.material.renderQueue = 3001;
        }
    }

    //decrease the renderqueue of this monitor
    public void DecreaseRenderQueue()
    {
        gameObject.GetComponent<Renderer>().material.renderQueue = 3000;
        foreach (Transform child in transform)
        {
            Renderer temp = child.gameObject.GetComponent<Renderer>();
            temp.material.renderQueue = 3000;
        }
    }

    //decrease the renderqueue of the monitor this collided with 
    public void DecreaseRenderQueue(Collider other)
    {
        GameObject t = other.gameObject;
        DragMonitor test = t.GetComponent<DragMonitor>();
        if(test && !test._canDrag){
            t.GetComponent<Renderer>().material.renderQueue = _renderVal - 1;
            test._renderVal = _renderVal - 1;
            foreach (Transform child in t.transform)
            {
                Renderer temp = child.gameObject.GetComponent<Renderer>();
                temp.material.renderQueue = _renderVal - 1;
            }
        }
    }
}
