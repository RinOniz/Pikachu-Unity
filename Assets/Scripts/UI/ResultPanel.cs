using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    // bang ket qua chung
    [SerializeField] private GameObject resultPanel;

    // cac thanh phan text hien thi thong so
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private TextMeshProUGUI resultTimePlayedText;
    [SerializeField] private TextMeshProUGUI playAgainText;

    // hinh anh ngoi sao chien thang va anh game over
    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject gameOverImage;

    // cac button dieu huong sau khi ket thuc van choi
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button resultMenuButton;

    // ham khoi tao va gan su kien cho cac nut tren bang ket qua
    private void Start()
    {
        playAgainButton.onClick.AddListener(OnMainButtonClicked);
        resultMenuButton.onClick.AddListener(ReturnToMenu);
    }

    // ham hien thi bang ket qua voi trang thai thang thua, diem so va thoi gian
    public void Show(bool winStatus, int score, float playTime)
    {
        // an ban co va cac thanh phan gameplay de tap trung vao popup ket qua
        GameObject boardObj = GameObject.Find("Board");
        if (boardObj != null) boardObj.SetActive(false);

        GameObject dangerLineObj = GameObject.Find("DangerLine");
        if (dangerLineObj != null) dangerLineObj.SetActive(false);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null) pathObj.SetActive(false);

        // bat popup bang ket qua
        resultPanel.SetActive(true);

        // hien thi sao khi thang va hien thi game over khi thua
        stars.SetActive(winStatus);
        gameOverImage.SetActive(!winStatus);

        // cap nhat diem so len giao dien
        resultScoreText.text = "Score: " + score;

        // dinh dang thoi gian choi thanh phut va giay
        float minutes = Mathf.FloorToInt(playTime / 60);
        float seconds = Mathf.FloorToInt(playTime % 60);
        string timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        resultTimePlayedText.text = "Time: " + timeStr;

        // xu ly text nut bam tuy theo trang thai thang hay thua va che do choi
        if (winStatus)
        {
            int gameMode = PlayerPrefs.GetInt("GameMode", 0);

            // funny mode chi co mot level duy nhat nen hien thi you win
            if (gameMode == 1)
            {
                playAgainText.text = "YOU WIN!";
            }
            else
            {
                // kiem tra xem con level tiep theo khong
                int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);
                if (currentLevel < 3) playAgainText.text = "NEXT LEVEL";
                else playAgainText.text = "YOU WIN!";
            }
        }
        else
        {
            // neu thua thi cho phep choi lai van do
            playAgainText.text = "PLAY AGAIN";
        }
    }

    // ham cho button chinh de choi lai, sang man tiep hoac quay ve menu
    private void OnMainButtonClicked()
    {
        // xu ly khi nguoi choi bam nut play again
        if (playAgainText.text == "PLAY AGAIN")
        {
            SceneManager.LoadScene("GameScene");
        }
        // xu ly khi vuot qua man choi va chuyen sang level tiep theo
        else if (playAgainText.text == "NEXT LEVEL")
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

            PlayerPrefs.SetInt("GameLevel", currentLevel + 1);

            SceneManager.LoadScene("GameScene");
        }
        // xu ly khi da pha dao toan bo game va quay ve menu
        else if (playAgainText.text == "YOU WIN!")
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    // ham cho button menu de quay ve man hinh menu chinh
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
