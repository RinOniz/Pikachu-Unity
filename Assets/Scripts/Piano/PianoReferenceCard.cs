using System.Collections;
using TMPro;
using UnityEngine;

public class PianoReferenceCard : MonoBehaviour
{
    public KeyCode boundKey;
    public string keyLabel;
    public int pokemonId;

    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private TextMeshPro labelText;

    private Color normalBgColor = Color.white;
    private Color pressedBgColor = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f); // Xanh lá sáng
    private Color errorBgColor = new Color(231f / 255f, 76f / 255f, 60f / 255f, 1.0f);   // Đỏ cảnh báo

    private Coroutine flashRoutine;

    public void Init(KeyCode key, string label, int pokeId, Sprite pokeSprite, Sprite bgSprite)
    {
        boundKey = key;
        keyLabel = label;
        pokemonId = pokeId;

        // 1. Background card
        GameObject bgObj = new GameObject("CardBackground");
        bgObj.transform.SetParent(transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        bgRenderer = bgObj.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = bgSprite;
        bgRenderer.sortingOrder = 2;
        bgRenderer.color = normalBgColor;

        // 2. Pokemon icon (hơi đẩy lên một chút để nhường chỗ cho nhãn phím)
        GameObject iconObj = new GameObject("CardIcon");
        iconObj.transform.SetParent(transform);
        iconObj.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        iconObj.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        iconRenderer = iconObj.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = pokeSprite;
        iconRenderer.sortingOrder = 3;

        // 3. Key label badge (chữ phím to rõ ở phía dưới ô)
        GameObject labelObj = new GameObject("KeyBadge");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = new Vector3(0f, -0.38f, 0f);

        labelText = labelObj.AddComponent<TextMeshPro>();
        labelText.text = label;
        labelText.fontSize = 3.6f;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = new Color(241f / 255f, 196f / 255f, 15f / 255f); // Màu vàng nổi bật
        labelText.sortingOrder = 4;
        labelText.fontStyle = FontStyles.Bold;
    }

    public void HighlightPressed()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(pressedBgColor));
    }

    public void HighlightError()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(errorBgColor));
    }

    private IEnumerator FlashRoutine(Color targetColor)
    {
        if (bgRenderer != null) bgRenderer.color = targetColor;
        yield return new WaitForSeconds(0.14f);
        if (bgRenderer != null) bgRenderer.color = normalBgColor;
    }
}
