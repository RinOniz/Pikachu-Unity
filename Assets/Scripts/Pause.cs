using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Sprite pauseImage;
    [SerializeField] private Sprite resumeImage;

    private void Start()
    {
        pauseButton.onClick.AddListener(SetPauseGame);
    }

    private void SetPauseGame()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if (!gameController.IsPause())
        {
            gameController.SetPause(true);
        }
        else
        {
            gameController.SetPause(false);
            //gameController.PlayStartSound();
        }
    }

    private void Update()
    {
        if (!GetComponent<GameController>().IsPause())
        {
            pauseButton.GetComponent<Image>().sprite = pauseImage;
            pauseText.text = "PAUSE";
        }
        else
        {
            pauseButton.GetComponent<Image>().sprite = resumeImage;
            pauseText.text = "RESUME";
        }
    }
}
