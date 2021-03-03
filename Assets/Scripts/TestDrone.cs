using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 길 정보를 이용해서 순서대로 목적지를 순회하고싶다.
// 태어날 때 최초 목적지를 결정하고 agent를 이용해서 알려주고싶다.
public class TestDrone: MonoBehaviour
{
    // - 순서 
    int wayIndex;
    int count = 1;

    // - agent
    NavMeshAgent nav;
    GameObject player;
    public GameObject target;
    public GameObject MakeDir;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        nav = GetComponent<NavMeshAgent>();
        wayIndex = Random.Range(0, PathManager.instance.trn.Length);
        Transform target = PathManager.instance.trn[wayIndex];
        // 태어날 때 최초 목적지를 결정하고 agent를 이용해서 알려주고싶다.
        //nav.destination = target.position;
        nav.stoppingDistance = 3;

        // 태어날 때 추적 상태로 하고싶다.
        state = State.Patrol;
    }
    public enum State
    {
        Patrol, // 순찰
        Chase, // 추적
    }
    public State state;
    private void Update()
    {
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                CheckPlayer();
                break;
            case State.Chase:
                UpdateChase();
                CheckLostPlayer();
                break;
        }
    }

    // - 인식범위 
    public float 인식범위 = 5;
    public float 추적범위 = 10;
    private void CheckLostPlayer()
    {
        // 만약 플레이어가 범위에서 벗어났다면 
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > 추적범위)
        {
            // 순찰상태로 전이하고싶다. 목적지를 순찰지로 변경하고싶다.
            state = State.Patrol;
            nav.destination = PathManager.instance.trn[wayIndex].position;
        }
    }
    GameObject DroneClone;
    private void UpdateChase()
    {
        // 목적지를 향해 계속 이동하고싶다.
        //nav.destination = player.transform.position;
        transform.position = Vector3.Lerp(transform.position, target.transform.position, 0.005f);
        if (count == 1)
        {
            GameObject DroneDir = Instantiate(MakeDir);
            DroneDir.transform.position = transform.position;
            Transform PlayerTr = player.GetComponent<Transform>();
            DroneDir.transform.position = Vector3.Lerp(DroneDir.transform.position, player.transform.position, 0.005f);
            DroneClone = DroneDir;

            count++;
        }
        DroneClone.transform.position = player.transform.position;
        transform.LookAt(DroneClone.transform.position);
    }

    private void CheckPlayer()
    {
        // 순찰중에 플레이어가 내 범위에 들어왔다면 
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= 인식범위)
        {
            // 추적상태로 변경하고 목적지를 플레이어로 하고싶다.
            state = State.Chase;
            // nav.destination = player.transform.position;
        }
    }
    // Update is called once per frame
    void UpdatePatrol()
    {
        // 살아가면서 길 정보를 이용해서 순서대로 목적지를 순회하고싶다.
        // 1. 만약 목적지와의 거리가 stoppingDistance이하라면 (도착이라면)
        float distance = Vector3.Distance(transform.position, nav.destination);
        print("1");
        if (distance <= nav.stoppingDistance)
        {
            print("2");

            // 2. 목적지를 다음 인덱스번호로 변경하고싶다.
            wayIndex++;
            if (wayIndex >= PathManager.instance.trn.Length)
            {
                print("3");

                wayIndex = 0;
            }
            nav.destination = PathManager.instance.trn[wayIndex].position;
        }
        // 3. agent에게 목적지를 알려주고싶다.
    }
}