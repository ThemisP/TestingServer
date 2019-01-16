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

    public PlayerInfo player;
 
    private byte[] asyncBuff;
    public bool shouldHandleData;

    public void Awake() {
        instance = this;
        player = new PlayerInfo();
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

    void ConnectCallback(IAsyncResult result) {
        if (PlayerSocket != null) {
            PlayerSocket.EndConnect(result);
            if (!PlayerSocket.Connected) {
                isConnected = false;
                return;
            } else {
                PlayerSocket.NoDelay = true;
                myStream = PlayerSocket.GetStream();
                myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);
            }
        }
    }


    #region "Server Communication"

    void OnReceive(IAsyncResult result) {
        if (PlayerSocket != null) {
            if (PlayerSocket == null) return;

            int byteArray = myStream.EndRead(result);
            byte[] myBytes = null;
            Array.Resize(ref myBytes, byteArray);
            Buffer.BlockCopy(asyncBuff, 0, myBytes, 0, byteArray);

            if (byteArray == 0) {
                Debug.Log("You got disconnected");
                PlayerSocket.Close();
                return;
            }

            //Handle Data
            ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
            buffer.WriteBytes(myBytes);
            string msg = buffer.ReadString();
            Debug.Log(msg);

            if (PlayerSocket == null) return;
            myStream.BeginRead(asyncBuff, 0, 8192, OnReceive, null);

        }
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
        player.ChangeUsername(username);
        myStream.Write(buffer.BuffToArray(), 0, buffer.Length());
    }

    public void CreateGame(int MaxPlayers) {
        if (PlayerSocket == null || !PlayerSocket.Connected) {
            PlayerSocket.Close();
            PlayerSocket = null;
            Debug.Log("Disconnected");
            return;
        }
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteInt(2);
        buffer.WriteInt(MaxPlayers);
        myStream.Write(buffer.BuffToArray(), 0, buffer.Length());
        byte[] bufferBytes = new byte[8];
        myStream.Read(bufferBytes, 0, 8);
        buffer.Clear();
        buffer.WriteBytes(bufferBytes);
        int finished = buffer.ReadInt();
        int roomIndex = buffer.ReadInt();
        if(finished==1) {
            Debug.Log("Succeded with roomIndex: " + roomIndex);
            Network.instance.player.JoinRoom(roomIndex);
        } else {
            Debug.Log("Failed to create a room!");
        }
        
    }

    public void GetPlayersInRoom() {
        if (PlayerSocket == null || !PlayerSocket.Connected) {
            PlayerSocket.Close();
            PlayerSocket = null;
            Debug.Log("Disconnected");
            return;
        }
        if (!player.InRoom()) return;
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteInt(5);
        buffer.WriteInt(player.GetRoomIndex());
        myStream.Write(buffer.BuffToArray(), 0, buffer.Length());
        Debug.Log("1st part");
        byte[] bufferBytes = new byte[4];
        myStream.Read(bufferBytes, 0, 4);
        buffer.Clear();
        buffer.WriteBytes(bufferBytes);
        int lengthofData = buffer.ReadInt();
        Debug.Log("2nd part");
        bufferBytes = new byte[lengthofData];
        myStream.Read(bufferBytes, 0, lengthofData);
        buffer.Clear();
        buffer.WriteBytes(bufferBytes);
        Debug.Log("3rd part");
        int numberOfPlayers = buffer.ReadInt();
        Debug.Log("read int");
        for (int i = 0; i < numberOfPlayers; i++) {
            string user = buffer.ReadString();
            Debug.Log("User: " + user);
        }


    }

    
    #endregion
}
