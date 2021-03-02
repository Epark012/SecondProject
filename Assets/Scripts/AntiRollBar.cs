using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//속도가 올라가면 회전을 할 때 차가 뒤짚어짐. 보완 하기 위한 스크립트.
public class AntiRollBar : MonoBehaviour
{
    public WheelCollider WheelL; // 뒷바퀴 왼쪽
    public WheelCollider WheelR; // 뒷바퀴 오른쪽
    private Rigidbody carRigidbody;

    public float AntiRoll = 5000.0f;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        WheelHit hit = new WheelHit(); //WheelHit 기능 - 휠이 충돌 할 때 마다 그 정보를 휠 히트 오브젝트의 형태로 출력 가능하게 해줌.
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);

        if(groundedL)
        {
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
        }

        bool groundedR = WheelR.GetGroundHit(out hit);

        if (groundedR)
        {
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
        }

        var antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            carRigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                WheelL.transform.position);
        if (groundedR)
            carRigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                WheelR.transform.position);

    }
}
