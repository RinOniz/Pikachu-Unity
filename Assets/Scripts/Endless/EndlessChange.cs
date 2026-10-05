using UnityEngine;
using UnityEngine.UI;

public class EndlessChange : MonoBehaviour
{
    [SerializeField] private Button changeButton;

    private EndlessGameController gameController;

    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (changeButton != null)
        {
            changeButton.onClick.AddListener(OnChangeClicked);
        }
    }

    private void OnChangeClicked()
    {
        if (gameController == null)
        {
            gameController = GetComponent<EndlessGameController>();
        }

        if (gameController != null)
        {
            gameController.ChangeBoard();
        }
    }
}
