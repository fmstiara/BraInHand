using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainControllerManager : NetworkBehaviour
{

    [SerializeField] private GameObject _DrawLinePrefab;
    [SerializeField] private Transform _HandAnchor;
    [SerializeField] private float _MaxDistance = 100.0f; // 距離
    [SerializeField] private LineRenderer _LaserPointerRenderer; // LineRenderer
    [SerializeField] private GameObject PaletteColor;

    GameObject CurrentLine;
    GameObject DrawLines;

    private Vector3 lastPointerPosition;
    private bool isGrabbing = false;
    private GameObject grabObject;

    private int penMode = 0; //0=black, 1=red, 2=green, 3=blue, 4=yellow
    private int operateMode = 0; //0=draw mode, 1=object mode, 2=eraser mode


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

        DrawLines = GameObject.FindWithTag("DrawObjects");
        DrawLines.transform.SetParent(null);
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
            OnChangeLineColor();
            OnChangeMode();
            DrawOnSpace();
            if (PointingForm(true))
            {
                ShowLaserPointer();
            }
            else
            {
                HideLaserPointer();
            }

            if (isGrabbing)
            {
                MoveObject();
            }
        }
        else
        {
            ShowLaserPointer();
        }


    }

    void MoveObject()
    {
        var pointer = Pointer;
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        if(lastPointerPosition != null)
        {
            Vector3 move = pointer.position - lastPointerPosition;
            Debug.Log(move);
            grabObject.transform.position += move;
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            isGrabbing = false;
        }
        lastPointerPosition = pointer.position;
    }

    void DrawOnSpace()
    {
        var pointer = Pointer;
        if (pointer == null)
        {
            Debug.Log("pointer not defiend");
            return;
        }

        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            if (CurrentLine == null)
            {
                CurrentLine = Instantiate(_DrawLinePrefab, new Vector3(0,0,0), Quaternion.identity);
                NetworkLineRenderer line = CurrentLine.GetComponent<NetworkLineRenderer>();
                NetworkServer.Spawn(CurrentLine);
                line.CmdSetColorMode(penMode);
                line.CmdAddPosition(pointer.position);
            }
            else
            {
                NetworkLineRenderer line = CurrentLine.GetComponent<NetworkLineRenderer>();
                line.CmdAddPosition(pointer.position);
            }
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            if (CurrentLine != null)
            {
                CurrentLine.transform.SetParent(DrawLines.transform);
                CurrentLine = null;
            }
        }
    }

    void OnChangeMode()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickUp))
        {
            operateMode++;
            if (operateMode > 2)
                operateMode = 0;
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickDown))
        {
            operateMode--;
            if (operateMode > 2)
                operateMode = 2;
        }
    }

    void OnChangeLineColor()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))
        {
            penMode++;
            if (penMode > 4)
                penMode = 0;

            PaletteColor.GetComponent<Renderer>().material.color = GetColor(penMode);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
        {
            penMode--;
            if (penMode < 0)
                penMode = 4;

            PaletteColor.GetComponent<Renderer>().material.color = GetColor(penMode);
        }
    }

    Color GetColor(int _colorMode)
    {
        Color res;
        switch (_colorMode)
        {
            case 0:
                res = new Color(0.2f, 0.2f, 0.2f);
                break;
            case 1:
                res = new Color(0.9f, 0.2f, 0.2f);
                break;
            case 2:
                res = new Color(0.2f, 0.9f, 0.2f);
                break;
            case 3:
                res = new Color(0.2f, 0.2f, 0.9f);
                break;
            case 4:
                res = new Color(0.9f, 0.9f, 0.2f);
                break;
            default:
                res = new Color(0.2f, 0.2f, 0.2f);
                break;
        }
        return res;
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
                    Debug.Log("Grabbing a Object");
                    grabObject = hitInfo.collider.gameObject.transform.root.gameObject;
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
