using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryHint : MonoBehaviour
{
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintText;

    private int totalGetHintTime = 0;
    private MemoryGameController gameController;

    private void Start()
    {
        gameController = GetComponent<MemoryGameController>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(GetHint);
        }
    }

    private void GetHint()
    {
        if (gameController == null)
        {
            gameController = GetComponent<MemoryGameController>();
            if (gameController == null) return;
        }

        if (gameController.IsGameOver() || gameController.IsPause() || gameController.IsProcessing() || gameController.IsPreviewing())
        {
            return;
        }

        if (hintText != null && hintText.text == "HINT")
        {
            bool hasHint = gameController.GetHint(totalGetHintTime);
            if (hasHint)
            {
                totalGetHintTime++;
            }
        }
    }

    public void ChangeText(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
    }
}
