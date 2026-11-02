using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // cac thanh phan giao dien cua nut tam dung
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    // ham khoi tao va gan su kien cho nut pause
    private void Start()
    {
        pauseButton.onClick.AddListener(SetPauseGame);
    }

    // ham cho button pause de bat hoac tat trang thai tam dung game
    private void SetPauseGame()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        // neu chua tam dung thi chuyen sang trang thai tam dung
        if (!gameController.IsPause())
        {
            gameController.SetPause(true);
        }
        // neu dang tam dung thi tiep tuc tro choi va phat am thanh
        else
        {
            gameController.SetPause(false);
            gameController.PlayStartSound();
        }
    }

    // ham update cap nhat hinh anh va chu cua nut theo trang thai
    private void Update()
    {
        // cap nhat giao dien khi game dang chay binh thuong
        if (!GetComponent<GameController>().IsPause())
        {
            pauseButton.GetComponent<Image>().sprite = pauseImage;
            pauseText.text = "PAUSE";
        }
        // cap nhat giao dien khi game dang bi tam dung
        else
        {
            pauseButton.GetComponent<Image>().sprite = resumeImage;
            pauseText.text = "RESUME";
        }
    }
}
