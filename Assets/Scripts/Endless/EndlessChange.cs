using UnityEngine;
using UnityEngine.UI;

public class EndlessChange : MonoBehaviour
{
    // nut bam dao vi tri cac quan co
    [SerializeField] private Button changeButton;

    private EndlessGameController gameController;

    // ham khoi tao va gan lang nghe su kien cho nut change
    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (changeButton != null)
        {
            changeButton.onClick.AddListener(OnChangeClicked);
        }
    }

    // ham cho button change de thuc hien dao lai vi tri cac quan co tren ban
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
