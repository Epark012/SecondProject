using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Patrol : MonoBehaviour
{
    public Transform[] points;
    private Transform trn;

    public int nextIdx = 1;
    public float speed = 3.0f;
    public float damping = 5.0f;

    NavMeshAgent agent;
    void Start()
    {
        trn = GetComponent<Transform>();
        points = GameObject.Find("Path").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = Quaternion.LookRotation(points[nextIdx].position - trn.position);
        trn.rotation = Quaternion.Slerp(trn.rotation, rot, Time.deltaTime * damping);
        trn.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag=="Way_Point")
        {
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }
}
