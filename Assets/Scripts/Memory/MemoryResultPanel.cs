using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemoryResultPanel : MonoBehaviour
{
    // bang ket qua chung
    [SerializeField] private GameObject resultPanel;

    // cac text hien thi thong so
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultTimePlayedText;
    [SerializeField] private TextMeshProUGUI playAgainText;

    // anh ngoi sao thang va anh game over
    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject gameOverImage;

    // cac nut bam dieu huong sau van dau
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button resultMenuButton;

    // ham khoi tao va gan su kien cho cac nut ket qua
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

    // ham hien thi bang ket qua voi thong so thang thua, diem so va thoi gian
    public void Show(bool winStatus, int score, float playTime)
    {
        // an ban co va duong noi de tap trung vao popup
        GameObject boardObj = GameObject.Find("Board");
        if (boardObj != null) boardObj.SetActive(false);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null) pathObj.SetActive(false);

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        // hien thi sao hoac anh thua cuoc
        if (stars != null) stars.SetActive(winStatus);
        if (gameOverImage != null) gameOverImage.SetActive(!winStatus);

        // cap nhat diem so
        if (resultScoreText != null)
        {
            resultScoreText.text = "Score: " + score;
        }

        // dinh dang chuoi phut giay
        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (resultTimePlayedText != null)
        {
            resultTimePlayedText.text = "Time: " + timeStr;
        }

        // cap nhat chu tren nut bam theo tinh huong
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

    // ham cho button chinh de choi lai hoac sang man moi
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

    // ham cho button menu de quay ve man hinh menu chinh
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
