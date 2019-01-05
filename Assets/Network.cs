using System;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class Network : MonoBehaviour {

    public static Network instance;

    [Header("Network Settings")]
    public string IP = "127.0.0.1";
    public int Port = 5500;
    public bool isConnected;

    public TcpClient PlayerSocket;
    public NetworkStream myStream;
    public StreamReader myReader;
    public StreamWriter myWriter;
    

    private byte[] asyncBuff;
    public bool shouldHandleData;

    public void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        
        ConnectToGameServer();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ConnectToGameServer() {
        if(PlayerSocket != null) {
            if(PlayerSocket.Connected || isConnected) return;
            PlayerSocket.Close();
            PlayerSocket = null;
        }

        PlayerSocket = new TcpClient();
        PlayerSocket.ReceiveBufferSize = 4096;
        PlayerSocket.SendBufferSize = 4096;
        PlayerSocket.NoDelay = false;
        Array.Resize(ref asyncBuff, 8192);
        PlayerSocket.BeginConnect(IP, Port, new AsyncCallback(ConnectCallback), PlayerSocket);
        isConnected = true;
    }

    public void Login(string username) {
        if(PlayerSocket == null || !PlayerSocket.Connected) {
            PlayerSocket.Close();
            PlayerSocket = null;
            Debug.Log("Disconnected");
            return;
        }
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteInt(1);
        buffer.WriteString(username);
        myStream.BeginWrite(buffer.BuffToArray(), 0, buffer.Length(), new AsyncCallback(LoginResponse), myStream);
    }

    void LoginResponse(IAsyncResult result) {

        Debug.Log("response on login: " + result.IsCompleted);
    }


    void ConnectCallback(IAsyncResult result) {
        if(PlayerSocket != null) {
            PlayerSocket.EndConnect(result);
            if(!PlayerSocket.Connected) {
                isConnected = false;
                return;
            } else {
                PlayerSocket.NoDelay = true;
                myStream = PlayerSocket.GetStream();
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
            }
        }
    }

    void OnReceive(IAsyncResult result) {
        if(PlayerSocket != null) {
            if (PlayerSocket == null) return;

            int byteArray = myStream.EndRead(result);
            byte[] myBytes = null;
            Array.Resize(ref myBytes, byteArray);
            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteArray);

            if(byteArray == 0) {
                Debug.Log("You got disconnected");
                PlayerSocket.Close();
                return;
            }

            //Handle Data

            if (PlayerSocket == null) return;
            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);

        }
    }
}
