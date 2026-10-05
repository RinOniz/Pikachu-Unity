using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemoryResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultTimePlayedText;
    [SerializeField] private TextMeshProUGUI playAgainText;

    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject gameOverImage;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button resultMenuButton;

    private void Start()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnMainButtonClicked);
        }

        if (resultMenuButton != null)
        {
            resultMenuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    public void Show(bool winStatus, int score, float playTime)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (stars != null) stars.SetActive(winStatus);
        if (gameOverImage != null) gameOverImage.SetActive(!winStatus);

        if (resultScoreText != null)
        {
            resultScoreText.text = "Score: " + score;
        }

        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (resultTimePlayedText != null)
        {
            resultTimePlayedText.text = "Time: " + timeStr;
        }

        if (playAgainText != null)
        {
            if (winStatus)
            {
                int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);
                if (currentLevel < 3)
                {
                    playAgainText.text = "NEXT LEVEL";
                }
                else
                {
                    playAgainText.text = "YOU WIN!";
                }
            }
            else
            {
                playAgainText.text = "PLAY AGAIN";
            }
        }
    }

    private void OnMainButtonClicked()
    {
        if (playAgainText != null && playAgainText.text == "NEXT LEVEL")
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);
            PlayerPrefs.SetInt("GameLevel", currentLevel + 1);
            SceneManager.LoadScene("MemoryGameScene");
        }
        else if (playAgainText != null && playAgainText.text == "YOU WIN!")
        {
            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            SceneManager.LoadScene("MemoryGameScene");
        }
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
