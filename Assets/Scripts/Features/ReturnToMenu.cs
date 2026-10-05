using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] private Button returnToMenuButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        returnToMenuButton.onClick.AddListener(ReturnToMenuScene);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void ReturnToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
