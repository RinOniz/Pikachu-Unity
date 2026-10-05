using UnityEngine;

public class EndlessTile : MonoBehaviour
{
    [SerializeField] private GameObject tileBackground;

    public int row;
    public int col;
    public int id;

    private SpriteRenderer bgRenderer;

    private void Awake()
    {
        if (tileBackground != null)
        {
            bgRenderer = tileBackground.GetComponent<SpriteRenderer>();
        }
    }

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

        if (gameController.IsSelected())
        {
            if (gameController.GetFirstTile() != gameObject)
            {
                if (bgRenderer != null)
                {
                    bgRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                }

                gameController.SelectSecondTile(gameObject);
            }
        }
        else
        {
            if (bgRenderer != null)
            {
                bgRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
            }

            gameController.SelectFirstTile(gameObject);
        }
    }

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
