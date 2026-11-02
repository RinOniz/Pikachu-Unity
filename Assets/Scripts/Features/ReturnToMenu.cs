using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    // nut bam de quay tro ve man hinh menu
    [SerializeField] private Button returnToMenuButton;

    // ham khoi tao va gan su kien click cho nut return
    private void Start()
    {
        returnToMenuButton.onClick.AddListener(ReturnToMenuScene);
    }

    // ham update chay moi frame
    private void Update()
    {
        
    }

    // ham cho button return de tai lai scene menu chinh
    private void ReturnToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
