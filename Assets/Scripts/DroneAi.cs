using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DroneAi : MonoBehaviour
{
    public Transform target;
    public GameObject player;
    public GameObject look;

    float timer = 0;

    Vector3 randDir;
    int count = 1;
    void Start()
    {
        StartCoroutine(ChangeDir());
    }

    void Update()
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
