using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketDirection : MonoBehaviour
{
    LineRenderer line;
    // 내 위치를 플레이어의 진행방향 앞쪽으로 위치시킨다.
    // x값은 랜덤값
    GameObject target;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        target = GameObject.Find("RightTarget");
        direction();
    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime*15;
    }
    public void direction()
    {
        float width = target.transform.localScale.x;
        float height = target.transform.localScale.y;
        float xmin = target.transform.position.x - width / 2;
        float xmax = target.transform.position.x + width / 2;
        float ymin = target.transform.position.y - height / 2;
        float ymax = target.transform.position.y + height / 2;

        float x = Random.Range(xmin, xmax);
        float y = Random.Range(ymin, ymax);
        float z = target.transform.position.z;

       Vector3 origin = new Vector3(x, y, z);
        transform.LookAt(origin);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, origin);
    }
}
