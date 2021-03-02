using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRotate : MonoBehaviour
{
    // z축 값이 변경이 된다면 
    // GameObject.Find("FrontWheel").transform.rotation.y 값을 변경시켜라
    
    Transform FW; // 앞바퀴y값
    void Start()
    {
        FW = GameObject.Find("FrontWheel").GetComponent<Transform>();
    }

    void Update()
    {
        // 내 z축의 값을 FrontWheel의 y축의 값으로 지정한다.
        Vector3 angle = FW.eulerAngles; 
        angle.y = -transform.eulerAngles.z * 0.5f; 
        FW.eulerAngles = angle;
    }
}
