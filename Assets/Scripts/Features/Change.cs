using UnityEngine;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    // nut bam doi vi tri cac quan co tren ban
    [SerializeField] private Button changeButton;

    // dem tong so lan nguoi choi da su dung tro giup doi co
    private int totalChanges = 0;

    // ham khoi tao va gan su kien cho nut change
    private void Start()
    {
        changeButton.onClick.AddListener(ChangeBoard);
    }

    // ham cho button change de dao lai vi tri toan bo quan co tren ban
    private void ChangeBoard()
    {
        // neu game da ket thuc hoac dang tam dung thi khong cho doi
        if (GetComponent<GameController>().IsGameOver() || GetComponent<GameController>().IsPause())
        {
            return;
        }

        // thuc hien dao vi tri cac quan co
        GetComponent<Board>().Change();

        // reset lai text cua nut goi y
        GetComponent<Hint>().ChangeText("HINT");

        // tru diem phat theo so lan su dung tro giup
        GetComponent<GameController>().MinusChangeScore(totalChanges);

        // phat am thanh hieu ung bat dau dao co
        GetComponent<GameController>().PlayStartSound();

        // tang so lan da su dung chuc nang doi vi tri
        totalChanges++;
    }
}
