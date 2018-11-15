using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerManager : MonoBehaviour {

    [SerializeField] private Transform _HandAnchor;
    [SerializeField] private float _MaxDistance = 100.0f; // 距離
    [SerializeField] private LineRenderer _LaserPointerRenderer; // LineRenderer

    private Transform Pointer
    {
        get
        {
            return _HandAnchor;
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        var pointer = Pointer;
        if(pointer == null)
        {
            Debug.Log("pointer not defiend");
            return;
        }
        if(OnWindows())
        {
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

    bool PointingForm(bool isRight)
    {
        bool HandTriggerIsPulled = false;
        bool ThumbIsTouching = false;
        bool IndexTriggerIsTouched = false;

        if(isRight)
        {
            HandTriggerIsPulled = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
            ThumbIsTouching = OVRInput.Get(OVRInput.RawTouch.RThumbstick)
                || OVRInput.Get(OVRInput.RawTouch.RThumbRest)
                || OVRInput.Get(OVRInput.RawTouch.A)
                || OVRInput.Get(OVRInput.RawTouch.B)
                || OVRInput.Get(OVRInput.RawTouch.X)
                || OVRInput.Get(OVRInput.RawTouch.Y);
            IndexTriggerIsTouched = OVRInput.Get(OVRInput.RawTouch.RIndexTrigger);
        } else
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

    void HideLaserPointer()
    {
        _LaserPointerRenderer.enabled = false;
    }

    void ShowLaserPointer()
    {
        _LaserPointerRenderer.enabled = true;
        var pointer = Pointer; // コントローラーを取得
                               // コントローラーがない or LineRendererがなければ何もしない
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        // コントローラー位置からRayを飛ばす
        Ray pointerRay = new Ray(pointer.position, pointer.forward);

        // レーザーの起点
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
                if (OVRInput.GetDown(OVRInput.RawButton.A)
                    || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
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
