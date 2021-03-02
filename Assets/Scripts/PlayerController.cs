using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float h;
    float v;
    public float speed = 2;
    public GameObject enemy;
    Vector3 vec;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    public void PlayerMove()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        vec = new Vector3(h, 0, v);
        transform.position += vec * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
           GameObject.Find("DroneParent").GetComponent<Patrol>().enabled = false;
            enemy.GetComponent<DroneAi>().enabled = true;
        }

    }
}

