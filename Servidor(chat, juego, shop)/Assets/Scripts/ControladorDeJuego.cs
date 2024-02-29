using UnityEngine;
using TMPro;
using System.Collections.Generic;
using WebSocketSharp;
using System;
using UnityEngine.UI;

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

    void Start()
    {
        score = 0;
        spawneador = GetComponent<Juego>();
        UpdateScoreText();
        ConnectToServer();
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

        //if (ws != null && ws.ReadyState == WebSocketState.Open)
        //{
        //    var data = new
        //    {
        //        username = username,
        //        score = score
        //    };
        //    string jsonData = JsonUtility.ToJson(data);
        //    ws.Send(jsonData);
        //}
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
                var data = new
                {
                    username = username,
                    score = score
                };
                string jsonData = JsonUtility.ToJson(data);
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
