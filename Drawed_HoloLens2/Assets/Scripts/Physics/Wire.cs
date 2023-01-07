using UnityEngine;

public class Wire : MonoBehaviour
{
    [Header("Static")]
    public static Wire wire; //전역 참조 변수

    [Header("Component")]
    public Transform wireTransform; //Wire의 Transform 컴포넌트

    [Header("SetUp")]
    public float sphereColliderRadius; //SphereCollider의 반지름
    public float lineRendererWidth; //LineRenderer의 두께
    public Color32 lineRendererColor; //LineRenderer의 색상

    [Header("Variable")]
    public float elasticity; //탄성력
    public LayerMask elementLayerMask; //원소의 LayerMask

    private void Awake()
    {
        wire = this; //전역 참조 변수 초기화
    }

    private void Start()
    {
        for (int i = 0; i < wireTransform.childCount; ++i) //자식 개수 만큼 반복
        {
            Element element = wireTransform.GetChild(i).gameObject.AddComponent<Element>();
            element.SetUp(i == 0 ? null : wireTransform.GetChild(i - 1));
        }
    }
}