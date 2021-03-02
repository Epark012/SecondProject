using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JH : MonoBehaviour
{
    [SerializeField]
    private string hellowWorld;
    // Start is called before the first frame update
    void Start()
    {
        hellowWorld = "Hellow World";
        print(hellowWorld);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
