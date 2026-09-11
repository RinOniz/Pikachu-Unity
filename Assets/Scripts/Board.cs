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
        CreateBoard(ShuffleTiles());
    }

    private void PickRandomPairs()
    {
        remainingPairs = new List<int>();

        for (int i = 0; i < totalTiles / 2; i++)
        {
            int pairID = Random.Range(0, tileSprites.Length);

            remainingPairs.Add(pairID);
        }
    }

    private List<int> ShuffleTiles()
    {
        List<int> duplicatedList = new List<int>();

        for (int i = 0; i < remainingPairs.Count; i++)
        {
            duplicatedList.AddRange(new List<int> { remainingPairs[i], remainingPairs[i] });
        }

        for (int i = 0; i < duplicatedList.Count; i++)
        {
            int temp = duplicatedList[i];
            int randomId = Random.Range(i, duplicatedList.Count);

            duplicatedList[i] = duplicatedList[randomId];
            duplicatedList[randomId] = temp;
        }

        return duplicatedList;
    }

    private void CreateBoard(List<int> duplicatedList)
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
                    int valueId = duplicatedList[i];

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

    public bool CheckLine(Position startPos, Position endPos, bool drawPath)
    {
        if (startPos.row == endPos.row && CheckLineCol(startPos.row, startPos.col, endPos.col))
        {
            if (drawPath)
            {
                return true;
            }
        }

        if (startPos.col == endPos.col && CheckLineRow(startPos.col, startPos.row, endPos.row))
        {
            if (drawPath)
            {
                return true;
            }
        }

        if (idGrid[startPos.row, endPos.col] == -1 &&
            CheckLineCol(startPos.row, startPos.col, endPos.col) &&
            CheckLineRow(endPos.col, startPos.row, endPos.row))
        {
            if (drawPath)
            {
                return true;
            }
        }

        if (idGrid[endPos.row, startPos.col] == -1 &&
            CheckLineRow(startPos.col, startPos.row, endPos.row) &&
            CheckLineCol(endPos.row, startPos.col, endPos.col))
        {
            if (drawPath) 
                return true;
        }

        for (int r = 0; r < totalRows; r++)
        {
            if (r != startPos.row && r != endPos.row && idGrid[r, startPos.col] == -1 && idGrid[r, endPos.col] == -1)
            {
                if (CheckLineRow(startPos.col, startPos.row, r) &&
                    CheckLineRow(endPos.col, endPos.row, r) &&
                    CheckLineCol(r, startPos.col, endPos.col))
                {
                    if (drawPath)
                    {
                        return true;
                    }
                }
            }
        }

        for (int c = 0; c < totalCols; c++)
        {
            if (c != startPos.col && c != endPos.col && idGrid[startPos.row, c] == -1 && idGrid[endPos.row, c] == -1)
            {
                if (CheckLineCol(startPos.row, startPos.col, c) &&
                    CheckLineCol(endPos.row, endPos.col, c) &&
                    CheckLineRow(c, startPos.row, endPos.row))
                {
                    if (drawPath) 
                        return true;
                }
            }
        }

        return false;
    }
         
    private bool CheckLineRow(int col, int row1, int row2)
    {
        int min = Mathf.Min(row1, row2);
        int max = Mathf.Max(row1, row2);

        for (int r = min + 1; r < max; r++)
        {
            if (idGrid[r, col] != -1)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckLineCol(int row, int col1, int col2)
    {
        int min = Mathf.Min(col1, col2);
        int max = Mathf.Max(col1, col2);

        for (int c = min + 1; c < max; c++)
        {
            if (idGrid[row, c] != -1)
            {
                return false;
            }
        }
        return true;
    }
}
