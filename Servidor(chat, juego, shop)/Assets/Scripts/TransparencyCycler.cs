using UnityEngine;
using TMPro;

public class TransparencyCycler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private TextMeshProUGUI[] textMeshPros;
    private int transparencyState = 0;

    // Transparencia semitransparente
    private float semiTransparentAlpha = 0.5f;
    // Transparencia completamente transparente
    private float fullyTransparentAlpha = 0f;
    // Transparencia completamente opaca
    private float opaqueAlpha = 1f;

    void Start()
    {
        spriteRenderers = FindObjectsOfType<SpriteRenderer>();
        textMeshPros = FindObjectsOfType<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeTransparency();
        }
    }

    void ChangeTransparency()
    {
        transparencyState = (transparencyState + 1) % 3;

        switch (transparencyState)
        {
            case 0:
                SetTransparency(opaqueAlpha);
                break;
            case 1:
                SetTransparency(semiTransparentAlpha);
                break;
            case 2:
                SetTransparency(fullyTransparentAlpha);
                break;
        }
    }

    void SetTransparency(float alpha)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        foreach (var textMeshPro in textMeshPros)
        {
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
    }
}
