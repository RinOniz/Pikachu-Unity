using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();

            // DontDestroyOnLoad(gameObject);

            UpdateMusicVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMusicVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
    }
}
