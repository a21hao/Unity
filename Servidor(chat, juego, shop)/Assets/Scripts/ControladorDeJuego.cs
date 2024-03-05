using UnityEngine;
using TMPro;
using System.Collections.Generic;
using WebSocketSharp;
using System;
using UnityEngine.UI;
using System.Collections;

public class ControladorDeJuego : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:3002";

    public List<TextMeshProUGUI> scoreText;
    public int score;

    private int pointsPerNormalObject = 1;
    private int pointsPerSpecialObject = 10;

    private Juego spawneador;
    private bool gameEnded = false;
    [SerializeField] TMP_InputField usernameInputField;
    private Queue<string> messageQueue = new Queue<string>();
    public GameObject messagePrefab;
    public Transform messageParent;

    void Start()
    {
        score = 0;
        spawneador = GetComponent<Juego>();
        UpdateScoreText();
        ConnectToServer();
        StartCoroutine(ReconnectRoutine());
    }

    void Update()
    {
        if (!gameEnded)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.transform != null)
                {
                    Destroy(hit.transform.gameObject);

                    if (hit.transform.CompareTag("Special"))
                    {
                        score += pointsPerSpecialObject;
                    }
                    else
                    {
                        score += pointsPerNormalObject;
                    }

                    UpdateScoreText();
                    spawneador.Spawn();
                }
            }
        }
        if (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            InstantiateMessage(message);
        }
    }

    private void InstantiateMessage(string message)
    {
        GameObject messageInstance = Instantiate(messagePrefab, messageParent);
        TextMeshProUGUI messageText = messageInstance.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;
    }

    public void SubtractPoints(int points)
    {
        score -= points;
        UpdateScoreText();
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        foreach (TextMeshProUGUI text in scoreText)
        {
            text.text = score.ToString();
        }
    }

    public void EndGame()
    {
        gameEnded = true;
        spawneador.enabled = false;

        SendMessageToServer();

    }

    public void RestartGame()
    {
        gameEnded = false;
        //score = 0;
        UpdateScoreText();
        spawneador.RestartTime(); 
    }

    public void ChangeCursorType(int cursorIndex)
    {
        switch (cursorIndex)
        {
            case 0:
                pointsPerNormalObject = 2;
                pointsPerSpecialObject = 15;
                break;
            case 1:
                pointsPerNormalObject = 3;
                pointsPerSpecialObject = 20;
                break;
            case 2:
                pointsPerNormalObject = 6;
                pointsPerSpecialObject = 30;
                break;
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
        Debug.Log("Connected to game server");
    }

    public void SendMessageToServer()
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
                    score = score
                };
                string jsonData = "{\"username\": \"" + username + "\", \"Score\": \"" + score + "\"}";
                ws.Send(jsonData);
            }
        }
        else
        {
            Debug.LogWarning("Username is not set.");
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
}
