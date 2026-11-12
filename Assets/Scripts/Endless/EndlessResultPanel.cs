using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessResultPanel : MonoBehaviour
{
    // bang ket qua chung
    [SerializeField] private GameObject resultPanel;

    // text hien thi thong so van sinh ton
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultTimePlayedText;
    [SerializeField] private TextMeshProUGUI playAgainText;

    // cac gameobject trang tri
    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject gameOverImage;

    // cac button dieu huong
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button resultMenuButton;

    // ham khoi tao va gan su kien cho cac nut ket qua
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

    // ham hien thi bang ket qua sinh ton voi so cap da an va so hang da day
    public void Show(float survivalTime, int pairsCleared, int rowsPushed)
    {
        // an ban co va cac vat the gameplay
        GameObject boardObj = GameObject.Find("Board");
        if (boardObj != null) boardObj.SetActive(false);

        GameObject dangerLineObj = GameObject.Find("DangerLine");
        if (dangerLineObj != null) dangerLineObj.SetActive(false);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null) pathObj.SetActive(false);

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

        // hien thi tong diem va so cap quan co da an
        if (resultScoreText != null)
        {
            int score = pairsCleared * 100;
            resultScoreText.text = $"Score: {score} | Pairs: {pairsCleared}";
        }

        // tinh toan thoi gian sinh ton theo phut va giay
        float minutes = Mathf.FloorToInt(survivalTime / 60f);
        float seconds = Mathf.FloorToInt(survivalTime % 60f);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (resultTimePlayedText != null)
        {
            resultTimePlayedText.text = "Survival: " + timeStr;
        }

        if (playAgainText != null)
        {
            playAgainText.text = "PLAY AGAIN";
        }
    }

    // ham cho button play again de choi lai van sinh ton moi
    private void OnPlayAgainClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ham cho button menu de quay ve man hinh chinh
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
