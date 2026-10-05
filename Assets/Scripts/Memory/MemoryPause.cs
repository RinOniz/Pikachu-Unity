using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPause : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    private MemoryGameController gameController;

    private void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(SetPauseGame);
        }
    }

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

        if (gameController == null || gameController.IsPreviewing()) return;

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
