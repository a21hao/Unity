using UnityEngine;
using WebSocketSharp;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;

public class Connection : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:3000";
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private GameObject usernamePanel;
    [SerializeField] private GameObject sendMessageButton;
    [SerializeField] private GameObject connectButton;
    public Transform content;

    private string username;
    private bool usernameSent = false;
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
        // Asignar una función al evento "onEndEdit" para detectar cuando se termina de editar el campo de nombre de usuario
        usernameInput.onEndEdit.AddListener(delegate { SendUsername(); });
        // Agregar el Listener para detectar cuando se presiona Enter en el campo de texto del mensaje
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
        Debug.Log("Connected to server");
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        Debug.Log("Received raw message: " + e.Data);

        // Check if the message is not an empty JSON object
        if (e.Data.Trim() == "{}")
        {
            Debug.LogError("Received an empty JSON object. Ignoring.");
            return;
        }

        try
        {
            var parsedMessage = JsonUtility.FromJson<Message>(e.Data);
            //Debug.Log("Parsed message: " + parsedMessage);

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
        // Clear the current text before displaying messages
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Display messages
        foreach (string message in receivedMessages)
        {
            GameObject newText = Instantiate(textPrefab, content);
            newText.GetComponent<TextMeshProUGUI>().text = message;
        }
    }




    private void OnDisconnected(object sender, CloseEventArgs e)
    {
        Debug.Log("Disconnected from server");
        if (usernamePanel != null)
        {
            Debug.Log("klk");
            usernamePanel.SetActive(true);
        }
        usernameSent = false;
        connectButton.SetActive(true);
    }

    public void SendUsername()
    {
        if (!usernameSent)
        {
            username = usernameInput.text;
            if (string.IsNullOrEmpty(username))
            {
                username = "Anonymous";
            }

            // Format the JSON message for the username
            string jsonUsername = "{\"username\": \"" + username + "\"}";
            ws.Send(jsonUsername);

            // Disable the username panel
            if (usernamePanel != null)
            {
                usernamePanel.SetActive(false);
            }

            usernameSent = true; // Mark that the username has been sent
        }
    }

    public void SendMessageToServer()
    {
        if (usernameSent)
        {
            string message = messageInput.text;
            if (!string.IsNullOrEmpty(message) && ws != null && ws.ReadyState == WebSocketState.Open)
            {
                // Format the JSON message for the chat message including the username
                var messageObject = new
                {
                    username = username,
                    message = message
                };

                string jsonMessage = "{\"username\": \"" + username + "\", \"message\": \"" + messageObject.message + "\"}";
                //string jsonMessage = JsonUtility.ToJson(messageObject);
                Debug.Log("Sending message to server: " + jsonMessage);
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
