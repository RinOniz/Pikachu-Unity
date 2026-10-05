using UnityEngine;

public class MemoryTile : MonoBehaviour
{
    [SerializeField] private GameObject tileBackground;

    public int row;
    public int col;
    public int id;

    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private bool isFaceUp = false;

    public bool IsFaceUp => isFaceUp;

    private void Awake()
    {
        iconRenderer = GetComponent<SpriteRenderer>();
        if (tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }
    }

    public void Initialize(int row, int col, int id, Sprite sprite)
    {
        this.row = row;
        this.col = col;
        this.id = id;

        if (iconRenderer == null)
        {
            iconRenderer = GetComponent<SpriteRenderer>();
        }
        if (bgRenderer == null && tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }

        iconRenderer.sortingOrder = 2;
        iconRenderer.sprite = sprite;

        HideTile();
    }

    public void ShowTile(bool isSelectedHighlight = true)
    {
        isFaceUp = true;
        if (iconRenderer != null)
        {
            iconRenderer.enabled = true;
        }

        if (bgRenderer != null)
        {
            bgRenderer.color = isSelectedHighlight ? new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f) : Color.white;
        }
    }

    public void HideTile()
    {
        isFaceUp = false;
        if (iconRenderer != null)
        {
            iconRenderer.enabled = false;
        }

        if (bgRenderer != null)
        {
            bgRenderer.color = Color.white;
        }
    }

    public void HighlightHint()
    {
        if (iconRenderer != null)
        {
            iconRenderer.enabled = true;
        }

        if (bgRenderer != null)
        {
            bgRenderer.color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
        }
    }

    public void RestoreFromHint()
    {
        if (!isFaceUp)
        {
            HideTile();
        }
        else
        {
            ShowTile();
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (iconRenderer == null)
        {
            iconRenderer = GetComponent<SpriteRenderer>();
        }
        iconRenderer.sprite = sprite;
    }

    public void OnMouseDown()
    {
        GameObject gameControllerObj = GameObject.Find("GameController");
        if (gameControllerObj == null)
        {
            return;
        }

        MemoryGameController controller = gameControllerObj.GetComponent<MemoryGameController>();
        if (controller == null)
        {
            return;
        }

        if (controller.IsPause() || controller.IsGameOver() || controller.IsProcessing() || controller.IsPreviewing())
        {
            return;
        }

        // Không cho phép bấm lại vào ô đã mở
        if (isFaceUp)
        {
            return;
        }

        if (controller.IsSelected())
        {
            if (controller.GetFirstTile() != this)
            {
                controller.SelectSecondTile(this);
            }
        }
        else
        {
            controller.SelectFirstTile(this);
        }
    }
}
