using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    Vector3 fireDirection;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += fireDirection * speed * Time.deltaTime;
    }

    internal void GoToTarget(Vector3 dir)
    {
        fireDirection = dir;
    }
}