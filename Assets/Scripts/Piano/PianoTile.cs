using UnityEngine;

public class PianoTile : MonoBehaviour
{
    public int pokemonId;
    public int laneIndex;
    public float fallSpeed = 2.4f;

    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private bool isHit = false;

    private const float MISS_Y = -4.3f;

    public void Init(int id, int lane, float speed, Sprite pokeSprite, Sprite bgSprite)
    {
        pokemonId = id;
        laneIndex = lane;
        fallSpeed = speed;
        isHit = false;

        // Background card
        GameObject bgObj = new GameObject("TileBackground");
        bgObj.transform.SetParent(transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        bgRenderer = bgObj.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = bgSprite;
        bgRenderer.sortingOrder = 2;
        bgRenderer.color = Color.white;

        // Pokemon icon
        GameObject iconObj = new GameObject("TileIcon");
        iconObj.transform.SetParent(transform);
        iconObj.transform.localPosition = Vector3.zero;
        iconObj.transform.localScale = Vector3.one;

        iconRenderer = iconObj.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = pokeSprite;
        iconRenderer.sortingOrder = 3;
    }

    private void Update()
    {
        if (isHit) return;
        if (PianoGameController.Instance != null && (PianoGameController.Instance.IsPause() || PianoGameController.Instance.IsGameOver())) return;

        // Di chuyển rơi xuống
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Kiểm tra rơi quá vạch đáy mà không bấm -> Miss
        if (transform.position.y <= MISS_Y)
        {
            isHit = true;
            if (PianoGameController.Instance != null)
            {
                PianoGameController.Instance.OnTileMiss(this);
            }
            Destroy(gameObject);
        }
    }

    public void OnHit()
    {
        isHit = true;
        // Hiệu ứng bắt trúng: thu nhỏ và biến mất
        StartCoroutine(HitEffectRoutine());
    }

    private System.Collections.IEnumerator HitEffectRoutine()
    {
        float timer = 0f;
        Vector3 initialScale = transform.localScale;

        while (timer < 0.12f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.12f;
            transform.localScale = Vector3.Lerp(initialScale, initialScale * 1.25f, t);

            if (bgRenderer != null)
            {
                bgRenderer.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t);
            }
            if (iconRenderer != null)
            {
                iconRenderer.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
