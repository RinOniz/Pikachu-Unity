using UnityEngine;

public class EndlessTile : MonoBehaviour
{
    // vien nen cua quan co de doi mau highlight
    [SerializeField] private GameObject tileBackground;

    // toa do hang cot va dinh danh sprite
    public int row;
    public int col;
    public int id;

    private SpriteRenderer bgRenderer;

    // ham khoi tao sprite renderer cua nen quan co
    private void Awake()
    {
        if (tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }
    }

    // ham xu ly su kien click chuot vao quan co trong che do endless
    public void OnMouseDown()
    {
        GameObject gameControllerObj = GameObject.Find("GameController");
        if (gameControllerObj == null)
        {
            return;
        }

        EndlessGameController gameController = gameControllerObj.GetComponent<EndlessGameController>();
        if (gameController == null || gameController.IsPause() || gameController.IsGameOver())
        {
            return;
        }

        if (bgRenderer == null && tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }

        // xu ly khi da co o thu nhat duoc chon hoac day la o thu nhat
        if (gameController.IsSelected())
        {
            if (gameController.GetFirstTile() != gameObject)
            {
                if (bgRenderer != null)
                {
                    bgRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                }

                // chuyen quan co thu hai cho controller xu ly so sanh
                gameController.SelectSecondTile(gameObject);
            }
        }
        else
        {
            if (bgRenderer != null)
            {
                bgRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
            }

            // luu quan co thu nhat vao controller
            gameController.SelectFirstTile(gameObject);
        }
    }

    // ham bat sang quan co mau xanh la khi duoc goi y
    public void HighlightHint()
    {
        if (bgRenderer == null && tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }

        if (bgRenderer != null)
        {
            bgRenderer.color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
        }
    }

    // ham khoi phuc lai mau trang binh thuong cho quan co
    public void RestoreColor()
    {
        if (bgRenderer == null && tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }

        if (bgRenderer != null)
        {
            bgRenderer.color = Color.white;
        }
    }
}
