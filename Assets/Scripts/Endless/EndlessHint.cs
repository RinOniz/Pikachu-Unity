using UnityEngine;
using UnityEngine.UI;

public class EndlessHint : MonoBehaviour
{
    [SerializeField] private Button hintButton;

    private EndlessGameController gameController;

    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(OnHintClicked);
        }
    }

    private void OnHintClicked()
    {
        if (gameController == null)
        {
            gameController = GetComponent<EndlessGameController>();
        }

        if (gameController != null)
        {
            gameController.GetHint();
        }
    }
}
