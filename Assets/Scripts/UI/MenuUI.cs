using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button modeSelectButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private GameObject tutorialPanel;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startGameSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playButton.onClick.AddListener(QuickPlayClassic);
        modeSelectButton.onClick.AddListener(OpenModeSelect);
        tutorialButton.onClick.AddListener(OpenTutorialPanel);
        closeButton.onClick.AddListener(CloseGame);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void QuickPlayClassic()
    {
        PlaySound();

        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("GameLevel", 1);

        SceneManager.LoadScene("GameScene");
    }

    private void OpenModeSelect()
    {
        modeSelectPanel.GetComponent<ModeSelectPanel>().Show();
    }

    private void OpenTutorialPanel()
    {
        tutorialPanel.GetComponent<TutorialPanel>().Show();
    }

    private void CloseGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(startGameSound);
    }
}
