using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
     	Camera.main.transform.position = new Vector3(-2.035f,5.25f,12.55f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
