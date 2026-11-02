using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSelectPanel : MonoBehaviour
{
    // cac nut bam de chon che do choi
    [SerializeField] private Button returnButton;
    [SerializeField] private Button normalModeButton;
    [SerializeField] private Button funnyModeButton;
    [SerializeField] private Button onlineModeButton;

    // panel giao dien chon che do
    [SerializeField] private GameObject modeSelectPanel;

    // tham chieu den cac panel con
    [SerializeField] private LevelSelectPanel levelSelectPanel;
    [SerializeField] private SpecialModePanel specialModePanel;

    // ham khoi tao va gan lang nghe su kien click
    private void Awake()
    {
        returnButton.onClick.AddListener(HidePanel);
        normalModeButton.onClick.AddListener(SelectNormalMode);
        funnyModeButton.onClick.AddListener(SelectFunnyMode);
        onlineModeButton.onClick.AddListener(SelectOnlineMode);
    }

    // ham hien thi bang chon che do
    public void Show()
    {
        modeSelectPanel.SetActive(true);
    }

    // ham an bang chon che do
    private void HidePanel()
    {
        modeSelectPanel.SetActive(false);
    }

    // ham cho button chon che do co dien normal mode
    private void SelectNormalMode()
    {
        // cai dat che do 0 va mo tiep bang chon level
        PlayerPrefs.SetInt("GameMode", 0);

        HidePanel();

        levelSelectPanel.Show();
    }

    // ham cho button chon che do hai huoc funny mode
    private void SelectFunnyMode()
    {
        // cai dat che do funny 1 va vao thang van choi
        PlayerPrefs.SetInt("GameMode", 1);

        HidePanel();

        PlayerPrefs.SetInt("GameLevel", 1);

        SceneManager.LoadScene("GameScene");
    }

    // ham cho button mo danh sach che do mo rong dac biet
    private void SelectOnlineMode()
    {
        HidePanel();

        // mo panel chua cac che do dac biet nhu memory, tetris, piano
        if (specialModePanel != null)
        {
            specialModePanel.Show();
        }
    }
}
