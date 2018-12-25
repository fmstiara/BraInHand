using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class MainControllerManager : NetworkBehaviour
{

    [SerializeField] private Transform _HandAnchor;
    [SerializeField] private Transform _SubHandAnchor;
    [SerializeField] private float _MaxDistance = 100.0f; // 距離
    [SerializeField] private LineRenderer _LaserPointerRenderer; // LineRenderer

    GameObject CurrentDrawObject;

    private Vector3 lastPointerPosition;
    private Vector3 lastHandAnchorsDiff;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        AddDrawObject();
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

            OnChangeLineColor();
            DrawOnSpace();
            OnMoveObject();
            OnAddObject();

            if (_HandAnchor == null || _LaserPointerRenderer == null)
            {
                return;
            }

            lastPointerPosition = _HandAnchor.position;
            lastHandAnchorsDiff = _HandAnchor.position - _SubHandAnchor.position;
        }
        else
        {
            ShowLaserPointer();
        }
    }

    void HideLaserPointer()
    {
        _LaserPointerRenderer.enabled = false;
    }

    void ShowLaserPointer()
    {
        _LaserPointerRenderer.enabled = true;
       if (_HandAnchor == null || _LaserPointerRenderer == null)
        {
            return;
        }
        Ray pointerRay = new Ray(_HandAnchor.position, _HandAnchor.forward);
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
                else if (OVRInput.GetDown(OVRInput.RawButton.B))
                {
                    // line objectの削除
                    GameObject line = hitInfo.collider.gameObject.transform.parent.gameObject;
                    Destroy(line);
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
