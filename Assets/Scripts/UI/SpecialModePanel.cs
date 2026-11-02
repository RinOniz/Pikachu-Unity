using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpecialModePanel : MonoBehaviour
{
    // cac button chon cac che do dac biet
    [SerializeField] private Button memoryButton;
    [SerializeField] private Button tetrisButton;
    [SerializeField] private Button pianoButton;
    [SerializeField] private Button backButton;

    // panel che do va panel tetris de chuyen doi
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private TetrisModePanel tetrisModePanel;

    // script menu chua ham phat am thanh
    [SerializeField] private MenuUI menuScript;

    // ham khoi tao chay khi doi tuong duoc danh thuc
    private void Awake()
    {
        // gan su kien click cho button che do tri nho
        if (memoryButton != null)
        {
            memoryButton.onClick.AddListener(SelectMemoryMode);
        }

        // gan su kien click cho button che do tetris
        if (tetrisButton != null)
        {
            tetrisButton.onClick.AddListener(SelectTetrisMode);
        }

        // gan su kien click cho button che do piano tiles
        if (pianoButton != null)
        {
            pianoButton.onClick.AddListener(SelectPianoMode);
        }

        // gan su kien click cho button back
        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackToModeSelect);
        }
    }

    // ham hien thi bang che do dac biet
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // ham an bang che do dac biet
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // ham cho button chon che do tri nho memory
    private void SelectMemoryMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        // luu che do 2 memory vao playerprefs
        PlayerPrefs.SetInt("GameMode", 2);
        PlayerPrefs.SetInt("GameLevel", 1);

        // nap scene che do tri nho
        SceneManager.LoadScene("MemoryGameScene");
    }

    // ham cho button chon che do roi tu do tetris
    private void SelectTetrisMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        // an bang hien tai va mo bang tuy chon tetris
        Hide();

        if (tetrisModePanel != null)
        {
            tetrisModePanel.Show();
        }
    }

    // ham cho button chon che do ban phim piano tiles
    private void SelectPianoMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        // luu che do 4 piano va chuyen scene
        PlayerPrefs.SetInt("GameMode", 4);
        SceneManager.LoadScene("PianoGameScene");
    }

    // ham cho button back quay lai bang chon che do chinh
    private void GoBackToModeSelect()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        Hide();

        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(true);
        }
    }
}
