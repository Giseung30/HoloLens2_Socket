using UnityEngine;
using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace UnityCoder.RawVideoUDP
{
    public class RawVideoReceiver : MonoBehaviour
    {
        [Header("Setting")]
        public bool activeConnection; //연결을 시도하는 지 판단하는 변수
        public Material targetMaterial; //타겟 머터리얼
        public string serverIp; //서버 아이피
        public int port; //포트 번호

        [Header("Socket")]
        private const int MAX_DGRAM = 32768;
        private const int MAX_IMAGE_DGRAM = MAX_DGRAM - 64;

        private int MAX_PKT_CNT = 7;
        private int CRNT_PKT_CNT = 1;

        private Socket client;
        private IPEndPoint ipEndPoint;

        private Thread thredRecvd;
        private byte[] receivedBytes = new byte[MAX_DGRAM];

        private Texture2D tex;
        private int size = 256;

        private int imageSize = 0;

        private byte[] dump;
        private int bufferSize = 0;
        private int bufferIndex = 0;
        private int bufferFrameStart = 0;
        private byte[] temp;

        private void Start()
        {
            tex = new Texture2D(size, size, TextureFormat.RGB24, false, false);

            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            imageSize = size * size * 3;
            temp = new byte[imageSize];

            /* 픽셀 초기화 */
            for (int i = 0; i < imageSize; i += 3)
            {
                temp[i] = 255;
                temp[i + 1] = 0;
                temp[i + 2] = 255;
            }
            tex.LoadRawTextureData(temp);
            tex.Apply(false);

            bufferSize = imageSize * 100;

            dump = new byte[bufferSize];

            targetMaterial.mainTexture = tex;

            if (activeConnection) InitializeUDPClient(); //UDP 클라이언트 초기화
        }

        private Queue<int> frameIndex = new Queue<int>();
        private int frameBufferCount = 0;

        private void FixedUpdate()
        {
            if (frameBufferCount > 0)
            {
                Buffer.BlockCopy(dump, frameIndex.Dequeue(), temp, 0, imageSize);
                frameBufferCount--;
                tex.LoadRawTextureData(temp);

                /* GrayScale */
                for (int x = 0; x < tex.height; ++x)
                {
                    for (int y = 0; y < tex.width; ++y)
                    {
                        Color color = tex.GetPixel(x, y);
                        float grayScale = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
                        tex.SetPixel(x, y, new Color(grayScale, grayScale, grayScale));
                    }
                }

                tex.Apply(false);
            }
        }

        private void InitializeUDPClient()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);

            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Connect(ipEndPoint);

            byte[] strt_pkt = new byte[10];

            strt_pkt[0] = (byte)'S';

            client.Send(strt_pkt);

            /* 서버 체크 시스템 시작, 스레드를 사용하지 않으면 서버 체크 Receive 에서 유니티가 멈춰버림! */
            thredRecvd = new Thread(ReceivePacket);
            thredRecvd.Start();
        }

        private void ReceivePacket()
        {
            while (true)
            {
                try
                {
                    var len = client.Receive(receivedBytes); //서버에서 온 패킷을 버퍼에 담기

                    len -= 1;
                    int seq = receivedBytes[0];
                    bufferIndex = seq * MAX_IMAGE_DGRAM;

                    Buffer.BlockCopy(receivedBytes, 1, dump, bufferIndex, len);
                    bufferIndex += len;

                    CRNT_PKT_CNT++;

                    if (bufferIndex - bufferFrameStart >= imageSize)
                    {
                        frameIndex.Enqueue(bufferFrameStart);
                        frameBufferCount++;
                        bufferFrameStart += imageSize;

                        CRNT_PKT_CNT = 1;
                    }

                    if (CRNT_PKT_CNT >= MAX_PKT_CNT)
                    {
                        bufferIndex = 0;
                        bufferFrameStart = 0;
                        frameBufferCount = 0;
                        frameIndex.Clear();
                    }
                }
                catch (Exception) { }
            }
        }

        private void OnDestroy()
        {
            if (thredRecvd != null)
            {
                if (thredRecvd.IsAlive)
                {
                    thredRecvd.Abort();
                }
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
    }
}