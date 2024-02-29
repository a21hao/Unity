//using UnityEngine;
//using WebSocketSharp;
//using TMPro;
//using System.Collections.Generic;
//using System;
//using System.Text;
//using UnityEngine.UI; // Add this line

//public class Store : MonoBehaviour
//{
//    private WebSocket ws;
//    private string serverUrl = "ws://localhost:3002";

//    [SerializeField] private TMP_Text moneyText;
//    [SerializeField] private GameObject item1;
//    [SerializeField] private GameObject item2;
//    [SerializeField] private GameObject item3;
//    [SerializeField] private GameObject item4;

//    private int playerMoney = 100;

//    // Start is called before the first frame update
//    void Start()
//    {
//        UpdateMoneyText();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // Check for other update logic
//    }

//    // Update the money display
//    private void UpdateMoneyText()
//    {
//        moneyText.text = playerMoney.ToString();
//    }

//    // Method for buying Item1
//    public void BuyItem1()
//    {
//        int item1Cost = 10;
//        if (playerMoney >= item1Cost)
//        {
//            playerMoney -= item1Cost;
//            item1.SetActive(false);
//            UpdateMoneyText();
//            Debug.Log("bought");

//        }
//        else
//        {
//            Debug.Log("Not enough money to buy Item 1!");
//        }
//    }
//    public void BuyItem2()
//    {
//        int item2Cost = 20;
//        if (playerMoney >= item2Cost)
//        {
//            playerMoney -= item2Cost;
//            item2.SetActive(false);
//            UpdateMoneyText();
//            Debug.Log("bought");

//        }
//        else
//        {
//            Debug.Log("Not enough money to buy Item 2!");
//        }
//    }
//    public void BuyItem3()
//    {
//        int item3Cost = 40;
//        if (playerMoney >= item3Cost)
//        {
//            playerMoney -= item3Cost;
//            item3.SetActive(false);
//            UpdateMoneyText();
//            Debug.Log("bought");

//        }
//        else
//        {
//            Debug.Log("Not enough money to buy Item 3!");
//        }
//    }
//    public void BuyItem4()
//    {
//        int item4Cost = 100;
//        if (playerMoney >= item4Cost)
//        {
//            playerMoney -= item4Cost;
//            item4.SetActive(false);
//            UpdateMoneyText();
//            Debug.Log("bought");

//        }
//        else
//        {
//            Debug.Log("Not enough money to buy Item 4!");
//        }
//    }

//    // Similar methods for other items if needed
//}
