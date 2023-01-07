using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct Packet
{
    [MarshalAs(UnmanagedType.Bool, SizeConst = 2)]
    public bool mouseHeldDown; //마우스 입력 여부
    public bool clearButtonMouseDown; //Canvas 초기화 버튼 호버링 여부

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public float mousePositionX; //마우스 X 위치
    public float mousePositionY; //마우스 Y 위치
    public float penColourR; //펜 색상 R
    public float penColourG; //펜 색상 G
    public float penColourB; //펜 색상 B
    public int penWidth; //펜 두께
}