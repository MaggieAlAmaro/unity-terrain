using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;


public class CClient : MonoBehaviour
{
    public static int dataBufferSize = 4096;
    public int port = 25001;
    public string ip = "127.0.0.1";
    //public static 

    private NetworkStream stream;

    Thread thread;
    TcpListener server;
    TcpClient client;
    bool running;
    void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        //ThreadStart ts = new ThreadStart(GetData);
        //thread = new Thread(ts);
        //thread.Start();
    }



    private byte[] receiveBuffer;
    public void Connect()
    {
        client = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };
        receiveBuffer = new byte[dataBufferSize];
        client.BeginConnect(ip, port, ConnectCallback, client);

    }

    private byte[] sendBuffer;
    public void ConnectCallback(IAsyncResult result)
    {
        sendBuffer = Encoding.ASCII.GetBytes("1,2,3");
        client.EndConnect(result);
        if (!client.Connected)
        {
            Debug.Log("Couldn't Connect.");
            return;
        }

        stream = client.GetStream();
        stream.BeginWrite(sendBuffer, 0, sendBuffer.Length, ReceiveCallback, client);

        //stream.BeginRead(receiveBuffer,0,dataBufferSize,ReceiveCallback, client);
    }
    public void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLength = stream.EndRead(result);
            if (byteLength <= 0)
            {
                //disconnect and return
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(receiveBuffer, data, byteLength);

            //todo:handle data

            Debug.Log(receiveBuffer);
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, client);
        }   
        catch (Exception)
        {
            Debug.LogWarning("Server Callback ERROR");
            //TODO: Disconnect
        }
    }
    /*
    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        client = server.AcceptTcpClient();

        // Start listening
        running = true;
        while (running)
        {
            Connection();
        }
        server.Stop();
    }
    */

}
