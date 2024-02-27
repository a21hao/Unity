using UnityEngine;
using UnityEngine.UI;

public class CambiadorDeCursor : MonoBehaviour
{
    [System.Serializable]
    public class CursorButton
    {
        public Button button;
        public Texture2D cursorTexture;
        public Vector2 hotSpot;
        public int basePrice;
        public int cursorIndex; // Nuevo campo para indicar el índice del cursor
    }

    public CursorButton[] cursorButtons;

    private ControladorDeJuego scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ControladorDeJuego>();

        for (int i = 0; i < cursorButtons.Length; i++)
        {
            int buttonIndex = i;
            cursorButtons[i].button.onClick.AddListener(() => CambiarCursor(buttonIndex));
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
    }
}
