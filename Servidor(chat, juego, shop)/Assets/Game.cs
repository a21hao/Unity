//using UnityEngine;
//using WebSocketSharp;
//using System.Collections.Generic;
//using System;

//public class Game : MonoBehaviour
//{
//    private WebSocket ws;
//    private string serverUrl = "ws://localhost:3001";
//    private string player = "1";
//    [SerializeField] private GameObject connectButton;

//    public int numRows = 6;
//    public int numColumns = 7;
//    public int[,] numMatrix;

//    [Serializable]
//    public class Wrapper
//    {
//        public string player;
//        public List<List<int>> matrix;
//    }

//    void Start()
//    {
//        ConnectToServer();
//        GameObject[,] objectsMatrix = CircleSpawner.circleMatrix;
//        int[,] numMatrix = new int[numRows, numColumns];
//    }

//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

//            if (hit.collider != null)
//            {
//                Debug.LogError("hit" + hit.collider.gameObject.name);
//                Debug.Log("Hit object: " + hit.collider.gameObject.name);
//                GameObject circleHit = hit.collider.gameObject;
//                string lastMove = hit.collider.gameObject.name;
//                if (!string.IsNullOrEmpty(lastMove) && ws != null && ws.ReadyState == WebSocketState.Open)
//                {
//                    string jsonMove = "{\"player\": \"" + player + "\", \"move\": \"" + lastMove + "\"}";
//                    ws.Send(jsonMove);
//                }
//                else
//                {
//                    Debug.Log("No object hit.");
//                }
//            }
//        }
//    }

//    private void ConnectToServer()
//    {
//        ws = new WebSocket(serverUrl);
//        ws.OnOpen += OnConnected;
//        ws.OnMessage += OnMessageReceived;
//        ws.OnClose += OnDisconnected;

//        ws.Connect();
//        connectButton.SetActive(false);
//    }

//    private void OnConnected(object sender, System.EventArgs e)
//    {
//        Debug.Log("Connected to server");
//    }

//    private void OnMessageReceived(object sender, MessageEventArgs e)
//    {
//        Debug.LogError("Received raw message: " + e.Data);

//        Wrapper receivedData = JsonUtility.FromJson<Wrapper>(e.Data);

//        // Update player value from the received message
//        player = receivedData.player;

//        // Update the matrix in Unity
//        List<List<int>> matrixList = receivedData.matrix;
//        for (int i = 0; i < numRows; i++)
//        {
//            for (int j = 0; j < numColumns; j++)
//            {
//                numMatrix[i, j] = matrixList[i][j];
//            }
//        }
//        Debug.LogError(numMatrix);
//        ChangeCircleColor();
//    }

//    private void OnDisconnected(object sender, CloseEventArgs e)
//    {
//        Debug.Log("Disconnected from server");
//        connectButton.SetActive(true);
//        StartCoroutine(AttemptReconnect());
//    }

//    IEnumerator<WaitForSeconds> AttemptReconnect()
//    {
//        while (true)
//        {
//            try
//            {
//                ConnectToServer();
//                Debug.Log("Reconnected to Chat");
//            }
//            catch
//            {
//                Debug.Log("Disconnected From Chat");
//            }
//            yield return new WaitForSeconds(5f);
//        }
//    }

//    void ChangeCircleColor()
//    {
//        for (int i = 0; i < numRows; i++)
//        {
//            for (int j = 0; j < numColumns; j++)
//            {
//                GameObject circle = CircleSpawner.circleMatrix[i, j];
//                SpriteRenderer circleRenderer = circle.GetComponent<SpriteRenderer>();

//                if (numMatrix[i, j] == 1)
//                {
//                    if (circleRenderer != null)
//                    {
//                        circleRenderer.color = Color.red;
//                    }
//                    else
//                    {
//                        Debug.LogError("Circle does not have a SpriteRenderer component.");
//                    }
//                }
//                else if (numMatrix[i, j] == 2)
//                {
//                    if (circleRenderer != null)
//                    {
//                        circleRenderer.color = Color.blue;
//                    }
//                    else
//                    {
//                        Debug.LogError("Circle does not have a SpriteRenderer component.");
//                    }
//                }
//                else if (numMatrix[i, j] == 3)
//                {
//                    if (circleRenderer != null)
//                    {
//                        // Change the color to your desired color for player '3'
//                        circleRenderer.color = Color.green; // Change to the color you want
//                    }
//                    else
//                    {
//                        Debug.LogError("Circle does not have a SpriteRenderer component.");
//                    }
//                }
//            }
//        }
//    }
//}
