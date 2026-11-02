using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // cac thanh truot dieu chinh am luong va panel cai dat
    [Header("UI Elements")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject settingsPanel;

    // ham khoi tao gia tri thanh truot tu cau hinh da luu
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // ham cho button apply de luu cau hinh am thanh va ap dung
    public void OnApplyButtonClicked()
    {
        // luu thong so am luong vao bo nho may
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        PlayerPrefs.Save();

        // ap dung am luong moi cho he thong
        ApplySettingsToGame();

        // dong bang cai dat
        settingsPanel.SetActive(false);
    }

    // ham ap dung thay doi am luong len bo quan ly nhac nen
    private void ApplySettingsToGame()
    {
        if (BgmManager.instance != null)
        {
            BgmManager.instance.UpdateMusicVolume();
        }
    }
}
