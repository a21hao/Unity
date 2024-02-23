using UnityEngine;
using TMPro;

public class ControladorDeJuego : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score;
    private int pointsPerNormalObject = 1;
    private int pointsPerSpecialObject = 10;

    private Juego spawneador;

    void Start()
    {
        score = 0;
        spawneador = GetComponent<Juego>();
    }

    void Update()
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

                scoreText.text = score.ToString();
                spawneador.Spawn();
            }
        }
    }
}
