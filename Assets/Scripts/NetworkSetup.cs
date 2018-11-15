using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSetup : MonoBehaviour
{
    UNETConnection UNET;

    [SerializeField] VRInteractiveItem HostButton;
    [SerializeField] NetworkManager Manager;

    void Start()
    {
        UNET = Manager.GetComponent<UNETConnection>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        HostButton.OnClick += HandleClick;
    }

    private void HandleClick()
    {
        Debug.Log("HandleClick to join room.");
        UNET.Connect();
    }
}