using UnityEngine;
using WebSocketSharp;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;
using System.Collections;

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
    private List<TextMeshProUGUI> chatMessages = new List<TextMeshProUGUI>();

    private int transparencyState = 0;
    private float semiTransparentAlpha = 0.5f;
    private float fullyTransparentAlpha = 0f;
    private float opaqueAlpha = 1f;

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
        admin.SetActive(false);
        StartCoroutine(ReconnectRoutine());
    }

    void Update()
    {
        if (newMessage)
        {
            CheckMessageQueue();
            newMessage = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleTransparency();
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
        while (receivedMessages.Count > 0)
        {
            string message = receivedMessages.Dequeue();
            GameObject newText = Instantiate(chatTextPrefab, content);
            TextMeshProUGUI tmp = newText.GetComponent<TextMeshProUGUI>();
            tmp.text = message;
            chatMessages.Add(tmp);
        }

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

    IEnumerator ReconnectRoutine()
    {
        while (true)
        {
            if (ws == null || ws.ReadyState != WebSocketState.Open)
            {
                Debug.Log("Attempting to reconnect to server...");
                ConnectToServer();
            }

            yield return new WaitForSeconds(5f);
        }
    }

    private void CycleTransparency()
    {
        transparencyState = (transparencyState + 1) % 3;

        float alpha;
        switch (transparencyState)
        {
            case 0:
                alpha = opaqueAlpha;
                break;
            case 1:
                alpha = semiTransparentAlpha;
                break;
            case 2:
                alpha = fullyTransparentAlpha;
                break;
            default:
                alpha = opaqueAlpha;
                break;
        }

        foreach (var tmp in chatMessages)
        {
            Color color = tmp.color;
            color.a = alpha;
            tmp.color = color;
        }
    }
}
