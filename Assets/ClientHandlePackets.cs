﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ClientHandlePackets : MonoBehaviour {
    public static ClientHandlePackets instance = new ClientHandlePackets();
    private delegate void Packet_(byte[] data);
    private Dictionary<int, Packet_> Packets;

    public void InitMessages() {
        Packets = new Dictionary<int, Packet_>();
        Packets.Add(1, HandleWelcomeMessage);
        //handleLoginSuccess....
        Packets.Add(3, HandleCreateRoomResponse);
        Packets.Add(4, HandleGetPlayersInRoom);
        Packets.Add(5, HandleJoinRoomResponse);
    }

    public void HandleData(byte[] data) {
        int packetnum;
        Packet_ packet;
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadInt();
        buffer = null;
        
        if (packetnum == 0) return;

        if (Packets.TryGetValue(packetnum, out packet)) {
            packet.Invoke(data);
        } else {
            Debug.Log("Packet number does not exist the client doesn't know what to do with the data.");
        }
    }

    void HandleWelcomeMessage(byte[] data) {
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteBytes(data);
        int packetnum = buffer.ReadInt();
        string msg = buffer.ReadString();
        Debug.Log(msg);
    }

    void HandleCreateRoomResponse(byte[] data) {
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteBytes(data);
        int packetnum = buffer.ReadInt();
        int finished = buffer.ReadInt();
        int roomIndex = buffer.ReadInt();
        if (finished == 1) {
            Debug.Log("Succeded with roomIndex: " + roomIndex);
            Network.instance.player.JoinRoom(roomIndex);
            Network.instance.mainMenu.CreateGameSuccessfull();
        } else {
            Debug.Log("Failed to create a room!");
        }
    }

    void HandleGetPlayersInRoom(byte[] data) {
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteBytes(data);
        int packetnum = buffer.ReadInt();
        int numberOfPlayers = buffer.ReadInt();
        Debug.Log(numberOfPlayers);
        for (int i = 0; i < numberOfPlayers; i++) {
            string user = buffer.ReadString();
            if (user != Network.instance.player.GetUsername())
                Network.instance.player.SetTeammateUsername(user);
            Debug.Log("User: " + user);
        }
    }

    void HandleJoinRoomResponse(byte[] data) {
        ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer();
        buffer.WriteBytes(data);
        int packetnum = buffer.ReadInt();
        int response = buffer.ReadInt();
        int roomIndex = buffer.ReadInt();
        if(response == 1) {
            Debug.Log("Joined");
            Network.instance.player.JoinRoom(roomIndex);
            Network.instance.mainMenu.JoinGameSuccessfull();
        } else {
            Debug.Log("Failed");
        }
    }
}
    

