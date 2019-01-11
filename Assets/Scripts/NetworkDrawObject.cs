using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDrawObject : NetworkBehaviour
{
    [SerializeField] private Transform m_transform;
    private GameObject DrawObjects;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Network object spawned");
        DrawObjects = GameObject.Find("DrawObjects");
        this.name = "obj" + DrawObjects.transform.childCount.ToString();
        m_transform.SetParent(DrawObjects.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(this.GetComponent<Transform>().position);
    }

    [Command]
    public void CmdMovePosition(Vector3 m)
    {
        if (!isServer)
        {
            Debug.Log("isn't Server");
            return;
        }
        RpcMovePosition(m);
    }


    [ClientRpc]
    private void RpcMovePosition(Vector3 m)
    {
        Debug.Log("move");
        m_transform.position += m * 4;
    }

    [Command]
    public void CmdSetChild(GameObject obj)
    {
        if (!isServer)
        {
            Debug.Log("isn't Server");
            return;
        }
        RpcSetChild(obj);
    }

    [ClientRpc]
    private void RpcSetChild(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
    }

}
