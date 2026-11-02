using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    // nen vien cua quan co de doi mau khi chon
    [SerializeField] private GameObject tileBackground;

    // toa do hang, cot va dinh danh pokemon
    public int row, col, id;

    // ham xu ly su kien khi nguoi choi click chuot vao quan co
    public void OnMouseDown()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        SpriteRenderer tileRenderer = tileBackground.GetComponent<SpriteRenderer>();

        // bo qua su kien click neu game dang tam dung hoac da ket thuc
        if (gameController.IsPause() || gameController.IsGameOver())
        {
            return;
        }

        // neu da co quan co thu nhat duoc chon truoc do
        if (gameController.IsSelected())
        {
            // tranh chon lai chinh quan co dau tien
            if (gameController.GetFirstTile() != gameObject)
            {
                // doi mau vien quan co thu hai
                tileRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                
                // gui quan co thu hai den game controller de so sanh
                gameController.SelectSecondTile(gameObject);
            }
        }
        // neu chua chon quan co nao thi day la quan co thu nhat
        else
        {
            // doi mau vien quan co thu nhat de bao hieu da chon
            tileRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);

            // luu lai quan co thu nhat vao game controller
            gameController.SelectFirstTile(gameObject);
        }
    }
}
