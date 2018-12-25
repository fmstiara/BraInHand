using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Set(Vector3 _last, Vector3 _new)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        float lineLength = Vector3.Distance(_new, _last);
        collider.size = new Vector3(0.05f, 0.05f, lineLength);

        Vector3 middlePoint = (_new + _last) / 2;
        collider.transform.position = middlePoint;

        collider.transform.LookAt(_last);
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("on collision enter");
    }
}
