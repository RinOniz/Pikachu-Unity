using UnityEngine;
using UnityEngine.UI;

public class MemoryChange : MonoBehaviour
{
    // nut bam dao vi tri the bai
    [SerializeField] private Button changeButton;

    private int totalChanges = 0;
    private MemoryGameController gameController;
    private MemoryBoard board;
    private MemoryHint hint;

    // ham khoi tao va gan su kien click cho nut change
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

    // ham cho button change dao lai vi tri cac the bai tren ban
    private void ChangeBoard()
    {
        if (gameController == null) gameController = GetComponent<MemoryGameController>();
        if (board == null) board = GetComponent<MemoryBoard>();
        if (hint == null) hint = GetComponent<MemoryHint>();

        // bo qua khi game over, pause hoac dang xu ly
        if (gameController != null && (gameController.IsGameOver() || gameController.IsPause() || gameController.IsProcessing() || gameController.IsPreviewing()))
        {
            return;
        }

        // dao vi tri toan bo the bai
        if (board != null)
        {
            board.Change();
        }

        // reset chu tren nut goi y
        if (hint != null)
        {
            hint.ChangeText("HINT");
        }

        // tru diem phat va phat am thanh
        if (gameController != null)
        {
            gameController.MinusChangeScore(totalChanges);
            gameController.PlayStartSound();
        }

        totalChanges++;
    }
}
