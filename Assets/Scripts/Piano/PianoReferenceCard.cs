using System.Collections;
using TMPro;
using UnityEngine;

public class PianoReferenceCard : MonoBehaviour
{
    // phim ban phim gan voi the nay, chu cai hien thi va id pokemon
    public KeyCode boundKey;
    public string keyLabel;
    public int pokemonId;

    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private TextMeshPro labelText;

    // cac mau sac tuong ung voi trang thai binh thuong, bam dung va bam sai
    private Color normalBgColor = Color.white;
    private Color pressedBgColor = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f); // xanh la sang
    private Color errorBgColor = new Color(231f / 255f, 76f / 255f, 60f / 255f, 1.0f);   // do canh bao

    private Coroutine flashRoutine;

    // ham khoi tao the tham chieu gom vien, anh pokemon va chu cai phim
    public void Init(KeyCode key, string label, int pokeId, Sprite pokeSprite, Sprite bgSprite)
    {
        boundKey = key;
        keyLabel = label;
        pokemonId = pokeId;

        // buoc 1: khoi tao nen vien the tham chieu
        GameObject bgObj = new GameObject("CardBackground");
        bgObj.transform.SetParent(transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        bgRenderer = bgObj.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = bgSprite;
        bgRenderer.sortingOrder = 2;
        bgRenderer.color = normalBgColor;

        // buoc 2: khoi tao bieu tuong pokemon dat hoi lech len tren
        GameObject iconObj = new GameObject("CardIcon");
        iconObj.transform.SetParent(transform);
        iconObj.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        iconObj.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        iconRenderer = iconObj.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = pokeSprite;
        iconRenderer.sortingOrder = 3;

        // buoc 3: khoi tao nhan chu cai dai dien cho phim bam o phia duoi
        GameObject labelObj = new GameObject("KeyBadge");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = new Vector3(0f, -0.38f, 0f);

        labelText = labelObj.AddComponent<TextMeshPro>();
        labelText.text = label;
        labelText.fontSize = 3.6f;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = new Color(241f / 255f, 196f / 255f, 15f / 255f); // mau vang noi bat
        labelText.sortingOrder = 4;
        labelText.fontStyle = FontStyles.Bold;
    }

    // ham nhay sang mau xanh la khi nguoi choi bam dung phim
    public void HighlightPressed()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(pressedBgColor));
    }

    // ham nhay sang mau do khi nguoi choi bam sai phim
    public void HighlightError()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(errorBgColor));
    }

    // coroutine tao hieu ung chop mau trong 0.14 giay roi tro ve binh thuong
    private IEnumerator FlashRoutine(Color targetColor)
    {
        if (bgRenderer != null) bgRenderer.color = targetColor;
        yield return new WaitForSeconds(0.14f);
        if (bgRenderer != null) bgRenderer.color = normalBgColor;
    }
}
