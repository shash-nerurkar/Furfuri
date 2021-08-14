using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementJoystick : MonoBehaviour
{
    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickVec;
    public Vector2 joystickTouchPos;
    public Vector2 joystickOriginalPos;
    public float joystickRadius;

    void Start() 
    {
        joystickOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 2;
    }
    
    // FOR FLOATING JOYSTICK
    public void PointerDown() 
    {
        joystick.transform.position = Input.mousePosition;
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;
    }

    public void Drag(BaseEventData baseEventData) 
    { 
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist =  Vector2.Distance(dragPos, joystickTouchPos);

        if(joystickDist < joystickRadius)
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }
        else
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }
    }
    
    public void PointerUp() 
    {
        joystickVec = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
        // FOR FLOATING JOYSTICK
        joystickBG.transform.position = joystickOriginalPos;
    }

    public void RevertToBasePosition()
    {
        joystickVec = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
        // FOR FLOATING JOYSTICK
        joystickBG.transform.position = joystickOriginalPos;
    }
}
