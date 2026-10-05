using UnityEngine;
using UnityEngine.UI;

public class MemoryChange : MonoBehaviour
{
    [SerializeField] private Button changeButton;

    private int totalChanges = 0;
    private MemoryGameController gameController;
    private MemoryBoard board;
    private MemoryHint hint;

    private void Start()
    {
        gameController = GetComponent<MemoryGameController>();
        board = GetComponent<MemoryBoard>();
        hint = GetComponent<MemoryHint>();

        if (changeButton != null)
        {
            changeButton.onClick.AddListener(ChangeBoard);
        }
    }

    private void ChangeBoard()
    {
        if (gameController == null) gameController = GetComponent<MemoryGameController>();
        if (board == null) board = GetComponent<MemoryBoard>();
        if (hint == null) hint = GetComponent<MemoryHint>();

        if (gameController != null && (gameController.IsGameOver() || gameController.IsPause() || gameController.IsProcessing() || gameController.IsPreviewing()))
        {
            return;
        }

        if (board != null)
        {
            board.Change();
        }

        if (hint != null)
        {
            hint.ChangeText("HINT");
        }

        if (gameController != null)
        {
            gameController.MinusChangeScore(totalChanges);
            gameController.PlayStartSound();
        }

        totalChanges++;
    }
}
