using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ControladorDeJuego : MonoBehaviour
{
    public List<TextMeshProUGUI> scoreText;
    public int score;
    private int pointsPerNormalObject = 1;
    private int pointsPerSpecialObject = 10;

    private Juego spawneador;
    private bool gameEnded = false;

    void Start()
    {
        score = 0;
        spawneador = GetComponent<Juego>();
        UpdateScoreText();
    }

    void Update()
    {
        if (!gameEnded) // Verificar si el juego aún no ha terminado
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
        spawneador.enabled = false; // Deshabilitar el componente Juego para detener el spawning
    }

    public void ChangeCursorType(int cursorIndex)
    {
        switch (cursorIndex)
        {
            case 0: // Copper Cursor
                pointsPerNormalObject = 2; // Aumenta en 1
                pointsPerSpecialObject = 15; // Aumenta en 10
                break;
            case 1: // Silver Cursor
                pointsPerNormalObject = 3; // Aumenta en 2
                pointsPerSpecialObject = 20; // Aumenta en 20
                break;
            case 2: // Gold Cursor
                pointsPerNormalObject = 6; // Aumenta en 5
                pointsPerSpecialObject = 30; // Aumenta en 50
                break;
        }
    }
}
