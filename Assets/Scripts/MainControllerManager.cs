using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainControllerManager : MonoBehaviour
{

    [SerializeField] private GameObject _DrawLinePrefab;
    [SerializeField] private Transform _HandAnchor;
    [SerializeField] private float _MaxDistance = 100.0f; // 距離
    [SerializeField] private LineRenderer _LaserPointerRenderer; // LineRenderer

    GameObject CurrentLine;
    GameObject DrawLines;

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
        DrawLines = GameObject.Find("DrawLines");
        DrawLines.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(OnWindows())
        {
            DrawOnSpace();
            if (PointingForm(true))
            {
                ShowLaserPointer();
            }
            else
            {
                HideLaserPointer();
            }
        }
        else
        {
            ShowLaserPointer();
        }

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
                CurrentLine = Instantiate(_DrawLinePrefab, pointer.position, Quaternion.identity);
                LineRenderer DrawLine = CurrentLine.GetComponent<LineRenderer>();
                DrawLine.positionCount = 1;
                DrawLine.SetPosition(0, pointer.position);
            }
            else
            {
                LineRenderer DrawLine = CurrentLine.GetComponent<LineRenderer>();
                int NextPositionIndex = DrawLine.positionCount;
                DrawLine.positionCount = NextPositionIndex + 1;
                DrawLine.SetPosition(NextPositionIndex, pointer.position);
                //Debug.Log(DrawLine);
            }
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            if (CurrentLine != null)
            {
                CurrentLine.transform.SetParent(DrawLines.transform);
                NetworkServer.Spawn(CurrentLine);
                CurrentLine = null;
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
            VRInteractiveItem interactible = hitInfo.collider.GetComponent<VRInteractiveItem>();
            if (interactible)
            {
                interactible.Over();
                if (OVRInput.GetDown(OVRInput.RawButton.A))
                {
                    Debug.Log("Pull Trigger");
                    interactible.Click();
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
        bool HandTriggerIsPulled = false;
        bool ThumbIsTouching = false;
        bool IndexTriggerIsTouched = false;

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

        if (HandTriggerIsPulled && ThumbIsTouching && !IndexTriggerIsTouched)
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
