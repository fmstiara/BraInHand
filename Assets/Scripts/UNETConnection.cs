using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UNETConnection : NetworkManager
{
    [SerializeField] UNETDiscovery m_NetworkDiscovery;

    public void Connect()
    {
        SetIPAddress();
        SetPort();
        if (OnWindows())
        {
            Debug.Log("connect a server as a HOST");
            NetworkManager.singleton.StartHost();

            m_NetworkDiscovery.Initialize();
            m_NetworkDiscovery.StartAsServer();
        }
        else
        {
            Debug.Log("connect a server");

            m_NetworkDiscovery.Initialize();
            m_NetworkDiscovery.StartAsClient();
        }

    }

    public override void OnStopServer()
    {
        m_NetworkDiscovery.StopBroadcast();
    }

    private void SetIPAddress()
    {
        string ipAddress = "192.168.100.28";
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    private void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnPlayerConnectd");
    }

    private bool OnWindows()
    {
        if(Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            return true;
        }

        return false;
    }
}