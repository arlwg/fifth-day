using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Header("Body parts")]
    public Transform rightHand;
    public Transform leftHand;
    public Transform playerBody;
    
    float XRotation = 0f;
    float mouseX = 0;
    float mouseY = 0;
    bool isJoystickUsed = false;
    private PlayerController _player;
    
    
    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        //Cursor.lockState = CursorLockMode.Locked;//@@@@@ Before deploying to mobile comment this line 
    }

    void Update()
    {
        if(!FindObjectOfType<DragItem>().inAction)
        {
            isJoystickUsed = (_player.joystick.Horizontal == 0 && _player.joystick.Vertical == 0 ) ? false : true;

                if (Application.isMobilePlatform && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !isJoystickUsed)
                {
                    mouseX = Input.GetTouch(0).deltaPosition.x * 0.3f;
                    mouseY = Input.GetTouch(0).deltaPosition.y * 0.3f;
                }
                else if(Application.isMobilePlatform && (Input.touchCount > 0 && Input.touchCount < 2 && isJoystickUsed) || Input.touchCount == 0 || (Input.touchCount > 0 && !isJoystickUsed && Input.GetTouch(0).phase == TouchPhase.Stationary))
                {
                    mouseX = 0.0f;
                    mouseY = 0.0f;
                }

                if (_player.enableMouseRotation)
                {
                    mouseX = Input.GetAxis("Mouse X") * _player.mouseSensetivity;
                     mouseY =  Input.GetAxis("Mouse Y") *  _player.mouseSensetivity;
                }

        
            XRotation -= mouseY;
            XRotation = Mathf.Clamp(XRotation,_player.MaxMinYCameraRotation.x,_player.MaxMinYCameraRotation.y);

            transform.localRotation = Quaternion.Euler(XRotation,0f,0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        
    }
}
