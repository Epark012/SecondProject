using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class Grip : MonoBehaviour
{
    public SteamVR_Action_Boolean grip;
    public SteamVR_Input_Sources hand;
    public Grabbable grabObject;

    void Start()
    {

    }


    void Update()
    {
        if (grip.GetStateDown(hand))
        {
            Catch();
        }
    }

    private void Catch()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 10f);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                grabObject = cols[i].GetComponent<Grabbable>();
                if (grabObject != null)
                {
                    
                    grabObject.Grab(transform.position, transform);
                    break;
                }
            }
        }
    }
}
