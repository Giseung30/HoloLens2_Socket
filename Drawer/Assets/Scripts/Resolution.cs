using UnityEngine;

public class Resolution : MonoBehaviour
{
    private void Start()
    {
        SetResolution(1600);
    }

    /* 해상도 설정 함수 */
    private void SetResolution(int setWidth)
    {
        Screen.SetResolution(setWidth, (int)(((float)Screen.height / Screen.width) * setWidth), true); // SetResolution 함수 제대로 사용하기
    }
}