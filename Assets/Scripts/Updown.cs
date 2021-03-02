using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updown : MonoBehaviour
{
    float dir = 2;
    void Start()
    {
        StartCoroutine(ChangeDir());
    }

    void Update()
    {
        transform.localPosition += transform.up * Time.deltaTime * dir ;
        transform.localPosition += transform.right * Time.deltaTime * dir;
    }

    IEnumerator ChangeDir()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            dir *= -1;
        }
    }
}
