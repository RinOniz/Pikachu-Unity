using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject tileBackground;

    public int row, col, id;

    public void OnMouseDown()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();

        SpriteRenderer tileRenderer = tileBackground.GetComponent<SpriteRenderer>();

        if (gameController.IsSelected())
        {
            if (gameController.GetFirstTile() != gameObject)
            {
                tileRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);
                
                gameController.SelectSecondTile(gameObject);

                //Debug.Log("Selected second tile: " + gameObject.name);
            }
        }
        else
        {
            tileRenderer.color = new Color(234f / 255f, 150f / 255f, 150f / 255f, 1.0f);

            gameController.SelectFirstTile(gameObject);

            //Debug.Log("Selected first tile: " + gameObject.name);
        }
    }
}
