using UnityEngine;
using TMPro;

public class NearMenu : MonoBehaviour
{
    [Header("Object Lock")]
    public bool isObjectLock; //Object 고정 여부
    public Transform objectsTransform; //Objects의 Transform 정보
    public TextMeshPro labelTMP; //Label의 TMP 정보

    [Header("Change Monitor")]
    public bool isChangedMonitor; //Monitor 변경 여부
    public GameObject _1Monitor; //1 Monitor 오브젝트
    public GameObject _6Monitor; //6 Monitor 오브젝트
    public BoxCollider monitorBoxCollider; //Monitor 오브젝트의 BoxCollider 컴포넌트
    public Transform screenTransform; //Screen 오브젝트의 Transform 컴포넌트

    /* ObjectLock 버튼을 클릭했을 때 실행되는 함수 */
    public void OnClickObjectLockButton()
    {
        isObjectLock = !isObjectLock; //Object 상태 변경

        if (isObjectLock) //고정되어 있으면
        {
            labelTMP.text = "Object Lock"; //Text 변경

            for (int i = 0; i < objectsTransform.childCount; ++i) //오브젝트를 탐색
            {
                objectsTransform.GetChild(i).GetComponent<Collider>().enabled = false; //조작 불허
            }
        }
        else //고정되어 있지 않으면
        {
            labelTMP.text = "Object Unlock"; //Text 변경

            for (int i = 0; i < objectsTransform.childCount; ++i) //오브젝트를 탐색
            {
                objectsTransform.GetChild(i).GetComponent<Collider>().enabled = true; //조작 허용
            }
        }
    }

    /* ChangeMonitor 버튼을 클릭했을 때 실행되는 함수 */
    public void OnClickChangeMonitorButton()
    {
        isChangedMonitor = !isChangedMonitor; //모니터 변경 여부

        if (isChangedMonitor) //모니터 변경
        {
            /* Collider 크기 조절 */
            monitorBoxCollider.size = new Vector3(monitorBoxCollider.size.x * 3f, monitorBoxCollider.size.y * 2f, monitorBoxCollider.size.z);

            /* Screen 위치 조절 */
            screenTransform.localPosition = new Vector3(-0.3f, -0.09f, 0f);

            /* 모니터 활성화 */
            _1Monitor.SetActive(false);
            _6Monitor.SetActive(true);
        }
        else //모니터 복구
        {
            /* 모니터 비활성화 */
            _6Monitor.SetActive(false);
            _1Monitor.SetActive(true);

            /* Screen 위치 조절 */
            screenTransform.localPosition = Vector3.zero;

            /* Collider 크기 조절 */
            monitorBoxCollider.size = new Vector3(monitorBoxCollider.size.x / 3f, monitorBoxCollider.size.y / 2f, monitorBoxCollider.size.z);
        }
    }
}