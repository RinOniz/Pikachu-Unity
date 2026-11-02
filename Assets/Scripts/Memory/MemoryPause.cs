using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPause : MonoBehaviour
{
    // nut tam dung va cac hinh anh bieu tuong pause resume
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    private MemoryGameController gameController;

    // ham khoi tao va gan su kien cho nut pause
    private void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(SetPauseGame);
        }
    }

    // ham cho button pause bat hoac tat trang thai tam dung
    private void SetPauseGame()
    {
        if (gameController == null)
        {
            GameObject controllerObj = GameObject.Find("GameController");
            if (controllerObj != null)
            {
                gameController = controllerObj.GetComponent<MemoryGameController>();
            }
        }

        // khong cho tam dung khi he thong dang trinh chieu xem truoc the
        if (gameController == null || gameController.IsPreviewing()) return;

        // chuyen doi trang thai pause
        if (!gameController.IsPause())
        {
            gameController.SetPause(true);
        }
        else
        {
            gameController.SetPause(false);
            gameController.PlayStartSound();
        }
    }

    // ham update cap nhat giao dien nut pause theo trang thai moi frame
    private void Update()
    {
        if (gameController == null)
        {
            gameController = GetComponent<MemoryGameController>();
            if (gameController == null)
            {
                GameObject controllerObj = GameObject.Find("GameController");
                if (controllerObj != null)
                {
                    gameController = controllerObj.GetComponent<MemoryGameController>();
                }
            }
        }

        if (gameController == null || pauseButton == null) return;

        Image img = pauseButton.GetComponent<Image>();

        // cap nhat bieu tuong va chu khi dang choi hoac dang tam dung
        if (!gameController.IsPause())
        {
            if (img != null && pauseImage != null) img.sprite = pauseImage;
            if (pauseText != null) pauseText.text = "PAUSE";
        }
        else
        {
            if (img != null && resumeImage != null) img.sprite = resumeImage;
            if (pauseText != null) pauseText.text = "RESUME";
        }
    }
}
