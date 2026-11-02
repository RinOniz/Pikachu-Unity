using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    // nut bam lay goi y va text hien thi tren nut
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintText;

    // dem so lan nguoi choi da lay goi y de tinh diem tru
    private int totalGetHintTime = 0;

    // ham khoi tao va gan lang nghe su kien click nut hint
    private void Start()
    {
        hintButton.onClick.AddListener(GetHint);
    }

    // ham cho button hint de tim va lam noi bat cap co hop le
    private void GetHint()
    {
        // kiem tra trang thai game over hoac pause truoc khi goi y
        if (GetComponent<GameController>().IsGameOver() || GetComponent<GameController>().IsPause())
        {
            return;
        }

        // chi thuc hien khi nut dang o trang thai san sang goi y
        if (hintText.text == "HINT")
        {
            bool hasHint = GetComponent<GameController>().GetHint(totalGetHintTime);

            // neu tim thay cap goi y thi tang so lan dem
            if (hasHint)
            {
                totalGetHintTime++;
            }
        }
    }

    // ham thay doi noi dung chu hien thi tren nut goi y
    public void ChangeText(string text)
    {
        hintText.text = text;
    }
}
