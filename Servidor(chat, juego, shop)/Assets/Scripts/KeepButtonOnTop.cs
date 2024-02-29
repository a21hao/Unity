using UnityEngine;
using UnityEngine.UI;

public class KeepButtonOnTop : MonoBehaviour
{
    private void Start()
    {
        // Obtener el componente Button
        Button button = GetComponent<Button>();

        // Asegurarse de que el botón esté siempre en la parte superior
        button.transform.SetAsLastSibling();
    }
}
