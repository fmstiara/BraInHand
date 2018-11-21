using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerSetup : NetworkBehaviour
{
    [SerializeField] OVRCameraRig m_OVRCameraRig;

    void Start()
    {
        //自分が操作するオブジェクトに限定する
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Player Network Setup");
        if (isLocalPlayer)
        {
            //FPSCharacterCamを使うため、Scene Cameraを非アクティブ化
            GameObject.Find("Main-Camera").SetActive(false);
            //GetComponent<OvrAvatar>().enabled = true;
            //GetComponent<MainControllerManager>().enabled = true;
            //m_OVRCameraRig.gameObject.SetActive(true);
            
            if (isServer)
            {
                NonActiveGoAssets();
            }
            else if (isClient)
            {
                NonActiveRiftAssets();
            }
        }
    }

    private void NonActiveGoAssets()
    {
        GameObject.Find("GoAssets").SetActive(false);
        GameObject.Find("GoPlayerAssets").SetActive(false);
    }

    private void NonActiveRiftAssets()
    {
        GameObject.Find("RiftAssets").SetActive(false);
        GameObject.Find("RiftPlayerAssets").SetActive(false);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

}