using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Grab(Vector3 pos,Transform parent)
    {
        transform.position = pos;
        transform.parent = parent;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
