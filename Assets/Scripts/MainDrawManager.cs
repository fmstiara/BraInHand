using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class MainControllerManager
{

    [SerializeField] private GameObject _DrawLinePrefab;
    [SerializeField] private GameObject PaletteColor;

    private GameObject CurrentLine;
    private int penMode = 0; //0=black, 1=red, 2=green, 3=blue, 4=yellow

    // Use this for initialization

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
                CurrentLine = Instantiate(_DrawLinePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                NetworkLineRenderer line = CurrentLine.GetComponent<NetworkLineRenderer>();
                NetworkServer.Spawn(CurrentLine);
                line.CmdSetColorMode(penMode);
                line.CmdAddPosition(pointer.position);
            }
            else
            {
                NetworkLineRenderer line = CurrentLine.GetComponent<NetworkLineRenderer>();
                line.CmdAddPosition(pointer.position);
            }
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            if (CurrentLine != null)
            {
                CurrentLine.transform.SetParent(CurrentDrawObject.transform);
                CurrentLine = null;
            }
        }
    }

    void OnChangeLineColor()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))
        {
            penMode++;
            if (penMode > 4)
                penMode = 0;

            PaletteColor.GetComponent<Renderer>().material.color = GetColor(penMode);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
        {
            penMode--;
            if (penMode < 0)
                penMode = 4;

            PaletteColor.GetComponent<Renderer>().material.color = GetColor(penMode);
        }
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
}
