using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerSetup : NetworkBehaviour
{
    [SerializeField] OVRCameraRig m_OVRCameraRig;
    [SerializeField] GameObject GoAssets;
    [SerializeField] GameObject RiftAssets;

    void Start()
    {
        //自分が操作するオブジェクトに限定する
    }

    public override void OnStartLocalPlayer()
    {
        //Debug.Log("Player Network Setup");
        if (isLocalPlayer)
        {
            //FPSCharacterCamを使うため、Scene Cameraを非アクティブ化
            GameObject.Find("Main-Camera").SetActive(false);
            //GetComponent<OvrAvatar>().enabled = true;
            //GetComponent<MainControllerManager>().enabled = true;
            //m_OVRCameraRig.gameObject.SetActive(true);
            
            if (isServer)
            {
                Debug.Log("Player set up on Server");
                ActivateRiftAssets();
            }
            else if (isClient)
            {
                Debug.Log("Player set up as Client");
                ActivateGoAssets();
            }
        }
    }

    private void ActivateGoAssets()
    {
        GoAssets.SetActive(true);
    }

    private void ActivateRiftAssets()
    {
        RiftAssets.SetActive(true);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

}