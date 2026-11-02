using UnityEngine;

public class PianoTile : MonoBehaviour
{
    // dinh danh pokemon, chi so lan roi va toc do roi
    public int pokemonId;
    public int laneIndex;
    public float fallSpeed = 2.4f;

    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private bool isHit = false;

    // moc toa do y khi not roi qua vach ma nguoi choi khong bat kip
    private const float MISS_Y = -4.3f;

    // ham khoi tao not roi voi thong so lan, toc do va hinh anh
    public void Init(int id, int lane, float speed, Sprite pokeSprite, Sprite bgSprite)
    {
        pokemonId = id;
        laneIndex = lane;
        fallSpeed = speed;
        isHit = false;

        // khoi tao doi tuong vien nen quan co
        GameObject bgObj = new GameObject("TileBackground");
        bgObj.transform.SetParent(transform);
        bgObj.transform.localPosition = Vector3.zero;
        bgObj.transform.localScale = Vector3.one;

        bgRenderer = bgObj.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = bgSprite;
        bgRenderer.sortingOrder = 3;
        bgRenderer.color = Color.white;

        // khoi tao doi tuong hinh anh pokemon
        GameObject iconObj = new GameObject("TileIcon");
        iconObj.transform.SetParent(transform);
        iconObj.transform.localPosition = Vector3.zero;
        iconObj.transform.localScale = Vector3.one;

        iconRenderer = iconObj.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = pokeSprite;
        iconRenderer.sortingOrder = 4;
    }

    // ham update di chuyen quan co roi xuong moi frame
    private void Update()
    {
        if (isHit) return;
        if (PianoGameController.Instance != null && (PianoGameController.Instance.IsPause() || PianoGameController.Instance.IsGameOver())) return;

        // di chuyen quan co roi xuong theo toc do
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // kiem tra neu quan co roi qua vach day ma khong bam kip thi tinh la miss
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

    // ham xu ly khi nguoi choi bat trung not roi
    public void OnHit()
    {
        isHit = true;

        // bat coroutine hieu ung thu nho va bien mat khi danh trung
        StartCoroutine(HitEffectRoutine());
    }

    // coroutine dien hoat hieu ung bat trung not nhac
    private System.Collections.IEnumerator HitEffectRoutine()
    {
        float timer = 0f;
        Vector3 initialScale = transform.localScale;

        // phong to nhe va lam mo dan trong 0.12 giay
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
