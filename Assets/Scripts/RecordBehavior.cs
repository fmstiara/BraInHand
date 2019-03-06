using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordBehavior : MonoBehaviour
{
    private Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        this.camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
//        this.transform.LookAt(this.camera.position);
//        transform.Rotate(new Vector3(0f,-180f,0f));

    }
}
