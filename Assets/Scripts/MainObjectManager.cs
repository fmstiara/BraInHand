using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public partial class MainControllerManager : NetworkBehaviour
{
    [SerializeField] private GameObject _DrawObjectPrefab;
    private bool isGrabbing = false;
    private GameObject grabObject;
    private float baseDistance = -1;

    void OnAddObject()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            AddDrawObject();
        }
    }

    void AddDrawObject()
    {
        CurrentDrawObject = null;
        CurrentDrawObject = Instantiate(_DrawObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(CurrentDrawObject);
    }

    void OnMoveObject()
    {
        if (_HandAnchor == null || _LaserPointerRenderer == null)
        {
            return;
        }
        if (lastPointerPosition != null && isGrabbing)
        {
            if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))//両手で中指を引いてるとき
            {
                Debug.Log("double grab");
                Vector3 currentHandAnchorsDiff = _HandAnchor.position - _SubHandAnchor.position;
                if (baseDistance < 0)
                {
                    baseDistance = currentHandAnchorsDiff.magnitude;
                }
                else
                {
                    float distRate = currentHandAnchorsDiff.magnitude / baseDistance;
                    Vector3 standardScale = new Vector3(1, 0.75f, 1);
                    grabObject.transform.localScale = standardScale * distRate;
                }
            }
            else//右手だけ中指を引いてるとき
            {
                
                if (grabObject.tag == "DrawObject")
                {
                    Vector3 move = _HandAnchor.position - lastPointerPosition;
                    move.z = 0;
                    //Debug.Log(move);
                    grabObject.transform.position += move * 6;
                    grabObject.GetComponent<NetworkDrawObject>().CmdMovePosition(move); 
                } 
            }
        }
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            isGrabbing = false;
            baseDistance = -1f;
        }
    }
}
