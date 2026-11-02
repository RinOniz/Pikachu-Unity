using UnityEngine;

public class BgmManager : MonoBehaviour
{
    // instance singleton dung chung toan bo game
    public static BgmManager instance;

    // nguon phat am thanh nhac nen
    private AudioSource audioSource;

    // ham khoi tao chay khi doi tuong duoc load
    private void Awake()
    {
        // ap dung mo hinh singleton tranh duplicate nhac nen
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();

            // cap nhat am luong nhac nen tu cai dat da luu
            UpdateMusicVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ham dong bo va cap nhat am luong nhac tu playerprefs
    public void UpdateMusicVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
    }
}
