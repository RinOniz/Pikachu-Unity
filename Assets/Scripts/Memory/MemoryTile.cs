using UnityEngine;

public class MemoryTile : MonoBehaviour
{
    // vien nen cua the bai
    [SerializeField] private GameObject tileBackground;

    // toa do hang cot va dinh danh pokemon
    public int row;
    public int col;
    public int id;

    // thanh phan ve hinh anh va trang thai lat the
    private SpriteRenderer iconRenderer;
    private SpriteRenderer bgRenderer;
    private bool isFaceUp = false;

    // thuoc tinh kiem tra the dang mo hay dang up
    public bool IsFaceUp => isFaceUp;

    // ham khoi tao cac thanh phan sprite renderer
    private void Awake()
    {
        iconRenderer = GetComponent<SpriteRenderer>();
        if (tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }
    }

    // ham khoi tao thong so the bai va mac dinh up the xuong
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

        // mac dinh ban dau up the xuong
        HideTile();
    }

    // ham mo the bai va hien thi hinh anh pokemon
    public void ShowTile(bool isSelectedHighlight = true)
    {
        isFaceUp = true;
        if (iconRenderer != null)
        {
            iconRenderer.enabled = true;
        }

        // doi mau nen vien khi duoc chon
        if (bgRenderer != null)
        {
            bgRenderer.color = isSelectedHighlight ? new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f) : Color.white;
        }
    }

    // ham up the bai xuong an hinh anh pokemon
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

    // ham bat sang the bai khi duoc he thong goi y
    public void HighlightHint()
    {
        if (iconRenderer != null)
        {
            iconRenderer.enabled = true;
        }

        // doi mau nen vien sang mau xanh la
        if (bgRenderer != null)
        {
            bgRenderer.color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
        }
    }

    // ham khoi phuc lai trang thai the sau khi het thoi gian goi y
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

    // ham cap nhat lai sprite hinh anh pokemon khi dao ban
    public void SetSprite(Sprite sprite)
    {
        if (iconRenderer == null)
        {
            iconRenderer = GetComponent<SpriteRenderer>();
        }
        iconRenderer.sprite = sprite;
    }

    // ham xu ly khi nguoi choi click chuot vao the bai
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

        // bo qua click khi game dang pause, game over hoac he thong dang trinh chieu
        if (controller.IsPause() || controller.IsGameOver() || controller.IsProcessing() || controller.IsPreviewing())
        {
            return;
        }

        // khong cho phep bam lai vao the da lat mo
        if (isFaceUp)
        {
            return;
        }

        // xu ly chon the thu hai hoac the thu nhat
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
