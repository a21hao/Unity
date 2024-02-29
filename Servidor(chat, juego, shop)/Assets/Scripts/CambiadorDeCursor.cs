using UnityEngine;
using TMPro;
using System.Collections.Generic;
using WebSocketSharp;
using System;
using UnityEngine.UI;

public class CambiadorDeCursor : MonoBehaviour
{
    [System.Serializable]
    public class CursorButton
    {
        public string cursorName;
        public Button button;
        public Texture2D cursorTexture;
        public Vector2 hotSpot;
        public int basePrice;
        public int cursorIndex;
    }

    public GameObject messagePrefab;
    public Transform messageParent;

    private WebSocket ws;
    private string serverUrl = "ws://localhost:3003";

    public CursorButton[] cursorButtons;

    private ControladorDeJuego scoreManager;
    private Queue<string> messageQueue = new Queue<string>();
    [SerializeField] TMP_InputField usernameInputField;

    void Start()
    {
        ConnectToServer();
        scoreManager = FindObjectOfType<ControladorDeJuego>();

        for (int i = 0; i < cursorButtons.Length; i++)
        {
            int buttonIndex = i;
            cursorButtons[i].button.onClick.AddListener(() => CambiarCursor(buttonIndex));
        }
    }

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            InstantiateMessages(message);
        }
    }

    public void CambiarCursor(int index)
    {
        if (index >= 0 && index < cursorButtons.Length)
        {
            int totalPrice = cursorButtons[index].basePrice; // Calcular el precio total
            if (scoreManager != null && scoreManager.score >= totalPrice)
            {
                Cursor.SetCursor(cursorButtons[index].cursorTexture, cursorButtons[index].hotSpot, CursorMode.Auto);
                scoreManager.ChangeCursorType(cursorButtons[index].cursorIndex); // Llamar al método para cambiar el tipo de cursor
                scoreManager.SubtractPoints(totalPrice); // Restar el precio total
                SendCursorIndex(cursorButtons[index].cursorName); // Enviar el índice del cursor al servidor
            }
            else
            {
                Debug.LogWarning("No tienes suficientes puntos para comprar este cursor.");
            }
        }
        else
        {
            Debug.LogWarning("Índice de botón inválido.");
        }
        //selectedCursorIndex = cursorButtons[index].cursorIndex;
    }

    private void SendCursorIndex(string cursorName)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            Debug.Log("Sending cursor index to server: " + cursorName);
            string username = usernameInputField.text;
            var data = new
            {
                username = username,
                cursorName = cursorName
            };
            string jsonData = "{\"username\": \"" + username + "\", \"Cursor\": \"" + cursorName + "\"}";
            ws.Send(jsonData);
        }
        else
        {
            Debug.LogWarning("WebSocket connection is not open.");
        }
    }

    private void ConnectToServer()
    {
        ws = new WebSocket(serverUrl);
        ws.OnOpen += OnConnected;
        ws.OnMessage += OnMessageReceived;
        ws.OnClose += OnDisconnected;

        ws.Connect();
    }

    private void OnConnected(object sender, System.EventArgs e)
    {
        Debug.Log("Connected to shop server");
    }

    public void SendMessageToServer(int cursorIndex)
    {
        if (!string.IsNullOrEmpty(usernameInputField.text))
        {
            string username = usernameInputField.text;

            if (ws != null && ws.ReadyState == WebSocketState.Open)
            {
                Debug.Log("Sending message with username: " + username);
                var data = new
                {
                    username = username,
                    cursorIndex = cursorIndex
                };
                string jsonData = "{\"username\": \"" + username + "\", \"Update\": \"" + cursorIndex + "\"}";
                ws.Send(jsonData);
            }
        }
    }


    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        Debug.Log("Received raw message: " + e.Data);

        if (e.Data.Trim() == "{}")
        {
            Debug.LogError("Received an empty JSON object. Ignoring.");
            return;
        }

        messageQueue.Enqueue(e.Data);
    }

    private void InstantiateMessages(string message)
    {
            GameObject messageInstance = Instantiate(messagePrefab, messageParent);
            TextMeshProUGUI messageText = messageInstance.GetComponentInChildren<TextMeshProUGUI>();
            messageText.text = message;
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
}
