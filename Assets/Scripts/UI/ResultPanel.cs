using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
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
        playAgainButton.onClick.AddListener(OnMainButtonClicked);
        resultMenuButton.onClick.AddListener(ReturnToMenu);
    }

    public void Show(bool winStatus, int score, float playTime)
    {
        GameObject boardObj = GameObject.Find("Board");
        if (boardObj != null) boardObj.SetActive(false);

        GameObject dangerLineObj = GameObject.Find("DangerLine");
        if (dangerLineObj != null) dangerLineObj.SetActive(false);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null) pathObj.SetActive(false);

        resultPanel.SetActive(true);

        stars.SetActive(winStatus);
        gameOverImage.SetActive(!winStatus);

        resultScoreText.text = "Score: " + score;

        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        resultTimePlayedText.text = "Time: " + timeStr;

        if (winStatus)
        {
            int gameMode = PlayerPrefs.GetInt("GameMode", 0);

            if (gameMode == 1) // Funny Mode chỉ có 1 level duy nhất
            {
                playAgainText.text = "YOU WIN!";
            }
            else
            {
                int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);
                if (currentLevel < 3) playAgainText.text = "NEXT LEVEL";
                else playAgainText.text = "YOU WIN!";
            }
        }
        else
        {
            playAgainText.text = "PLAY AGAIN";
        }
    }

    private void OnMainButtonClicked()
    {
        if (playAgainText.text == "PLAY AGAIN")
        {
            SceneManager.LoadScene("GameScene");
        }
        else if (playAgainText.text == "NEXT LEVEL")
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

            PlayerPrefs.SetInt("GameLevel", currentLevel + 1);

            SceneManager.LoadScene("GameScene");
        }
        else if (playAgainText.text == "YOU WIN!")
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
