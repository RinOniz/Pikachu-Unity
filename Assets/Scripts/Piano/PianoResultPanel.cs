using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PianoResultPanel : MonoBehaviour
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
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (resultMenuButton != null)
        {
            resultMenuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    public void Show(int score, int notesHit, float playTime)
    {
        // Ẩn tất cả các đối tượng trò chơi phía sau
        GameObject fieldObj = GameObject.Find("PianoField");
        if (fieldObj != null) fieldObj.SetActive(false);

        GameObject lanesObj = GameObject.Find("Lanes");
        if (lanesObj != null) lanesObj.SetActive(false);

        GameObject refObj = GameObject.Find("ReferenceCards");
        if (refObj != null) refObj.SetActive(false);

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (stars != null) stars.SetActive(false);
        if (gameOverImage != null) gameOverImage.SetActive(true);

        if (resultScoreText != null)
        {
            resultScoreText.text = $"Score: {score} | Hits: {notesHit}";
        }

        float minutes = Mathf.FloorToInt(playTime / 60f);
        float seconds = Mathf.FloorToInt(playTime % 60f);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (resultTimePlayedText != null)
        {
            resultTimePlayedText.text = "Time: " + timeStr;
        }

        if (playAgainText != null)
        {
            playAgainText.text = "PLAY AGAIN";
        }
    }

    private void OnPlayAgainClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
