using UnityEngine;
using System.Collections;

public class Element : MonoBehaviour
{
    [Header("Component")]
    private Transform elementTransform; //원소의 Transform 컴포넌트
    private Rigidbody elementRigidbody; //원소의 Rigidbody 컴포넌트
    private LineRenderer elementLineRenderer; //원소의 LineRenderer 컴포넌트

    [Header("Connected")]
    private Transform connectedElementTransform; //연결된 원소의 Transform 컴포넌트
    private Rigidbody connectedElementRigidbody; //연결된 원소의 Rigidbody 컴포넌트
    private Vector3 connectedDirection; //연결된 방향

    /* 초기화 함수 */
    public void SetUp(Transform connectedElementTransform)
    {
        elementTransform = GetComponent<Transform>(); //원소의 Transform 컴포넌트 초기화

        elementRigidbody = gameObject.AddComponent<Rigidbody>(); //원소의 Rigidbody 컴포넌트 초기화
        elementRigidbody.useGravity = false; //중력 설정
        elementRigidbody.drag = Mathf.Infinity; //저항 설정
        elementRigidbody.angularDrag = Mathf.Infinity; //공기 저항 설정

        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>(); //원소에 SphereCollider 컴포넌트 추가
        sphereCollider.radius = Wire.wire.sphereColliderRadius; //반지름 설정

        if (connectedElementTransform == null) //연결된 원소가 없으면
        {
            elementRigidbody.isKinematic = true; //강체 설정
            return; //함수 종료
        }

        elementLineRenderer = gameObject.AddComponent<LineRenderer>(); //원소의 LineRenderer 컴포넌트 초기화
        elementLineRenderer.material = new Material(Shader.Find("Diffuse")); //재질 지정
        elementLineRenderer.material.color = Wire.wire.lineRendererColor; //색상 지정
        elementLineRenderer.startWidth = Wire.wire.lineRendererWidth; //처음 두께 지정
        elementLineRenderer.endWidth = Wire.wire.lineRendererWidth; //끝 두께 지정

        this.connectedElementTransform = connectedElementTransform; //연결된 원소의 Transform 컴포넌트 초기화
        connectedElementRigidbody = connectedElementTransform.GetComponent<Rigidbody>(); //연결된 원소의 Rigidbody 컴포넌트 초기화

        connectedDirection = connectedElementTransform.InverseTransformDirection(elementRigidbody.position - connectedElementRigidbody.position); //연결된 원소와의 거리 초기화

        StartCoroutine(ApplyElasticity());
        StartCoroutine(DrawLine());
    }

    /* 탄성력을 적용하는 코루틴 함수 */
    private IEnumerator ApplyElasticity()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            Vector3 destination = connectedElementRigidbody.position + connectedElementTransform.TransformDirection(connectedDirection); //목표 지점
            Vector3 resultPosition = connectedElementRigidbody.position + Vector3.Normalize(elementRigidbody.position - connectedElementRigidbody.position) * connectedDirection.magnitude; //결과 위치
            Vector3 normalVector = Vector3.Cross(resultPosition - connectedElementRigidbody.position, destination - connectedElementRigidbody.position); //법선 벡터

            float originalAngle = Vector3.Angle(destination - connectedElementRigidbody.position, resultPosition - connectedElementRigidbody.position); //본래 각도 저장

            resultPosition = RotatePointAroundPivot(resultPosition, connectedElementRigidbody.position, normalVector, originalAngle * Wire.wire.elasticity * Time.fixedDeltaTime); //중점을 따라 회전

            Vector3 direction = resultPosition - elementRigidbody.position; //방향 벡터
            if (Physics.Raycast(elementRigidbody.position, direction, out RaycastHit hit, direction.magnitude, ~Wire.wire.elementLayerMask)) //충돌하는 물체가 있으면
            {
                elementRigidbody.MovePosition(hit.point + Vector3.Reflect(hit.point - elementRigidbody.position, hit.normal) * Wire.wire.sphereColliderRadius); //닿은 지점으로 이동
            }
            else
            {
                elementRigidbody.MovePosition(resultPosition); //탄성력 적용
            }

            Vector3 lookRotation = Quaternion.LookRotation(elementRigidbody.position - connectedElementRigidbody.position).eulerAngles; //방향 벡터 각도 저장
            elementRigidbody.rotation = Quaternion.Euler(lookRotation.x, lookRotation.y, connectedElementRigidbody.rotation.eulerAngles.z); //각 위치에 맞게 회전

            yield return waitForFixedUpdate;
        }
    }

    /* 피봇을 중심으로 회전한 좌표를 반환하는 함수 */
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 center, Vector3 pivot, float angle)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, pivot);
        Vector3 dir = point - center;
        dir = rot * dir;
        return (center + dir);
    }

    /* 선을 그리는 코루틴 함수 */
    private IEnumerator DrawLine()
    {
        while (true)
        {
            elementLineRenderer.SetPosition(0, elementTransform.position); //시작 지점 설정
            elementLineRenderer.SetPosition(1, connectedElementTransform.position); //끝 지점 설정

            yield return null;
        }
    }
}