using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndlessPause : MonoBehaviour
{
    // nut tam dung va cac bieu tuong hinh anh
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    private EndlessGameController gameController;

    // ham khoi tao va gan su kien cho nut pause
    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
    }

    // ham cho button pause chuyen doi qua lai giua tam dung va tiep tuc
    private void TogglePause()
    {
        if (gameController == null)
        {
            gameController = GetComponent<EndlessGameController>();
        }

        if (gameController != null)
        {
            gameController.TogglePause();
        }
    }

    // ham update cap nhat hinh anh va text cua nut moi frame
    private void Update()
    {
        if (gameController == null) return;

        // cap nhat giao dien khi dang choi hoac dang tam dung
        if (!gameController.IsPause())
        {
            if (pauseImage != null && pauseButton != null)
            {
                Image img = pauseButton.GetComponent<Image>();
                if (img != null) img.sprite = pauseImage;
            }
            if (pauseText != null) pauseText.text = "PAUSE";
        }
        else
        {
            if (resumeImage != null && pauseButton != null)
            {
                Image img = pauseButton.GetComponent<Image>();
                if (img != null) img.sprite = resumeImage;
            }
            if (pauseText != null) pauseText.text = "RESUME";
        }
    }
}
