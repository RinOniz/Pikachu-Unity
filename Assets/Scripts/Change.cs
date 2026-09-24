using UnityEngine;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    [SerializeField] private Button changeButton;

    private int totalChanges = 0;

    private void Start()
    {
        changeButton.onClick.AddListener(ChangeBoard);
    }

    private void ChangeBoard()
    {
        if (GetComponent<GameController>().IsGameOver() || GetComponent<GameController>().IsPause())
        {
            return;
        }

        GetComponent<Board>().Change();
        GetComponent<Hint>().ChangeText("HINT");
        GetComponent<GameController>().MinusChangeScore(totalChanges);
        //GetComponent<GameController>().PlayStartSound();

        totalChanges++;
    }
}
