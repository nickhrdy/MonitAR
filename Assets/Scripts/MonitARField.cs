#define DEMO
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MonitARField : MonoBehaviour
{
    /**
        Class to save certain attributes of a Gameobject in a single bundle
     */
    private class MonitorAttr{
        public Vector3 location;    //location of the monitor
        public Vector3 scale;       //localScale of the monitor
        public float angleOffset;   //angle offset of the monitor from the player's forward direction
        
        public MonitorAttr(GameObject obj)
        {
            location    = obj.transform.position;
            scale       = obj.transform.localScale;
            angleOffset = Vector3.SignedAngle(RemoveY(Camera.main.transform.forward), RemoveY(location - Camera.main.transform.position), Vector3.up);
        }

        public MonitorAttr(Vector3 location, Vector3 scale)
        {
            this.location   = location;
            this.scale      = scale;
        }
    }
    
    
    #region Constants
    private const float _radius             = 0.60f; //radius from player
    private const float _cascade_radius     = 0.50f; //tab view radius from the player
    private const float _cascade_margin     = 4.20f; //margin btwn items when cascading
    private const float _tab_view_distance  = 0.20f; //radius from the player for tab view
    #endregion Constants


    #region Private Variables
    private Vector3                 _previewSize;
    private List<GameObject>        children;           //Stores the monitors
    private List<Vector3>           locationStorage;    //Used to store monitors transform during certain operations
    private List<Vector3>           sizeStorage;        //Used to store local size
    private List<MonitorAttr>       attrStorage;        //Stores monitor attributes for tab view
    private ToggleVisibilityButton  _vbutton;           //Visibility toggle button
    private int                     _sceneResetTimer;   //Counter to determine when to reset the view
    private int                     _monitorCreatedCount;       //Used to control what monitors to pop up during demo
    private bool                    _usingController;   //Used in resetScene() to flag whether the input in a controller or a keyboard
    private string[] startupimages = { "project_spec", "stack_overflow", "Desktop", "facebook", "spotify" }; //monitor images to load on startup
    //private string[] startupimages = { "project_spec", "stack_overflow", "Desktop", "facebook", "spotify", "doggo_search", "Sample_screen", "piazza" };
    private string[] startupmovies = { "ml_coding" };                  //VideoClips to load on startup
    //private string[] startupmovies = { "ml_coding", "youtube_video",  "word" };
    #endregion Private Variables


    #region Public Variables
    public bool         inTabView;      //in tabView or not
    public GameObject   monitorPrefab;  //prefab of monitor
    public gregg        gregg;
    public GameObject   Player;
    #endregion Public Variables


    void Start()
    {
        #region Initialization
        inTabView       = false;
        children        = new List<GameObject>();
        attrStorage     = new List<MonitorAttr>();
        locationStorage = new List<Vector3>();
        sizeStorage     = new List<Vector3>();
        _previewSize    = new Vector3(0.4759674f, 0.2677317f, 0f);
        _sceneResetTimer      = 0;
        gameObject.SetActive(true); //default state of the monitors
        _vbutton = (ToggleVisibilityButton)FindObjectOfType(typeof(ToggleVisibilityButton));
        #endregion Initialization
        
        resetScene(); //setup scene
        gregg.resetGregg();
        _vbutton.reset();
    }


    /**
    Resets the scene to it's original state
     */
    private void resetScene()
    {
        //Destroy old monitors
        foreach (GameObject obj in children){
            Destroy(obj);
        }

        //clear storage lists
        Player.transform.position = new Vector3(0,0,0);
        children.Clear();
        attrStorage.Clear();
        inTabView = false;
        gameObject.SetActive(true);

    #if DEMO
        _monitorCreatedCount = 0;
        GameObject monitor;
        foreach (string s in startupimages)
        {
            monitor         = Instantiate(monitorPrefab);
            var component   = monitor.GetComponent<DragMonitor>();
            component.setImage(s);
            children.Add(monitor);
        }

        foreach (string s in startupmovies){
            monitor         = Instantiate(monitorPrefab);
            var component   = monitor.GetComponent<DragMonitor>();
            component.setVideoClip(s);
            children.Add(monitor);
        }
        ToggleState();
    #else
        //Get all children (if any)
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
    #endif

        //position any children
        if (children.Count != 0)
        {
            //Set default position for all children
    #if DEMO
            float increment = 360f / 6f;
    #else
            float increment = 360f / (float)children.Count;
    #endif

            //Set position of all children
            //children will be set in two rows
            float angleOffset = 0;
            int i = 0;
            foreach (GameObject obj in children)
            {
                var offset = new Vector3();
                if (i < 6)
                {
                    offset = new Vector3(Mathf.Cos(angleOffset * Mathf.Deg2Rad) * _radius, 0, Mathf.Sin(angleOffset * Mathf.Deg2Rad) * _radius);
                }
                else
                {
                    offset = new Vector3(Mathf.Cos((angleOffset - 25) * Mathf.Deg2Rad) * _radius, Mathf.Sin(30 * Mathf.Deg2Rad) * _radius, Mathf.Sin((angleOffset - 25) * Mathf.Deg2Rad) * _radius);
                }
                obj.transform.position = Camera.main.transform.position + offset;
                obj.transform.LookAt(Camera.main.transform);
                angleOffset += increment;
                i++;
            }
        }

        gregg.resetGregg();
        _vbutton.reset();
    }


    /**
    Update method rigged to detect input for resetting the scene.
    Keyboard: hold 'period;
    Controller: hold the trackpad button and the minus button simultaneously
    */
    public void Update(){
        if(Input.GetKey(KeyCode.Period)){
            Debug.Log("Keyboard: Reset button pressed", this);
            _usingController = false;
            _sceneResetTimer += 1;
        }
        else if(GvrControllerInput.ClickButton && GvrControllerInput.AppButton){
            Debug.Log("Controller: Reset buttons pressed", this);
            _usingController = true;
            _sceneResetTimer += 1;
        }

        if(_sceneResetTimer == 100){
            Debug.Log("Resetting Scene!", this);
            resetScene();
            _sceneResetTimer = 0;
        }
        if(Input.GetKeyUp(KeyCode.Period)){
            Debug.Log("Keyboard: Reset button unpressed", this);
            _sceneResetTimer = 0;
        }
        if(!GvrControllerInput.ClickButton && !GvrControllerInput.AppButton && _usingController){
            Debug.Log("Controller: Reset buttons unpressed", this);
            _sceneResetTimer = 0;
        }
    }


    //Toggles the visibility of all the monitors 
    public void ToggleState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        foreach (GameObject obj in children)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }


    //Creates a monitor where the user is looking
    public void CreateMonitor()
    {
        //prevent user from creating a monitor when the donut is set to invisible
        if (!gameObject.activeSelf)
        {
            Debug.Log("Monitor Visibility toggled off! Can't create monitors!", this);
            return;
        }
        Debug.Log("Creating monitor!");

        //Create clone of prefab and position it in front of the player
        GameObject temp = Instantiate(monitorPrefab);
        temp.transform.position = Camera.main.transform.position + Camera.main.transform.forward * _radius;
        temp.transform.LookAt(Camera.main.transform);

    #if DEMO
        //force the first two monitors to be dragmonitors
        if(_monitorCreatedCount == 1){
            var component   = temp.GetComponent<DragMonitor>();
            component.setVideoClip("Word");
            _monitorCreatedCount++;
        }
        if(_monitorCreatedCount == 0){
            var component   = temp.GetComponent<DragMonitor>();
            component.setVideoClip("youtube_video");
            _monitorCreatedCount++;
        }
        if(_monitorCreatedCount > 1){
            var component   = temp.GetComponent<DragMonitor>();
            component.setRandomTexture();
            _monitorCreatedCount++;
        }
    #endif

        children.Add(temp);
    }

    //Tries to remove the referenced object from the list of monitors
    //Returns true if an object was successfully removed, false otherwise.
    public bool RemoveMonitor(GameObject obj)
    {
        if (obj != null && children.Remove(obj))
        {
            Destroy(obj);
            Debug.Log("Monitor Removed!", this);
            return true;
        }
        Debug.Log("Failed to remove monitor", obj);
        return false;
    }

    /*
    Show all monitors in front of the player in rows of three
     */
    public void TabView()
    {
        //store all transforms/sizes for later if not already in tab view
        if (!inTabView)
        {
            foreach (GameObject obj in children)
            {
                attrStorage.Add(new MonitorAttr(obj));
            }
        }

        Quaternion p = new Quaternion();
        //get the correct rotation by making a temp object look at the Camera.main
        GameObject temp = new GameObject();
        temp.transform.position = Camera.main.transform.position + Camera.main.transform.forward * _tab_view_distance;
        temp.transform.LookAt(Camera.main.transform);
        p = temp.transform.rotation;
        Destroy(temp);

        Vector3 cross = Vector3.Cross(Camera.main.transform.forward * 0.35f, Vector3.up);
        Vector3 right = Vector3.Cross(Camera.main.transform.forward * _tab_view_distance, -Vector3.up);
        Debug.Log("TabView - Cross:" + cross);
        Vector3 start = (cross + Vector3.Normalize(Camera.main.transform.forward) * _cascade_radius) + new Vector3(0, 0.175f, 0);
        

        //find grid size
        float rowSize = Mathf.Ceil(Mathf.Sqrt(children.Count));
        Vector3 horizontalOff = (right) * _cascade_margin * (1/rowSize);
        float i = 0;
        Vector3 offset = start;
        foreach (GameObject obj in children)
        {
            obj.transform.localScale = _previewSize * 1.5f * (1 / rowSize);
            obj.transform.position = offset;
            obj.transform.LookAt(Camera.main.transform);
            obj.transform.rotation = p;

            //build grid
            //these numbers are stupid specific, change at your own risk
            i++;
            if (i % rowSize == 0 && i != 0)
            {
                if(rowSize <= 3){
                    //move down and reset horizontal position
                    offset = start + new Vector3(0, -0.11f * (7f / (rowSize)) * (i / (rowSize)), 0);
                }
                else{
                    //move down and reset horizontal position
                    offset = start + new Vector3(0, -0.11f * (7f / (rowSize+1)) * (i / (rowSize+1)), 0);
                }
            }
            else
            {
                //shift offset
                offset += horizontalOff;
            }
        }
        inTabView = true;
    }

    //Called when clicking a monitor while in tab view
    public void ExitTabView(GameObject clickedObj)
    {
        //move windows back to stored postitions
        int idx = children.IndexOf(clickedObj);
        float clickedObjOffset = attrStorage[idx].angleOffset;
        Vector3 offset;
        int i = 0;
        foreach (GameObject obj in children)
        {
            obj.transform.localScale = attrStorage[i].scale;
            
            float angle = Vector3.SignedAngle(Vector3.forward, Camera.main.transform.forward, Vector3.up) + attrStorage[i].angleOffset - clickedObjOffset;
            //if(i == idx){
                //offset = RemoveY(Camera.main.transform.forward) *  0.6f;
                offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * _radius, 0, Mathf.Cos(angle * Mathf.Deg2Rad) * _radius);
            //}
            //else{
                //should have set all monitor back to there position regardless of thier original y coord, but this line would make every monitor rise further up than thier original location
                //offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * _radius  , attrStorage[i].location.y , Mathf.Cos(angle * Mathf.Deg2Rad) * _radius);
            //}
            obj.transform.position = Camera.main.transform.position + offset;
            
            obj.transform.LookAt(Camera.main.transform);
            i++;
        }

        locationStorage.Clear();
        sizeStorage.Clear();
        attrStorage.Clear();
        inTabView = false;
    }

    /**
    Helper method to replace the y value of a vector
     */
    private static Vector3 ReplaceY(Vector3 v, float y){
        return new Vector3(v.x, y, v.z);
    }

    /**
    Special case of ReplaceY
     */
    private static  Vector3 RemoveY(Vector3 v){
        return ReplaceY(v, 0);
    }
}
