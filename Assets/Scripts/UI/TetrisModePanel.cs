using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TetrisModePanel : MonoBehaviour
{
    // cac button tuy chon co che trong luc trong tetris
    [SerializeField] private Button noGravityButton;
    [SerializeField] private Button gravityButton;
    [SerializeField] private Button backButton;

    // tham chieu den bang special mode de quay ve
    [SerializeField] private SpecialModePanel specialModePanel;

    // script menu chua am thanh tuong tac
    [SerializeField] private MenuUI menuScript;

    // ham khoi tao va gan lang nghe su kien click
    private void Awake()
    {
        // gan su kien cho button che do khong trong luc
        if (noGravityButton != null)
        {
            noGravityButton.onClick.AddListener(SelectNoGravity);
        }

        // gan su kien cho button che do co trong luc tut xuong
        if (gravityButton != null)
        {
            gravityButton.onClick.AddListener(SelectGravity);
        }

        // gan su kien cho button back quay lai
        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackToSpecialMode);
        }
    }

    // ham hien thi bang tuy chon tetris
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // ham an bang tuy chon tetris
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // ham cho button chon che do tetris khong trong luc
    private void SelectNoGravity()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        // cai dat che do tetris 3 va khong trong luc 0
        PlayerPrefs.SetInt("GameMode", 3);
        PlayerPrefs.SetInt("TetrisGravity", 0); // 0 = no gravity giu nguyen khoang trong khi an

        // nap scene sinh ton vo tan endless
        SceneManager.LoadScene("EndlessGameScene");
    }

    // ham cho button chon che do tetris co trong luc
    private void SelectGravity()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        // cai dat che do tetris 3 va co trong luc 1
        PlayerPrefs.SetInt("GameMode", 3);
        PlayerPrefs.SetInt("TetrisGravity", 1); // 1 = gravity cac o phia tren tut xuong lap cho trong

        // nap scene sinh ton vo tan endless
        SceneManager.LoadScene("EndlessGameScene");
    }

    // ham cho button back quay ve bang special mode
    private void GoBackToSpecialMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        Hide();

        if (specialModePanel != null)
        {
            specialModePanel.Show();
        }
    }
}
