using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryHint : MonoBehaviour
{
    // nut lay goi y va text tren nut
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintText;

    private int totalGetHintTime = 0;
    private MemoryGameController gameController;

    // ham khoi tao va gan lang nghe su kien click cho nut hint
    private void Start()
    {
        gameController = GetComponent<MemoryGameController>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(GetHint);
        }
    }

    // ham cho button hint tim va lam sang cap the noi duoc
    private void GetHint()
    {
        if (gameController == null)
        {
            gameController = GetComponent<MemoryGameController>();
            if (gameController == null) return;
        }

        // kiem tra trang thai tro choi truoc khi goi y
        if (gameController.IsGameOver() || gameController.IsPause() || gameController.IsProcessing() || gameController.IsPreviewing())
        {
            return;
        }

        // goi ham lay goi y va tang so lan da dung
        if (hintText != null && hintText.text == "HINT")
        {
            bool hasHint = gameController.GetHint(totalGetHintTime);
            if (hasHint)
            {
                totalGetHintTime++;
            }
        }
    }

    // ham thay doi text hien thi tren nut goi y
    public void ChangeText(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
    }
}
