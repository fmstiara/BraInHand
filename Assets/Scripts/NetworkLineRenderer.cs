using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLineRenderer: NetworkBehaviour {

    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] private LineCollider m_lineCollider;

    private Vector3 lastPosition;

    Color LineColor = new Color(0.2f, 0.2f, 0.2f);
    Gradient LineGradient;

    // Use this for initialization
    void Start () {
        Debug.Log("NetworkLineRenderer is created.");
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

	}

    void AddColliderToLine(Vector3 _last, Vector3 _new)
    {
        LineCollider collider = Instantiate(m_lineCollider, new Vector3(0, 0, 0), Quaternion.identity);
        collider.transform.SetParent(this.gameObject.transform);
        collider.Set(_last, _new);
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

    Gradient GetGradient(Color _c)
    {
        float alpha = 1f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(_c, 0.0f), new GradientColorKey(_c, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );

        return gradient;
    }

    [Command]
    public void CmdAddPosition(Vector3 p)
    {
        if (!isServer)
        {
            Debug.Log("isn't Server");
            return;
        }
        RpcAddPosition(p);
    }

    [ClientRpc]
    void RpcAddPosition(Vector3 p)
    {
        int NextPositionIndex = m_lineRenderer.positionCount;
        m_lineRenderer.positionCount = NextPositionIndex + 1;
        m_lineRenderer.SetPosition(NextPositionIndex, p);

        AddColliderToLine(lastPosition, p);
        lastPosition = p;
    }

    [Command]
    public void CmdSetColorMode(int m)
    {
        if (!isServer)
        {
            Debug.Log("isn't Server");
            return;
        }
        RpcSetColorMode(m);
    }

    [ClientRpc]
    void RpcSetColorMode(int m)
    {
        LineColor = GetColor(m);
        LineGradient = GetGradient(LineColor);
        m_lineRenderer.colorGradient = LineGradient;
    }
}
