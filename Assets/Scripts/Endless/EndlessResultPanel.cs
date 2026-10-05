using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndlessResultPanel : MonoBehaviour
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

    public void Show(float survivalTime, int pairsCleared, int rowsPushed)
    {
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

        if (resultScoreText != null)
        {
            resultScoreText.text = $"Cleared: {pairsCleared} pairs | Rows: {rowsPushed}";
        }

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

    private void OnPlayAgainClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
