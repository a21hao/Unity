using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

public class RootManager : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:3000";
    [SerializeField] private GameObject buttonChatStop;
    [SerializeField] private GameObject buttonGameStop;
    [SerializeField] private GameObject buttonShopStop;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
        buttonChatStop.GetComponent<Button>().onClick.AddListener(SendStopSignalToChatServer);
        buttonGameStop.GetComponent<Button>().onClick.AddListener(SendStopSignalToGameServer);
        buttonShopStop.GetComponent<Button>().onClick.AddListener(SendStopSignalToShopServer);
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
            string jsonData = "{\"server\": \"" + serverName + "\", \"signal\": \"" + signal + "\"}";
            ws.Send(jsonData);
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

    // Métodos para enviar señales específicas a los servidores hijos
    public void SendStopSignalToChatServer()
    {
        SendSignalToServer("chat", "stop");
    }

    public void SendStartSignalToChatServer()
    {
        SendSignalToServer("chat", "start");
    }

    public void SendStopSignalToGameServer()
    {
        SendSignalToServer("game", "stop");
    }

    public void SendStartSignalToGameServer()
    {
        SendSignalToServer("game", "start");
    }

    public void SendStopSignalToShopServer()
    {
        SendSignalToServer("shop", "stop");
    }

    public void SendStartSignalToShopServer()
    {
        SendSignalToServer("shop", "start");
    }
}
