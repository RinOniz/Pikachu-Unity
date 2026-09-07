using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite[] classicSprites;
    
    private Sprite[] tileSprites;
    private GameObject[,] tileObjects;
    private List<int> remainingPairs;

    public static int totalTiles = 8 * 16;

    private const int totalRows = 10;
    private const int totalCols = 18;
    private int[,] idGrid;

    private float startX = -9.2f;
    private float startY = 4.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        int mode = PlayerPrefs.GetInt("GameMode", 0);

        if (mode == 0)
        {
            classicSprites = Resources.LoadAll<Sprite>("Pokemon");
            tileSprites = classicSprites;
        }
        else
        {
            classicSprites = Resources.LoadAll<Sprite>("Pokemon");
            tileSprites = classicSprites;
        }

        PickRandomPairs();
        CreateBoard();
    }

    private void PickRandomPairs()
    {
        remainingPairs = new List<int>();

        for (int i = 0; i < totalTiles; i++)
        {
            int pairID = Random.Range(0, tileSprites.Length);

            remainingPairs.Add(pairID);
        }
    }

    private void CreateBoard()
    {
        tileObjects = new GameObject[totalRows, totalCols];
        idGrid = new int[totalRows, totalCols];

        int i = 0;

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                if (row == 0 || row == totalRows - 1 || col == 0 || col == totalCols - 1)
                {
                    tileObjects[row, col] = null;
                    idGrid[row, col] = -1;
                }
                else
                {
                    int valueId = remainingPairs[i];

                    Vector3 position = new Vector3(startX + col, startY - row, 0.0f);

                    GameObject newTileObj = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
                    newTileObj.name = "Tile[" + row + ", " + col + "]";
                    newTileObj.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
                    newTileObj.transform.SetParent(board.transform);

                    Tile tileData = newTileObj.GetComponent<Tile>();
                    tileData.row = row;
                    tileData.col = col;
                    tileData.id = valueId;

                    SpriteRenderer spriteRenderer = newTileObj.GetComponent<SpriteRenderer>();
                    spriteRenderer.sortingOrder = 2;
                    spriteRenderer.sprite = tileSprites[tileData.id];

                    tileObjects[row, col] = newTileObj;
                    idGrid[row, col] = valueId;

                    i++;
                }
            }
        }
    }
}
