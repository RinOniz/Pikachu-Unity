using UnityEngine;
using UnityEngine.UI;

public class EndlessHint : MonoBehaviour
{
    // nut bam goi y nuoc di
    [SerializeField] private Button hintButton;

    private EndlessGameController gameController;

    // ham khoi tao va gan su kien cho nut hint
    private void Start()
    {
        gameController = GetComponent<EndlessGameController>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(OnHintClicked);
        }
    }

    // ham cho button hint de tim va hien thi cap co hop le
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
