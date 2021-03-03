using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
//우회전을 할 때 더 많은 힘을 주는 안티롤 바?는 이 힘을 받아 전달 . 아래쪽으로 힘을 가함. 상승하는 힘이 맞을 것으로 차를 안정하는데 도움을 줌 반대방향으로 타이어 힘 생성
//땅에 차가 잘 붙어있게 함.
[System.Serializable]
public struct WheelControlInfo
{
    public float MotorTorque;

    [Space(10f)]
    public float frictionStanForward;
    public float frictionStanSide;
    public float whenDrift;

    [Space(10f)]
    public float driftF_Forward;

    [Space(10f)]
    public WheelCollider[] AllWheel;
}

//오디오 제어 정보
[System.Serializable] //인스펙터에 구조체 시각화
public struct AudioControlInfo
{
    public AudioSource[] audio;
    public AudioClip[] audioClip;
}

//이펙트 제어 정보
[System.Serializable] //인스펙터에 구조체 시각화
public struct EffectControlInfo
{
    [Header("Obj")]
    public GameObject driftTrailObj;

    [Header("Mirror")]
    public GameObject[] Mirrors;
}

//음향 효과 타입
public enum SoundType
{
    EngineStart,
    EngineAccele,
    Drift
}

[RequireComponent(typeof(Rigidbody))]
public class CC : MonoBehaviour
{
    public UIManager uim;
    public List<Light> lights;
    public List<WheelCollider> throttlewheels; //뒷바퀴 (브레이크기능)
    public List<GameObject> steeringwheels; //앞바퀴 (방향키기능)
    public List<GameObject> meshes; //바퀴 휠 변환을 위한 게임오브젝트 
    public List<GameObject> taillights;
    public Transform CM; //질량중심 (엔진무게?역할- 급 회전시 뒤집어지지 않기 위해 질량을 제작)
    public Transform spawnPoint;
    public Rigidbody rb;
    public float strengthCoefficient = 10000f; //계수 강도(물질의 특정한 속성을 나타내는 수) 
    public float maxTurn = 30f; //바퀴 y축 회전 각도
    public float brakeStrength;
    public float RPMInterpolation = 1; //선형보간
    public float throttle; // 뒷바퀴
    public float steer; // 앞바퀴 
    public float s;
    //public int previousGear = 0;
    public bool l;
    public bool brake;
    public bool drift;
    public bool accel;


    [Header("Info")]
    public WheelControlInfo wheelinfo;
    public AudioControlInfo audioinfo;
    public EffectControlInfo effectinfo;

    SoundType soundSelected;
    GameObject[] driftEffectClone;

    bool isStart;
    bool isSetting;
    bool isInstantiated;

    public SteamVR_Action_Boolean grip;
    public SteamVR_Input_Sources brakeLeftTrigger;
    public SteamVR_Input_Sources brakeRightTrigger;
    public SteamVR_Input_Sources accelerator;
    void Awake()
    {
        //복제본 배열의 길이를 휠 갯수로 설정 (드리프트 시각화)
        driftEffectClone = new GameObject[wheelinfo.AllWheel.Length];
    }

    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();

       /* if (CM)
        {
            rb.centerOfMass = CM.position;
        }*/

