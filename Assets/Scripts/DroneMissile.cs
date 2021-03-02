using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DroneMissile : MonoBehaviour
{
    // 일정시간이 되면 미사일을 내 위치에 생성하고 
    // 미사일이 발사되기전 UI이미지를 띄우고
    // 목표지점에 궤적을 생성하고
    // 미사일을 플레이어의 진행 방향에 랜덤하게 발사하고싶다

    // - 기준시간
    // - 미사일공장
    // - UI
    // - 궤적생성
    // - 미사일방향
    float timer = 0;
    public GameObject RocketFactory;
    
    void Start()
    {
        GetComponent<DroneMissile>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        shot();
    }

    public void shot()
    {
        timer += Time.deltaTime;
        if (timer>=2)
        {
            GameObject rocket = Instantiate(RocketFactory);
            rocket.transform.position = 
                GameObject.Find("Drone").GetComponent<Transform>().position+ transform.forward;
            timer = 0;
        }
    }
}
