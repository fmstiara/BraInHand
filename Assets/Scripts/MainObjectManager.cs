using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public partial class MainControllerManager : NetworkBehaviour
{
    [SerializeField] private GameObject ObjectModeText;
    private bool isGrabbing = false;
    private GameObject grabObject;

    private int objectMode = 0; //0=move, 1=add, 2=delete

    void OnChangeObjectMode()
    {
        
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))
        {
            objectMode++;
            if (objectMode > 1)
                objectMode = 0;

            SetObjectMode();
            
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
        {
            objectMode--;
            if (objectMode < 0)
                objectMode = 1;

            SetObjectMode();
        }
    }

    void SetObjectMode()
    {
        Debug.Log("object mode change : " + objectMode.ToString());
        switch (objectMode)
        {
            case 0:
                ObjectModeText.GetComponent<TextMeshPro>().text = "MOVE";
                break;
            case 1:
                ObjectModeText.GetComponent<TextMeshPro>().text = "EDIT";
                break;
            default:
                break;
        }
    }

    void OnAddObject()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            AddDrawObject();
        }
    }

    void AddDrawObject()
    {
        CurrentDrawObject = new GameObject();
        CurrentDrawObject.name = "obj" + DrawObjects.transform.childCount.ToString();
        CurrentDrawObject.tag = "DrawObject";
        CurrentDrawObject.transform.SetParent(DrawObjects.transform);
    }

    void OnMoveObject()
    {
        var pointer = Pointer;
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        if (lastPointerPosition != null)
        {
            Vector3 move = pointer.position - lastPointerPosition;
            //Debug.Log(move);
            grabObject.transform.position += move;
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            isGrabbing = false;
        }
    }

    void ChangeCurrentObject()
    {

    }
}