        //시동 버튼을 눌렀으며, 초기 세팅이 되지 않았다면 실행
        if (!isSetting && Input.GetButtonDown("CarStart"))
        {
            //세팅 값을 true로 설정
            isSetting = true;

            //엔진 시동음 재생
            CarSoundEffect(audioinfo.audio[0], SoundType.EngineStart);
            //양보 반환을 통해 2초 기다림
            yield return new WaitForSeconds(2);
            //시동 값을 true로 설정
            isStart = true;
        }
    }

    void Update()
    {
        foreach (GameObject tl in taillights)
        {
            tl.GetComponent<Renderer>().material.SetColor("_EmissionColor", brake ? new Color(0.5f, 0.111f, 0.111f) : Color.black);
        }

        uim.changeText(transform.InverseTransformVector(rb.velocity).z); //Rb에 대한 레퍼런스를 얻음.변환 백터가 필요(inverse~)
        int shift_factor = Mathf.FloorToInt(Mathf.Abs(transform.InverseTransformDirection(rb.velocity).z) / 20);

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            rb.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (isStart)
        {
            //휠 제어
            WheelControl();
            //라이트 제어
            LightControl();
            //드리프트 제어
            DriftControl();
            //음향 효과 제어
            CarSoundEffect(audioinfo.audio[0], SoundType.EngineAccele);
        }
        else //시동을 걸지 않았다면 실행
        {
            //시동 코루틴 작동
            StartCoroutine(Start());

            //모든 휠 제어
            foreach (WheelCollider wheel in wheelinfo.AllWheel)
            {
                //휠 운동 시각화
                UpdateWheelVisual(wheel.transform.GetChild(0), wheel);
            }
        }
    }
    public void WheelControl()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        //brake = Input.GetKey(KeyCode.Space); 
        brake=grip.GetStateDown(brakeLeftTrigger)|| Input.GetKey(KeyCode.Space);
        foreach (WheelCollider wheel in throttlewheels)
        {
            if (brake)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = brakeStrength * Time.deltaTime;
                print("브레이크");
            }
            else
            {
                wheel.motorTorque = s * Time.deltaTime * throttle;
                wheel.brakeTorque = 0f;
            }
        }

        foreach (GameObject wheel in steeringwheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * steer; //steer휠은 y축에서 회전 왼쪽으로. 
            wheel.transform.localEulerAngles = new Vector3(0f, steer * maxTurn, 0f); //steer 입력 없으면 y축으로만 회전. 
        }

        foreach (GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.33f), 0f, 0f); //x축은 앞으로 가는 회전 방향 #3 
        }

        if (Input.GetKey(KeyCode.LeftShift)||grip.GetState(accelerator)) // accel 기능
        {
            s = strengthCoefficient * 10f;
        }
        else
        {
            s = strengthCoefficient;
        }
    }
    void UpdateWheelVisual(Transform trans, WheelCollider wheelCol)
    {
        Vector3 UpdatePos;
        Quaternion UpdateRot;

        //휠 운동 연산 결과를 월드 좌표로 변환
        wheelCol.GetWorldPose(out UpdatePos, out UpdateRot);

        trans.position = UpdatePos;
        trans.rotation = UpdateRot;
    }

    //휠 센터 좌표 설정
    /*void WheelCenterSetting()
    {
        //모든 휠 제어
        foreach (WheelCollider wheel in wheelinfo.AllWheel)
        {
            //해당 휠의 자식 오브젝트의 로컬 좌표를 센터 좌표로 설정
            wheel.center = wheel.transform.GetChild(0).transform.localPosition;
        }
    }*/
    public virtual void LightControl()
    {
        l = Input.GetKeyDown(KeyCode.L); //라이트 기능

        foreach (Light light in lights)
        {
            light.intensity = light.intensity == 0 ? 2 : 0;
        }
    }
    public void DriftControl()
    {
        //마찰력 평균
        float FrictionAverage = 0f;
        //드리프트 시, 뒤에서 밀어주는 힘
        float DriftForceF = (Input.GetAxis("Drift") * wheelinfo.driftF_Forward * Input.GetAxis("Vertical")) / rb.mass;

        //속도에 앞 방향으로 가속도의 법칙 F = ma에 따라 a = F/m 값인 가속도를 속도에 더함
        rb.velocity += transform.forward * DriftForceF;

        //모든 휠의 마찰력 제어
        foreach (WheelCollider wheel in wheelinfo.AllWheel)
        {
            //휠 마찰 커브 가져오기
            WheelFrictionCurve wheelCurveForward;
            WheelFrictionCurve wheelCurveSide;

            //휠의 전진 마찰력, 사이드 마찰력을 각각 wheelCurveForward, wheelCurveSide 마찰 커브로 치환
            wheelCurveForward = wheel.forwardFriction;
            wheelCurveSide = wheel.sidewaysFriction;

            //휠 마찰력 제어
            wheelCurveForward.extremumValue = Input.GetButton("Drift") ? Mathf.Clamp((wheelinfo.frictionStanForward / (1 + Input.GetAxis("Drift"))) / (1 + rb.velocity.magnitude), 0.35f, 3) : 2;
            wheelCurveForward.asymptoteValue = Input.GetButton("Drift") ? Mathf.Clamp((wheelinfo.frictionStanForward / (1 + Input.GetAxis("Drift"))) / (1 + rb.velocity.magnitude), 0.35f, 3) : 2;
            wheelCurveSide.extremumValue = Input.GetButton("Drift") ? Mathf.Clamp((wheelinfo.frictionStanSide / (1 + Input.GetAxis("Drift") * (1 + Input.GetAxis("Horizontal")))) / (1 + rb.velocity.magnitude), 0.4f, 3) : 1;
            wheelCurveSide.asymptoteValue = Input.GetButton("Drift") ? Mathf.Clamp((wheelinfo.frictionStanSide / (1 + Input.GetAxis("Drift") * (1 + Input.GetAxis("Horizontal")))) / (1 + rb.velocity.magnitude), 0.4f, 3) : 1;

            //휠의 전진 마찰력, 사이드 마찰력을 각각 마찰 커브 변수에 대입
            wheel.forwardFriction = wheelCurveForward;
            wheel.sidewaysFriction = wheelCurveSide;

            //전진 마찰력, 사이드 마찰력의 평균값
            FrictionAverage += (wheelCurveForward.extremumValue + wheelCurveSide.extremumValue) / 2;
        }

        //모든 휠의 마찰력 평균값
        FrictionAverage = FrictionAverage / wheelinfo.AllWheel.Length;

        //평균 마찰력이 일정 수치 미만이라면 드리프트 음향 효과 실행
        if (FrictionAverage < wheelinfo.whenDrift)
        {
            CarSoundEffect(audioinfo.audio[1], SoundType.Drift); //'음향 및 시각 효과 구현' 파트에서 설명할 예정
        }

        //드리프트 시각 효과 실행
        CarDrivingEffect(FrictionAverage); //'음향 및 시각 효과 구현' 파트에서 설명할 예정
    }
    void CarSoundEffect(AudioSource audio, SoundType type)
    {
        //오디오 클립을 사운드 타입을 정수형 변수로 변환한 값의 인덱스로 설정
        audio.clip = audioinfo.audioClip[(int)type];

        //타입을 switch문으로 돌림
        switch (type)
        {
            //시동 거는 효과음
            case SoundType.EngineStart:

                //볼륨 1로 설정
                audio.volume = 1;
                //루프 값을 false로 설정 (무한 재생이 아니기 때문)
                audio.loop = false;

                //오디오 재생
                audio.Play();

                break;

            //악셀 효과음
            case SoundType.EngineAccele:

                //볼륨 1로 설정
                audio.volume = 1;
                //루프 값을 true로 설정 (무한 재생)
                audio.loop = true;

                //현재 재생 중인 효과음이 자신이 아니라면 오디오 재생 (자신인 경우 재생하게 되면 오디오 재생이 끊김)
                if (soundSelected != SoundType.EngineAccele)
                    audio.Play();

                //오디오 빠르기를 토크에 비례하도록 설정
                audio.pitch = Mathf.Clamp(0.5f + wheelinfo.MotorTorque * Input.GetAxis("Vertical") * 0.01f, 0, 1.8f);

                break;

            //드리프트 효과음
            case SoundType.Drift:

                //오디오 볼륨 1로 설정
                audio.volume = 1;
                //루프 값을 false로 설정 (무한 재생이 아니기 때문)
                audio.loop = false;

                //재생 중이지 않다면 오디오 재생
                if (!audio.isPlaying)
                    audio.Play();

                break;
        }

        //현재 재생 중인 효과음 타입을 인수 type 값으로 설정
        soundSelected = type;
    }
    //드리프트 시각 효과 제어
    void CarDrivingEffect(float friction)
    {
        //트레일 렌더러 배열의 길이를 모든 휠의 갯수로 설정
        TrailRenderer[] trails = new TrailRenderer[wheelinfo.AllWheel.Length];

        //프리팹이 복제되지 않았다면 실행
        if (!isInstantiated)
        {
            //모든 휠에 반영
            for (int i = 0; i < wheelinfo.AllWheel.Length; i++)
            {
                /*드리프트 트레일 역할을 하는 오브젝트를 해당 휠의 좌표에 벡터 (0, -0.25, 0)을 더하고,
                각도는 오일러 각으로 x축 90도로 설정*/
                driftEffectClone[i] = Instantiate(effectinfo.driftTrailObj, wheelinfo.AllWheel[i].transform.position + new Vector3(0, -0.25f, 0), Quaternion.Euler(90, 0, 0));
            }

            //프리팹이 복제되었으므로, true로 설정
            isInstantiated = true;
        }

        //모든 휠에 반영
        for (int i = 0; i < wheelinfo.AllWheel.Length; i++)
        {
            //트레일 렌더러 배열의 해당 인덱스를 복제본의 컴포넌트로 설정
            trails[i] = driftEffectClone[i].GetComponent<TrailRenderer>();
            //마찰력 평균이 일정 수치보다 작고, 휠이 지면에 닿았을 때, 트레일 방출 (추가 렌더링)
            trails[i].emitting = friction < wheelinfo.whenDrift && wheelinfo.AllWheel[i].isGrounded ? true : false;

            //복제본의 좌표를 해당 휠의 자식 오브젝트의 좌표에 벡터 (0, -0.25, 0)를 더한 값으로 설정
            driftEffectClone[i].transform.position = wheelinfo.AllWheel[i].transform.GetChild(0).transform.position + new Vector3(0, -0.25f, 0);
        }
    }
}
