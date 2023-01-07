using UnityEngine;
using System.Collections;

public class MonitorController : MonoBehaviour
{
    /* 방향 Enum */
    private enum Direction
    {
        Up, Down, Left, Right
    }

    [Header("Variable")]
    public Transform screenTransform; //화면 Transform 컴포넌트
    public float screenMoveSpeed; //화면 이동 속도
    public Vector2 clampScreenPositionX; //화면의 X 위치를 제한하는 변수
    public Vector2 clampScreenPositionY; //화면의 Y 위치를 제한하는 변수

    [Header("Cache")]
    private bool moveLeft; //왼쪽 이동
    private bool moveRight; //오른쪽 이동
    private bool moveUp; //위쪽 이동
    private bool moveDown; //아래쪽 이동

    private void Start()
    {
        StartCoroutine(MoveScreen());
    }

    /* 방향키를 클릭했을 때 실행되는 함수 */
    public void OnEnterArrowKey(int direction)
    {
        switch (direction)
        {
            case (int)Direction.Up:
                moveUp = true;
                break;
            case (int)Direction.Down:
                moveDown = true;
                break;
            case (int)Direction.Left:
                moveLeft = true;
                break;
            case (int)Direction.Right:
                moveRight = true;
                break;
        }
    }

    /* 방향키에서 벗어났을 때 실행되는 함수 */
    public void OnExitArrowKey(int direction)
    {
        switch (direction)
        {
            case (int)Direction.Up:
                moveUp = false;
                break;
            case (int)Direction.Down:
                moveDown = false;
                break;
            case (int)Direction.Left:
                moveLeft = false;
                break;
            case (int)Direction.Right:
                moveRight = false;
                break;
        }
    }

    /* 화면을 움직이는 코루틴 함수 */
    private IEnumerator MoveScreen()
    {
        while (true)
        {
            Vector3 resultPosition = screenTransform.localPosition; //로컬 위치 저장
            /* 단축키에 따라 이동 */
            if (moveLeft)
            {
                resultPosition -= new Vector3(screenMoveSpeed * Time.deltaTime, 0f, 0f);
            }
            if (moveRight)
            {
                resultPosition += new Vector3(screenMoveSpeed * Time.deltaTime, 0f, 0f);
            }
            if (moveDown)
            {
                resultPosition += new Vector3(0f, screenMoveSpeed * Time.deltaTime, 0f);
            }
            if (moveUp)
            {
                resultPosition -= new Vector3(0f, screenMoveSpeed * Time.deltaTime, 0f);
            }
            resultPosition = new Vector3(Mathf.Clamp(resultPosition.x, clampScreenPositionX.x, clampScreenPositionX.y), Mathf.Clamp(resultPosition.y, clampScreenPositionY.x, clampScreenPositionY.y), resultPosition.z); //이동 범위 제한
            screenTransform.localPosition = resultPosition; //결과 위치 적용

            yield return null;
        }
    }
}