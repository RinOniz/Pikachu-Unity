using TMPro;
using UnityEngine;

public class MemoryRoundCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundCountdownText;
    [SerializeField] private int roundTime = 300;

    private float roundTimeLeft;
    private MemoryGameController gameController;

    private void Start()
    {
        roundTimeLeft = (float)roundTime;
        gameController = GetComponent<MemoryGameController>();
    }

    private void Update()
    {
        if (gameController == null)
        {
            gameController = GetComponent<MemoryGameController>();
            if (gameController == null) return;
        }

        if (gameController.IsPause() || gameController.IsGameOver() || gameController.IsPreviewing())
        {
            return;
        }

        float currentTime = 0.0f;

        if (roundTimeLeft > 0)
        {
            roundTimeLeft -= Time.deltaTime;
            currentTime = roundTimeLeft > 0 ? roundTimeLeft : 0.0f;

            if (currentTime == 0.0f)
            {
                gameController.SetGameOver();
            }
        }

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

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

    public float GetRemainTime()
    {
        return (float)roundTime - roundTimeLeft;
    }
}
