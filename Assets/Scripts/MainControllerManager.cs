using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class MainControllerManager : NetworkBehaviour
{

    [SerializeField] private Transform _HandAnchor;
    [SerializeField] private float _MaxDistance = 100.0f; // 距離
    [SerializeField] private LineRenderer _LaserPointerRenderer; // LineRenderer

    GameObject DrawObjects;
    GameObject CurrentDrawObject;

    private Vector3 lastPointerPosition;
    private int operateMode = 0; //0=draw , 1=object mode, 2=eraser mode

    private Transform Pointer
    {
        get
        {
            return _HandAnchor;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        DrawObjects = GameObject.FindWithTag("DrawObjects");
        DrawObjects.transform.SetParent(null);
        AddDrawObject();
        SetOperateMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (OnWindows())
        {
            if (PointingForm(true))
            {
                ShowLaserPointer();
            }
            else
            {
                HideLaserPointer();
            }

            OnChangeOperateMode();
            switch (operateMode)
            {
                case 0:
                    OnChangeLineColor();
                    DrawOnSpace();
                    break;
                case 1:
                    //object操作
                    OnChangeObjectMode();

                    if (objectMode==0 && isGrabbing)
                    {
                        OnMoveObject();
                    }
                    else if (objectMode == 1)
                    {
                        OnAddObject();
                    }
                    else if (objectMode == 2)
                    {
                        //OnDeleteObject();
                    }
                    break;
                case 2:
                    //eraser mode
                    break;
                default:
                    break;
            }

            var pointer = Pointer;
            if (pointer == null || _LaserPointerRenderer == null)
            {
                return;
            }
            lastPointerPosition = pointer.position;
        }
        else
        {
            ShowLaserPointer();
        }


    }

    void OnChangeOperateMode()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickUp))
        {
            operateMode--;
            if (operateMode < 0)
                operateMode = 1;

            SetOperateMode();
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickDown))
        {

            operateMode++;
            if (operateMode > 1)
                operateMode = 0;

            SetOperateMode();
        }
    }

    void SetOperateMode()
    {
        Debug.Log("change operate mode : " + operateMode.ToString());
        GameObject[] palettes = GameObject.FindGameObjectsWithTag("PaletteObject");
        for(int i=0; i<palettes.Length; i++)
        {
            if(i != operateMode)
            {
                palettes[i].GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                palettes[i].GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    void HideLaserPointer()
    {
        _LaserPointerRenderer.enabled = false;
    }

    void ShowLaserPointer()
    {
        _LaserPointerRenderer.enabled = true;
        var pointer = Pointer;
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        Ray pointerRay = new Ray(pointer.position, pointer.forward);
        _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

        RaycastHit hitInfo;
        if (Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
        {
            // Rayがヒットしたらそこまで
            _LaserPointerRenderer.SetPosition(1, hitInfo.point);

            string tagName = hitInfo.collider.gameObject.tag;
            VRInteractiveItem interactible = hitInfo.collider.GetComponent<VRInteractiveItem>();
            if (interactible)
            {
                interactible.Over();
                if (OVRInput.GetDown(OVRInput.RawButton.A)
                    || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                {
                    Debug.Log(hitInfo.collider);
                    interactible.Click();
                }
            }
            else if (tagName == "DrawLineCollider")
            {
                if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
                {
                    //collider < drawline < drawobject
                    grabObject = hitInfo.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
                    Debug.Log("Grabbing a Object with \"" + grabObject.tag + "\" tag");
                    isGrabbing = true;
                }
            }
        }
        else
        {
            // Rayがヒットしなかったら向いている方向にMaxDistance伸ばす
            _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
        }
    }

    private bool PointingForm(bool isRight)
    {
        bool HandTriggerIsPulled = false; //中指
        bool ThumbIsTouching = false;//親指
        bool IndexTriggerIsTouched = false;//人差し指

        if (isRight)
        {
            HandTriggerIsPulled = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
            ThumbIsTouching = OVRInput.Get(OVRInput.RawTouch.RThumbstick)
                || OVRInput.Get(OVRInput.RawTouch.RThumbRest)
                || OVRInput.Get(OVRInput.RawTouch.A)
                || OVRInput.Get(OVRInput.RawTouch.B)
                || OVRInput.Get(OVRInput.RawTouch.X)
                || OVRInput.Get(OVRInput.RawTouch.Y);
            IndexTriggerIsTouched = OVRInput.Get(OVRInput.RawTouch.RIndexTrigger);
        }
        else
        {
            HandTriggerIsPulled = OVRInput.Get(OVRInput.RawButton.LHandTrigger);
            ThumbIsTouching = OVRInput.Get(OVRInput.RawTouch.LThumbstick)
                || OVRInput.Get(OVRInput.RawTouch.LThumbRest)
                || OVRInput.Get(OVRInput.RawTouch.X)
                || OVRInput.Get(OVRInput.RawTouch.Y);
            IndexTriggerIsTouched = OVRInput.Get(OVRInput.RawTouch.LIndexTrigger);
        }

        //HandTriggerIsPulled
        if (ThumbIsTouching && !IndexTriggerIsTouched)
        {
            return true;
        }
        return false;
    }

    private bool OnWindows()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            return true;
        }
        return false;
    }
}
