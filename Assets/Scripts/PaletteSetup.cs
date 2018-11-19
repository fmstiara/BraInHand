using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PaletteSetup : NetworkBehaviour {
    [SerializeField] Transform _HandAnchor;

	// Use this for initialization
	void Start () {
        this.gameObject.transform.SetParent(_HandAnchor);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
