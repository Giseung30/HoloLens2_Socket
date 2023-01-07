using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public class Client : MonoBehaviour
{
    [Header("Static")]
    public static Client client; //전역 참조 변수

    [Header("Network")]
    public bool activeConnection; //연결을 시도하는 지 판단하는 변수
    private Socket mClient;
    public string mIp; //접속 IP
    public int mPort; //접속 포트
    [HideInInspector] public Packet mReceivePacket = new Packet();
    private IPEndPoint mServerIpEndPoint;

    private void Awake()
    {
        client = this;
    }

    private void Start()
    {
        if (activeConnection) //연결을 시도하면
            InitClient(); //클라이언트 연결
    }

    private void Update()
    {
        if (activeConnection) //연결을 시도하면
            Receive(); //패킷 수신
    }

    private void OnApplicationQuit()
    {
        CloseClient();
    }

    private void InitClient()
    {
        mServerIpEndPoint = new IPEndPoint(IPAddress.Parse(mIp), mPort);
        mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        mClient.Connect(mServerIpEndPoint);
    }

    private void Receive()
    {
        int receive = 0;
        if (mClient.Available != 0)
        {
            byte[] packet = new byte[1024];

            try
            {
                receive = mClient.Receive(packet);
            }

            catch (Exception)
            {
                return;
            }

            mReceivePacket = ByteArrayToStruct<Packet>(packet); //패킷 저장
        }
    }

    private void CloseClient()
    {
        if (mClient != null)
        {
            mClient.Close();
            mClient = null;
        }
    }

    private T ByteArrayToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length)
        {
            throw new Exception();
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
}