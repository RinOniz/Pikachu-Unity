using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
    // cac button chon cap do choi
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button backButton;

    // tham chieu den panel che do de quay ve
    [SerializeField] private GameObject modeSelectPanel;

    // script menu de phat am thanh khi bam
    [SerializeField] private MenuUI menuScript;

    // ham khoi tao va gan su kien cho cac nut chon level
    private void Start()
    {
        // lang nghe su kien click vao tung button level
        level1Button.onClick.AddListener(() => LoadGameWithLevel(1));
        level2Button.onClick.AddListener(() => LoadGameWithLevel(2));
        level3Button.onClick.AddListener(() => LoadGameWithLevel(3));

        // gan su kien quay ve cho button back
        backButton.onClick.AddListener(GoBackToModeSelect);
    }

    // ham hien thi bang chon level
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // ham an bang chon level
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // ham cho cac button level de tai game voi cap do tuong ung
    private void LoadGameWithLevel(int levelNumber)
    {
        menuScript.PlaySound();

        // luu so thu tu level vao playerprefs
        PlayerPrefs.SetInt("GameLevel", levelNumber);

        // nap scene choi game co dien
        SceneManager.LoadScene("GameScene");
    }

    // ham cho button back quay ve bang chon che do
    private void GoBackToModeSelect()
    {
        Hide();

        // bat lai bang chon che do
        modeSelectPanel.SetActive(true);
    }
}
