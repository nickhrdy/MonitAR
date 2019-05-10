using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The System Tray floats at the bottom of the user's view.
It holds common functionality for modifying window characteristics
*/
public class SystemTray : MonoBehaviour
{
    private Camera playerCamera;
    private const float _distance = 0.3f;  //radius from player
    private const float _yCoord = -0.06f;  //y position of the System Tray

    // Start is called before the first frame update
    void Start(){
        playerCamera = Camera.main;
    }

    //Update is called once per frame
    //Makes the system tray follow the player's head
    void Update(){       
        gameObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * _distance;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, _yCoord, gameObject.transform.position.z);
        gameObject.transform.LookAt(playerCamera.transform);
    }
}
