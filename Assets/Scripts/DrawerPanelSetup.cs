using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerPanelSetup : MonoBehaviour
{
    [SerializeField] Transform _baseAnchor;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.SetParent(_baseAnchor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
