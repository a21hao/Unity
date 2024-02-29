using UnityEngine;
using WebSocketSharp;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:3001";
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private GameObject chatTextPrefab;
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject sendMessageButton;
    [SerializeField] private GameObject connectButton;
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject admin;

    private string username;
    public bool usernameSent = false;
    private Queue<string> receivedMessages = new Queue<string>();
    private bool newMessage = false;

    [System.Serializable]
    public class Message
    {
        public string username;
        public string message;

        public override string ToString()
        {
            return $"[{username}]: {message}";
        }
    }   

    void Start()
    {
        ConnectToServer();
        usernameInput.onEndEdit.AddListener(delegate { SendUsername(); });
        messageInput.onSubmit.AddListener(delegate { SendMessageToServer(); });
    }

    void Update()
    {
        if (newMessage)
        {
            CheckMessageQueue();
            newMessage = false;
        }
    }

    private void ConnectToServer()
    {
        ws = new WebSocket(serverUrl);
        ws.OnOpen += OnConnected;
        ws.OnMessage += OnMessageReceived;
        ws.OnClose += OnDisconnected;

        ws.Connect();
        connectButton.SetActive(false);
    }

    private void OnConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected to chat server");
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        Debug.Log("Received raw message: " + e.Data);

        if (e.Data.Trim() == "{}")
        {
            Debug.LogError("Received an empty JSON object. Ignoring.");
            return;
        }

        try
        {
            var parsedMessage = JsonUtility.FromJson<Message>(e.Data);
            receivedMessages.Enqueue(parsedMessage.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing message: " + ex.Message);
        }
        newMessage = true;
    }

    private void CheckMessageQueue()
    {
        foreach (string message in receivedMessages)
        {
            GameObject newText = Instantiate(chatTextPrefab, content);
            newText.GetComponent<TextMeshProUGUI>().text = message;
        }

        receivedMessages.Clear();

        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnDisconnected(object sender, CloseEventArgs e)
    {
        Debug.Log("Disconnected from server");
        if (usernamePanel != null)
        {
            usernamePanel.SetActive(true);
        }
        usernameSent = false;
        connectButton.SetActive(true);
    }

    public void SendUsername()
    {
        if (!usernameSent && !string.IsNullOrEmpty(usernameInput.text))
        {
            username = usernameInput.text;
            if (string.IsNullOrEmpty(username))
            {
                username = "Anonymous";
            }

            string jsonUsername = "{\"username\": \"" + username + "\"}";
            ws.Send(jsonUsername);

            if (usernamePanel != null)
            {
                usernamePanel.SetActive(false);
            }

            usernameSent = true;

            // Agregar lógica para activar un GameObject específico si el nombre de usuario es "haojie"
            if (username.Equals("haojie", StringComparison.OrdinalIgnoreCase))
            {
                if (admin != null)
                {
                    admin.SetActive(true);
                }
            }
        }
    }

    public void SendMessageToServer()
    {
        if (usernameSent)
        {
            string message = messageInput.text;
            if (!string.IsNullOrEmpty(message) && ws != null && ws.ReadyState == WebSocketState.Open)
            {
                var messageObject = new
                {
                    username = username,
                    message = message
                };

                string jsonMessage = "{\"username\": \"" + username + "\", \"message\": \"" + messageObject.message + "\"}";
                ws.Send(jsonMessage);
                messageInput.text = string.Empty;
            }
        }
    }

    public void DisconnectFromServer()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close();
        }
    }

    public void ConnectToServerButton()
    {
        ConnectToServer();
    }

    public void SendButtonPressed()
    {
        SendMessageToServer();
    }

    private void OnApplicationQuit()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close();
        }
    }
}
