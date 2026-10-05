using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndlessPause : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    private EndlessGameController gameController;

    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
    }

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

    private void Update()
    {
        if (gameController == null) return;

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
