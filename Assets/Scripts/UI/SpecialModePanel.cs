using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpecialModePanel : MonoBehaviour
{
    [SerializeField] private Button memoryButton;
    [SerializeField] private Button tetrisButton;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private TetrisModePanel tetrisModePanel;

    [SerializeField] private MenuUI menuScript;

    private void Awake()
    {
        if (memoryButton != null)
        {
            memoryButton.onClick.AddListener(SelectMemoryMode);
        }

        if (tetrisButton != null)
        {
            tetrisButton.onClick.AddListener(SelectTetrisMode);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackToModeSelect);
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

    private void SelectMemoryMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        PlayerPrefs.SetInt("GameMode", 2);
        PlayerPrefs.SetInt("GameLevel", 1);

        SceneManager.LoadScene("MemoryGameScene");
    }

    private void SelectTetrisMode()
    {
        if (menuScript != null)
        {
            menuScript.PlaySound();
        }

        Hide();

        if (tetrisModePanel != null)
        {
            tetrisModePanel.Show();
        }
    }

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
