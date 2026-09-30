using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSelectPanel : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button normalModeButton;
    [SerializeField] private Button funnyModeButton;
    [SerializeField] private Button onlineModeButton;

    [SerializeField] private GameObject modeSelectPanel;

    [SerializeField] private LevelSelectPanel levelSelectPanel;

    private void Start()
    {
        returnButton.onClick.AddListener(HidePanel);
        normalModeButton.onClick.AddListener(SelectNormalMode);
        funnyModeButton.onClick.AddListener(SelectFunnyMode);
        onlineModeButton.onClick.AddListener(SelectOnlineMode);
    }

    public void Show()
    {
        modeSelectPanel.SetActive(true);
    }

    private void HidePanel()
    {
        modeSelectPanel.SetActive(false);
    }

    private void SelectNormalMode()
    {
        PlayerPrefs.SetInt("GameMode", 0);

        HidePanel();

        levelSelectPanel.Show();
    }

    private void SelectFunnyMode()
    {
        //menuScript.PlaySound();

        //SceneManager.LoadScene("GameScene");
        //PlayerPrefs.SetInt("GameMode", 1);
    }

    private void SelectOnlineMode()
    {
        //menuScript.PlaySound();
        //SceneManager.LoadScene("GameScene");
        //PlayerPrefs.SetInt("GameMode", 2);
    }
}
