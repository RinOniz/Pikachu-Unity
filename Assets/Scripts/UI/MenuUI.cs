using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    // cac nut bam tren giao dien menu chinh
    [SerializeField] private Button playButton;
    [SerializeField] private Button modeSelectButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button closeButton;

    // cac panel chuc nang duoc bat tat tu menu
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private GameObject tutorialPanel;

    // thanh phan phat am thanh khi tuong tac
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startGameSound;

    // ham khoi tao chay truoc frame dau tien
    private void Start()
    {
        // dang ky su kien click cho cac button
        playButton.onClick.AddListener(QuickPlayClassic);
        modeSelectButton.onClick.AddListener(OpenModeSelect);
        tutorialButton.onClick.AddListener(OpenTutorialPanel);
        closeButton.onClick.AddListener(CloseGame);
    }

    // ham update chay moi frame
    private void Update()
    {
        
    }

    // ham cho button play choi nhanh che do co dien level 1
    private void QuickPlayClassic()
    {
        PlaySound();

        // luu che do choi 0 va level 1 vao bo nho tam
        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("GameLevel", 1);

        // chuyen huong sang scene choi game
        SceneManager.LoadScene("GameScene");
    }

    // ham cho button mo bang chon che do
    private void OpenModeSelect()
    {
        modeSelectPanel.GetComponent<ModeSelectPanel>().Show();
    }

    // ham cho button mo bang huong dan
    private void OpenTutorialPanel()
    {
        tutorialPanel.GetComponent<TutorialPanel>().Show();
    }

    // ham cho button dong hoac thoat game
    private void CloseGame()
    {
        // kiem tra moi truong chay de xu ly thoat hop ly
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ham phat am thanh khi bam nut bat dau
    public void PlaySound()
    {
        audioSource.PlayOneShot(startGameSound);
    }
}
