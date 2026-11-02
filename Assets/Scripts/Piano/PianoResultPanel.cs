using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PianoResultPanel : MonoBehaviour
{
    // bang ket qua chung
    [SerializeField] private GameObject resultPanel;

    // cac text hien thi thong so diem, so not bat duoc va thoi gian
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultTimePlayedText;
    [SerializeField] private TextMeshProUGUI playAgainText;

    // cac doi tuong trang tri
    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject gameOverImage;

    // cac button dieu huong
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button resultMenuButton;

    // ham khoi tao va gan lang nghe su kien cho cac nut
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

    // ham hien thi bang ket qua man choi piano tiles
    public void Show(int score, int notesHit, float playTime)
    {
        // an tat ca cac thanh phan game phia sau de tap trung vao popup
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

        // cap nhat diem va so not bat trung len giao dien
        if (resultScoreText != null)
        {
            resultScoreText.text = $"Score: {score} | Hits: {notesHit}";
        }

        // dinh dang thoi gian theo phut va giay
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

    // ham cho button play again de choi lai van piano tiles
    private void OnPlayAgainClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ham cho button menu de quay ve man hinh menu chinh
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
