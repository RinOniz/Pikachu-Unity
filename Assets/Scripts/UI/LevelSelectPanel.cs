using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject modeSelectPanel;

    [SerializeField] private MenuUI menuScript;

    private void Start()
    {
        level1Button.onClick.AddListener(() => LoadGameWithLevel(1));
        level2Button.onClick.AddListener(() => LoadGameWithLevel(2));
        level3Button.onClick.AddListener(() => LoadGameWithLevel(3));

        backButton.onClick.AddListener(GoBackToModeSelect);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LoadGameWithLevel(int levelNumber)
    {
        menuScript.PlaySound();

        PlayerPrefs.SetInt("GameLevel", levelNumber);

        SceneManager.LoadScene("GameScene");
    }

    private void GoBackToModeSelect()
    {
        Hide();

        modeSelectPanel.SetActive(true);
    }
}
