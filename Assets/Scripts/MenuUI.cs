using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playButton.onClick.AddListener(QuickPlayClassic);
        closeButton.onClick.AddListener(CloseGame);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void QuickPlayClassic()
    {
        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("GameLevel", 1);

        SceneManager.LoadScene("GameScene");
    }

    private void CloseGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
