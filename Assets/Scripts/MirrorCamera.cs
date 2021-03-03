using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    //미러 카메라
    Camera mirrorCamera;

    void Awake()
    {
        //카메라 컴포넌트 가져오기
        mirrorCamera = GetComponent<Camera>();
    }

    //컬링 진행 전 호출
    void OnPreCull()
    {
        //미러 카메라 투영 행렬 초기화
        mirrorCamera.ResetProjectionMatrix();

        //카메라 반전 행렬를 미러 카메라 투영 행렬로 설정
        Matrix4x4 cameraMatrix = mirrorCamera.projectionMatrix;
        //카메라 반전 행렬에 벡터 (-1, 1, 1)을 스케일링한 대각 행렬을 곱함
        cameraMatrix *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        //미러 카메라 투영 행렬을 카메라 반전 행렬로 설정
        mirrorCamera.projectionMatrix = cameraMatrix;
    }

    //렌더링 전 호출
    void OnPreRender()
    {
        //후면 컬링 반전 bool 타입 값을 true로 설정 (후면 컬링 반전 O)
        GL.invertCulling = true;
    }

    //렌더링 후 호출
    void OnPostRender()
    {
        //후면 컬링 반전 bool 타입 값을 false로 설정 (후면 컬링 반전 X)
        GL.invertCulling = false;
    }
}
