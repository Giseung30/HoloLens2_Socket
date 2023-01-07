using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public class Server : MonoBehaviour
{
    [Header("Static")]
    public static Server server; //전역 참조 변수

    [Header("Network")]
    public bool activeConnection; //연결을 시도하는 지 판단하는 변수
    public int mPort; //접속 포트
    private Socket mServer; //서버 소켓
    private Socket mClient; //클라이언트 소켓
    private Packet mSendPacket = new Packet();
    private EndPoint mRemoteEndPoint;

    [Header("Variable")]
    public bool isHoloLens; //홀로렌즈에 패킷을 보내는 것인지 판단하는 변수

    private void Awake()
    {
        server = this; //전역 참조 변수 지정
    }

    private void Start()
    {
        if (activeConnection) //연결을 시도하면
            InitServer(); //서버 온
    }

    private void OnApplicationQuit()
    {
        CloseServer();
    }

    private void InitServer()
    {
        mServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        mRemoteEndPoint = new IPEndPoint(IPAddress.Any, mPort);
        mServer.Bind(mRemoteEndPoint);
        mServer.Listen(10); //Client 접속 대기

        if (isHoloLens) //홀로렌즈에 패킷을 보내는 것이면
        {
            mClient = mServer.Accept();
            mClient = mServer.Accept();
        }
        else //일반 전송이면
        {
            mClient = mServer.Accept();
        }
    }

    /* 패킷을 설정하는 함수 */
    private void SetSendPacket()
    {
        mSendPacket.mouseHeldDown = Input.GetMouseButton(0); //마우스 클릭 여부
        mSendPacket.clearButtonMouseDown = Drawable.drawable.clearButtonMouseDown; //Canvas 초기화 버튼 호버링 여부
        mSendPacket.mousePositionX = Drawable.drawable.mouseWorldPosition.x; //마우스 월드 위치 X
        mSendPacket.mousePositionY = Drawable.drawable.mouseWorldPosition.y; //마우스 월드 위치 Y
        mSendPacket.penColourR = Drawable.drawable.penColour.r;//펜 색상 R
        mSendPacket.penColourG = Drawable.drawable.penColour.g; //펜 색상 G
        mSendPacket.penColourB = Drawable.drawable.penColour.b; //펜 색상 B
        mSendPacket.penWidth = Drawable.drawable.penWidth; //펜 두께
    }

    public void Send()
    {
        if (mClient.Connected)
        {
            try
            {
                SetSendPacket();

                byte[] sendPacket = StructToByteArray(mSendPacket);
                mClient.Send(sendPacket, 0, sendPacket.Length, SocketFlags.None);
            }

            catch (Exception)
            {
                return;
            }
        }
    }

    private void CloseServer()
    {
        if (mClient != null)
        {
            mClient.Close();
            mClient = null;
        }

        if (mServer != null)
        {
            mServer.Close();
            mServer = null;
        }
    }

    private byte[] StructToByteArray(object obj)
    {
        int size = Marshal.SizeOf(obj);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }
}