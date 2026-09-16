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
    private GameObject hintTileOne;
    private GameObject hintTileTwo;
    private List<int> remainingPairs;
    private Path pathController;

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

        GameObject pathObj = GameObject.Find("Path");

        if (pathObj != null)
        {
            pathController = pathObj.GetComponent<Path>();
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
                //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, endPos });
                pathController.DrawPath(new List<Position> { startPos, endPos });

                return true;
            }
        }

        if (startPos.col == endPos.col && CheckLineRow(startPos.col, startPos.row, endPos.row))
        {
            if (drawPath)
            {
                //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, endPos });
                pathController.DrawPath(new List<Position> { startPos, endPos });

                return true;
            }
        }

        if (idGrid[startPos.row, endPos.col] == -1 &&
            CheckLineCol(startPos.row, startPos.col, endPos.col) &&
            CheckLineRow(endPos.col, startPos.row, endPos.row))
        {
            if (drawPath)
            {
                //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, new Position(startPos.row, endPos.col), endPos });
                pathController.DrawPath(new List<Position> { startPos, new Position(startPos.row, endPos.col), endPos });

                return true;
            }
        }

        if (idGrid[endPos.row, startPos.col] == -1 &&
            CheckLineRow(startPos.col, startPos.row, endPos.row) &&
            CheckLineCol(endPos.row, startPos.col, endPos.col))
        {
            if (drawPath) 
            {
                //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, new Position(endPos.row, startPos.col), endPos });
                pathController.DrawPath(new List<Position> { startPos, new Position(endPos.row, startPos.col), endPos });

                return true;
            }
        }

        for (int row = 0; row < totalRows; row++)
        {
            if (row != startPos.row && row != endPos.row && idGrid[row, startPos.col] == -1 && idGrid[row, endPos.col] == -1)
            {
                if (CheckLineRow(startPos.col, startPos.row, row) &&
                    CheckLineRow(endPos.col, endPos.row, row) &&
                    CheckLineCol(row, startPos.col, endPos.col))
                {
                    if (drawPath)
                    {
                        //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, new Position(row, startPos.col), new Position(row, endPos.col), endPos });
                        pathController.DrawPath(new List<Position> { startPos, new Position(row, startPos.col), new Position(row, endPos.col), endPos });

                        return true;
                    }
                }
            }
        }

        for (int col = 0; col < totalCols; col++)
        {
            if (col != startPos.col && col != endPos.col && idGrid[startPos.row, col] == -1 && idGrid[endPos.row, col] == -1)
            {
                if (CheckLineCol(startPos.row, startPos.col, col) &&
                    CheckLineCol(endPos.row, endPos.col, col) &&
                    CheckLineRow(col, startPos.row, endPos.row))
                {
                    if (drawPath)
                    {
                        //GameObject.Find("Path").GetComponent<Path>().DrawPath(new List<Position> { startPos, new Position(startPos.row, col), new Position(endPos.row, col), endPos });
                        pathController.DrawPath(new List<Position> { startPos, new Position(startPos.row, col), new Position(endPos.row, col), endPos });

                        return true;
                    }
                }
            }
        }

        return false;
    }
         
    private bool CheckLineRow(int col, int row1, int row2)
    {
        int min = Mathf.Min(row1, row2);
        int max = Mathf.Max(row1, row2);

        for (int row = min + 1; row < max; row++)
        {
            if (idGrid[row, col] != -1)
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

        for (int col = min + 1; col < max; col++)
        {
            if (idGrid[row, col] != -1)
            {
                return false;
            }
        }

        return true;
    }

    public void Clear(Position firstPos, Position secondPos)
    {
        int valueId = idGrid[firstPos.row, firstPos.col];

        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        remainingPairs.Remove(valueId);
    }

    public Position[] FindValidPairs()
    {
        Position[] adjacentPair = FindAdjacentPairs();

        if (adjacentPair != null)
        {
            Debug.Log("Lien ke");

            return adjacentPair;
        }

        Position[] edgePair = FindEdgePairs();
        if (edgePair != null)
        {
            Debug.Log("Mep");

            return edgePair;
        }

        Debug.Log("Vet can");

        return FindNormalPairs();
    }

    private Position[] FindAdjacentPairs()
    {
        for (int row = 1; row < totalRows - 1; row++)
        {
            for (int col = 1; col < totalCols - 1; col++)
            {
                int id = idGrid[row, col];

                if (id == -1)
                {
                    continue;
                }

                if (col + 1 < totalCols - 1 && idGrid[row, col + 1] == id)
                {
                    if (CheckLine(new Position(row, col), new Position(row, col + 1), false))
                    {
                        return new Position[] { new Position(row, col), new Position(row, col + 1) };
                    }
                }

                if (row + 1 < totalRows - 1 && idGrid[row + 1, col] == id)
                {
                    if (CheckLine(new Position(row, col), new Position(row + 1, col), false))
                    {
                        return new Position[] { new Position(row, col), new Position(row + 1, col) };
                    }
                }
            }
        }

        return null;
    }

    private Position[] FindEdgePairs()
    {
        List<Position> edgeTiles = new List<Position>();

        for (int row = 1; row < totalRows - 1; row++)
        {
            for (int col = 1; col < totalCols - 1; col++)
            {
                if (idGrid[row, col] != -1)
                {
                    int depth = GetLayerDepth(row, col);

                    if (depth <= 2)
                    {
                        edgeTiles.Add(new Position(row, col));
                    }
                }
            }
        }

        edgeTiles.Sort((p1, p2) =>
        {
            int depth1 = GetLayerDepth(p1.row, p1.col);
            int depth2 = GetLayerDepth(p2.row, p2.col);

            return depth1.CompareTo(depth2);
        });

        for (int i = 0; i < edgeTiles.Count; i++)
        {
            for (int j = i + 1; j < edgeTiles.Count; j++)
            {
                Position pos1 = edgeTiles[i];
                Position pos2 = edgeTiles[j];

                if (idGrid[pos1.row, pos1.col] == idGrid[pos2.row, pos2.col])
                {
                    if (CheckLine(pos1, pos2, false))
                    {
                        return new Position[] { pos1, pos2 };
                    }
                }
            }
        }

        return null;
    }

    private Position[] FindNormalPairs()
    {
        for (int row1 = 0; row1 < totalRows; row1++)
        {
            for (int col1 = 0; col1 < totalCols; col1++)
            {
                int id1 = idGrid[row1, col1];

                if (id1 == -1)
                {
                    continue;
                }

                for (int row2 = row1; row2 < totalRows; row2++)
                {
                    int startCol = (row1 == row2) ? col1 + 1 : 0;

                    for (int col2 = startCol; col2 < totalCols; col2++)
                    {
                        int id2 = idGrid[row2, col2];

                        if (id1 == id2)
                        {
                            if (CheckLine(new Position(row1, col1), new Position(row2, col2), false))
                            {
                                return new Position[] { new Position(row1, col1), new Position(row2, col2) };
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private int GetLayerDepth(int row, int col)
    {
        int distTop = row;
        int distBottom = (totalRows - 1) - row;
        int distLeft = col;
        int distRight = (totalCols - 1) - col;

        return Mathf.Min(distTop, distBottom, distLeft, distRight);
    }

    //public bool GetHint()
    //{
    //    Position[] pairs = FindValidPairs();

    //    if (pairs != null)
    //    {
    //        hintTileOne = tileObjects[pairs[0].row, pairs[0].col];
    //        hintTileTwo = tileObjects[pairs[1].row, pairs[1].col];

    //        Color color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
    //        hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    //        hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

    //        return true;
    //    }

    //    return false;
    //}
}
