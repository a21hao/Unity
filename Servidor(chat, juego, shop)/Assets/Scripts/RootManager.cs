using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class RootManager : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:3000";
    [SerializeField] private Toggle toggleChatStop;
    [SerializeField] private Toggle toggleGameStop;
    [SerializeField] private Toggle toggleShopStop;

    void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        ws = new WebSocket(serverUrl);
        ws.OnOpen += OnConnected;
        ws.OnClose += OnDisconnected;

        ws.Connect();
    }

    private void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected to main server");
    }

    public void SendSignalToServer(string serverName, string signal)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            string jsonData = "{\"serverName\": \"" + serverName + "\", \"signal\": \"" + signal + "\"}";
            ws.Send(jsonData);
            Debug.Log(serverName +signal);
        }
        else
        {
            Debug.LogWarning("WebSocket connection is not open.");
        }
    }

    private void OnDisconnected(object sender, CloseEventArgs e)
    {
        Debug.Log("Disconnected from server");
    }

    public void DisconnectFromServer()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close();
        }
    }

    private void OnApplicationQuit()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close();
        }
    }

    public void ToggleChatServer()
    {
        string signal = toggleChatStop.isOn ? "stop" : "start";
        SendSignalToServer("chat", signal);
    }

    public void ToggleGameServer()
    {
        string signal = toggleGameStop.isOn ? "stop" : "start";
        SendSignalToServer("game", signal);
    }

    public void ToggleShopServer()
    {
        string signal = toggleShopStop.isOn ? "stop" : "start";
        SendSignalToServer("shop", signal);
    }
}
