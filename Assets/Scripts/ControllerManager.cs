using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour {

    [SerializeField] private GameObject _DrawLinePrefab;
    [SerializeField] private Transform _HandAnchor;

    GameObject CurrentLine;
    GameObject DrawLines;

    private Transform Pointer
    {
        get
        {
            return _HandAnchor;
        }
    }

	// Use this for initialization
	void Start () {
        DrawLines = new GameObject("DrawLines");
        DrawLines.transform.SetParent(null);
	}
	
	// Update is called once per frame
	void Update () {
        var pointer = Pointer;
        if(pointer == null)
        {
            Debug.Log("pointer not defiend");
            return;
        }

        if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            if(CurrentLine == null)
            {
                CurrentLine = Instantiate(_DrawLinePrefab, pointer.position, Quaternion.identity);
                LineRenderer DrawLine = CurrentLine.GetComponent<LineRenderer>();
                DrawLine.positionCount = 1;
                DrawLine.SetPosition(0, pointer.position);
            }
            else
            {
                LineRenderer DrawLine = CurrentLine.GetComponent<LineRenderer>();
                int NextPositionIndex = DrawLine.positionCount;
                DrawLine.positionCount = NextPositionIndex + 1;
                DrawLine.SetPosition(NextPositionIndex, pointer.position);
                Debug.Log(DrawLine);
            }
        }

        if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            if(CurrentLine != null)
            {
                CurrentLine.transform.SetParent(DrawLines.transform);
                Debug.Log("get up right trigger");
                CurrentLine = null;
            }
        }

	}
}
