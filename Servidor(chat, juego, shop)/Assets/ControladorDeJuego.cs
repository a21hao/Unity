using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControladorDeJuego : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;

    private Juego spawneador;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        spawneador = GetComponent<Juego>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Detecta clic del botón izquierdo del ratón
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.transform != null) // Verifica si se ha hecho clic en un objeto
            {
                Destroy(hit.transform.gameObject);
                score += 1;
                scoreText.text = score.ToString();
                spawneador.Spawn();
            }
        }
    }
}
