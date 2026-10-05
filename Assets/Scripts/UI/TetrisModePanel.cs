using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TetrisModePanel : MonoBehaviour
{
    [SerializeField] private Button noGravityButton;
    [SerializeField] private Button gravityButton;
    [SerializeField] private Button backButton;

    [SerializeField] private SpecialModePanel specialModePanel;

    [SerializeField] private MenuUI menuScript;

    private void Awake()
    {
        if (noGravityButton != null)
        {
            noGravityButton.onClick.AddListener(SelectNoGravity);
        }

        if (gravityButton != null)
        {
            gravityButton.onClick.AddListener(SelectGravity);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackToSpecialMode);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SelectNoGravity()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        PlayerPrefs.SetInt("GameMode", 3);
        PlayerPrefs.SetInt("TetrisGravity", 0); // 0 = NoGravity (giữ nguyên khoảng trống khi ăn)

        SceneManager.LoadScene("EndlessGameScene");
    }

    private void SelectGravity()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        PlayerPrefs.SetInt("GameMode", 3);
        PlayerPrefs.SetInt("TetrisGravity", 1); // 1 = Gravity (các ô phía trên tụt xuống lấp chỗ trống)

        SceneManager.LoadScene("EndlessGameScene");
    }

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
