using TMPro;
using UnityEngine;

public class MemoryRoundCountdown : MonoBehaviour
{
    // text hien thi thoi gian dem nguoc
    [SerializeField] private TextMeshProUGUI roundCountdownText;
    [SerializeField] private int roundTime = 300;

    private float roundTimeLeft;
    private MemoryGameController gameController;

    // ham khoi tao thoi gian man choi tri nho
    private void Start()
    {
        roundTimeLeft = (float)roundTime;
        gameController = GetComponent<MemoryGameController>();
    }

    // ham update cap nhat thoi gian dem nguoc moi frame
    private void Update()
    {
        if (gameController == null)
        {
            gameController = GetComponent<MemoryGameController>();
            if (gameController == null) return;
        }

        // dung dem thoi gian khi game tam dung, ket thuc hoac dang trinh chieu
        if (gameController.IsPause() || gameController.IsGameOver() || gameController.IsPreviewing())
        {
            return;
        }

        float currentTime = 0.0f;

        // tru thoi gian troi qua
        if (roundTimeLeft > 0)
        {
            roundTimeLeft -= Time.deltaTime;
            currentTime = roundTimeLeft > 0 ? roundTimeLeft : 0.0f;

            // neu het gio thi bao thua cuoc
            if (currentTime == 0.0f)
            {
                gameController.SetGameOver();
            }
        }

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        // doi mau chu theo thoi gian con lai
        if (roundCountdownText != null)
        {
            if (currentTime <= 30f)
            {
                roundCountdownText.color = Color.red;
            }
            else if (currentTime > (float)roundTime * 0.6f)
            {
                roundCountdownText.color = new Color(47f / 255f, 154f / 255f, 8f / 255f, 1f);
            }
            else
            {
                roundCountdownText.color = new Color(217f / 255f, 152f / 255f, 0f, 1f);
            }

            roundCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // ham lay thoi gian da choi trong van
    public float GetRemainTime()
    {
        return (float)roundTime - roundTimeLeft;
    }
}
