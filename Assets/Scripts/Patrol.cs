using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Patrol : MonoBehaviour
{
    //public Transform[] points;
    //private Transform trn;

    //public int nextIdx = 1;
    //public float speed = 3.0f;
   // public float damping = 5.0f;
    float timer = 0;
    int count = 1;

    public GameObject player;
    public GameObject look;

    NavMeshAgent agent;
    Vector3 randDir;

    private Transform target;
    public enum State
    {
        Patrol,
        Chase,
    }
    public State state;
    void Start()
    {
        //trn = GetComponent<Transform>();
        //points = GameObject.Find("Path").GetComponentsInChildren<Transform>();

        target = GameObject.Find("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        state = State.Patrol;
        StartCoroutine(ChangeDir());
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Chase:
                UpdateChase();
                break;
        }
    }

    private void UpdateChase()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + randDir, Time.deltaTime * 3);
        lookAt();

        if (count == 1)
        {
            GameObject wing = Instantiate(look);
            wing.transform.position = transform.position;
            wing.transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 3);
            transform.LookAt(wing.transform.position);
            count++;
        }
    }

    private void UpdatePatrol()
    {
        //Quaternion rot = Quaternion.LookRotation(points[nextIdx].position - trn.position);
        //trn.rotation = Quaternion.Slerp(trn.rotation, rot, Time.deltaTime * damping);
        //trn.Translate(Vector3.forward * Time.deltaTime * speed);
        DistTarget();
    }

    private void OnTriggerEnter(Collider coll)
    {
        //if (coll.tag == "Way_Point")
        //{
        //    nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        //}
    }
    private void DistTarget()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < 20)
        {
            state = State.Chase;
        }
    }
    public void lookAt()
    {
        timer += Time.deltaTime;
        if (timer <= 2f)
        {
            print(timer);
            GameObject.Find("DroneMissile").GetComponent<DroneMissile>().enabled = true;

        }
    }
    IEnumerator ChangeDir()
    {
        while (true)
        {
            randDir = Random.insideUnitSphere;
            float rand = Random.Range(1, 3);
            yield return new WaitForSeconds(rand);
        }
    }
}
