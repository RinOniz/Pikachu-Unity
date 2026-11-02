using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryBoard : MonoBehaviour
{
    // cac gameobject ban co va prefab the bai
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;

    // tap sprite hinh anh
    [SerializeField] private Sprite[] classicSprites;
    [SerializeField] private Sprite[] funnySprites;

    private Sprite[] tileSprites;
    private MemoryTile[,] tileObjects;
    private MemoryTile hintTileOne;
    private MemoryTile hintTileTwo;
    private List<int> remainingPairs;
    private Path pathController;

    // tong so the tren ban choi
    public static int totalTiles = 8 * 16;

    // kich thuoc ma tran luoi bao gom vien dem 10x18
    private const int totalRows = 10;
    private const int totalCols = 18;
    private int[,] idGrid;

    // toa do goc khoi tao
    private float startX = -9.2f;
    private float startY = 4.5f;

    // coroutine tu dong tat goi y sau thoi gian ngan
    private Coroutine hintCoroutine;

    // ham khoi tao nap sprite va sinh ban co the bai
    private void Start()
    {
        // nap sprite pokemon hoac funny tu thu muc resources
        classicSprites = Resources.LoadAll<Sprite>("Pokemon");
        tileSprites = classicSprites;

        if (tileSprites == null || tileSprites.Length == 0)
        {
            funnySprites = Resources.LoadAll<Sprite>("Funny");
            tileSprites = funnySprites;
        }

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null)
        {
            pathController = pathObj.GetComponent<Path>();
        }

        // chon cap ngau nhien va sinh the bai
        PickRandomPairs();
        CreateBoard(ShuffleTiles());
    }

    // ham chon ngau nhien cac cap the cho van dau
    private void PickRandomPairs()
    {
        remainingPairs = new List<int>();

        for (int i = 0; i < totalTiles / 2; i++)
        {
            int pairID = Random.Range(0, tileSprites.Length);
            remainingPairs.Add(pairID);
        }
    }

    // ham nhan doi moi the thanh mot cap va xao tron
    private List<int> ShuffleTiles()
    {
        List<int> duplicatedList = new List<int>();

        for (int i = 0; i < remainingPairs.Count; i++)
        {
            duplicatedList.AddRange(new List<int> { remainingPairs[i], remainingPairs[i] });
        }

        // thuat toan xao tron vi tri ngau nhien
        for (int i = 0; i < duplicatedList.Count; i++)
        {
            int temp = duplicatedList[i];
            int randomId = Random.Range(i, duplicatedList.Count);

            duplicatedList[i] = duplicatedList[randomId];
            duplicatedList[randomId] = temp;
        }

        return duplicatedList;
    }

    // ham khoi tao ma tran va sinh cac gameobject the bai
    private void CreateBoard(List<int> duplicatedList)
    {
        tileObjects = new MemoryTile[totalRows, totalCols];
        idGrid = new int[totalRows, totalCols];

        int i = 0;

        // duyet qua toan bo luoi ma tran de tao the
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                // cac o vien ngoai luon la o trong
                if (row == 0 || row == totalRows - 1 || col == 0 || col == totalCols - 1)
                {
                    tileObjects[row, col] = null;
                    idGrid[row, col] = -1;
                }
                // cac o ben trong duoc gan du lieu va the bai
                else
                {
                    int valueId = duplicatedList[i];
                    Vector3 position = new Vector3(startX + col, startY - row, 0.0f);

                    GameObject newTileObj = Instantiate(tilePrefab, position, Quaternion.identity);
                    newTileObj.name = "Tile[" + row + ", " + col + "]";
                    newTileObj.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
                    newTileObj.transform.SetParent(board.transform);

                    MemoryTile tileData = newTileObj.GetComponent<MemoryTile>();
                    tileData.Initialize(row, col, valueId, tileSprites[valueId]);

                    tileObjects[row, col] = tileData;
                    idGrid[row, col] = valueId;

                    i++;
                }
            }
        }
    }

    // ham kiem tra duong noi hop le toi da 2 khuc gap giua hai the
    public bool CheckLine(Position startPos, Position endPos, bool drawPath)
    {
        // truong hop 1: noi thang theo chieu ngang chu i
        if (startPos.row == endPos.row && CheckLineCol(startPos.row, startPos.col, endPos.col))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, endPos });
            }
            return true;
        }

        // truong hop 2: noi thang theo chieu doc chu i
        if (startPos.col == endPos.col && CheckLineRow(startPos.col, startPos.row, endPos.row))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, endPos });
            }
            return true;
        }

        // truong hop 3: noi 1 khuc gap chu l qua goc ngang doc
        if (idGrid[startPos.row, endPos.col] == -1 &&
            CheckLineCol(startPos.row, startPos.col, endPos.col) &&
            CheckLineRow(endPos.col, startPos.row, endPos.row))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, new Position(startPos.row, endPos.col), endPos });
            }
            return true;
        }

        // truong hop 4: noi 1 khuc gap chu l qua goc doc ngang
        if (idGrid[endPos.row, startPos.col] == -1 &&
            CheckLineRow(startPos.col, startPos.row, endPos.row) &&
            CheckLineCol(endPos.row, startPos.col, endPos.col))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, new Position(endPos.row, startPos.col), endPos });
            }
            return true;
        }

        // truong hop 5: noi 2 khuc gap chu z hoac chu u theo hang ngang mo rong
        for (int row = 0; row < totalRows; row++)
        {
            if (row != startPos.row && row != endPos.row && idGrid[row, startPos.col] == -1 && idGrid[row, endPos.col] == -1)
            {
                if (CheckLineRow(startPos.col, startPos.row, row) &&
                    CheckLineRow(endPos.col, endPos.row, row) &&
                    CheckLineCol(row, startPos.col, endPos.col))
                {
                    if (drawPath && pathController != null)
                    {
                        pathController.DrawPath(new List<Position> { startPos, new Position(row, startPos.col), new Position(row, endPos.col), endPos });
                    }
                    return true;
                }
            }
        }

        // truong hop 6: noi 2 khuc gap chu z hoac chu u theo cot doc mo rong
        for (int col = 0; col < totalCols; col++)
        {
            if (col != startPos.col && col != endPos.col && idGrid[startPos.row, col] == -1 && idGrid[endPos.row, col] == -1)
            {
                if (CheckLineCol(startPos.row, startPos.col, col) &&
                    CheckLineCol(endPos.row, endPos.col, col) &&
                    CheckLineRow(col, startPos.row, endPos.row))
                {
                    if (drawPath && pathController != null)
                    {
                        pathController.DrawPath(new List<Position> { startPos, new Position(startPos.row, col), new Position(endPos.row, col), endPos });
                    }
                    return true;
                }
            }
        }

        return false;
    }

    // ham kiem tra doan duong doc co bi can tro boi o nao khong
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

    // ham kiem tra doan duong ngang co bi can tro boi o nao khong
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

    // ham xoa the bai khi an dung va xu ly co che don o theo level
    public void Clear(Position firstPos, Position secondPos)
    {
        int valueID = idGrid[firstPos.row, firstPos.col];

        // cap nhat ma tran ve o trong
        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        StopHint();

        remainingPairs.Remove(valueID);

        int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

        // don the xuong duoi neu o level 2
        if (currentLevel == 2)
        {
            ShiftDown();
        }
        // don the len tren neu o level 3
        else if (currentLevel == 3)
        {
            ShiftUp();
        }

        // neu het nuoc di hop le thi tu dong dao vi tri the bai
        if (remainingPairs.Count > 0 && HasAnyMoves() == false)
        {
            Change();
        }
    }

    // ham tim kiem cap the hop le de goi y
    public Position[] FindValidPairs()
    {
        Position[] adjacentPair = FindAdjacentPairs();
        if (adjacentPair != null) return adjacentPair;

        Position[] edgePair = FindEdgePairs();
        if (edgePair != null) return edgePair;

        return FindNormalPairs();
    }

    // ham tim cap the nam sat canh nhau
    private Position[] FindAdjacentPairs()
    {
        for (int row = 1; row < totalRows - 1; row++)
        {
            for (int col = 1; col < totalCols - 1; col++)
            {
                int id = idGrid[row, col];
                if (id == -1) continue;

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

    // ham tim cap the nam o mep vien ban co
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

        // sap xep do sau tu ngoai vao trong
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

    // ham vet can quet toan bo ban de tim cap the noi duoc
    private Position[] FindNormalPairs()
    {
        for (int row1 = 0; row1 < totalRows; row1++)
        {
            for (int col1 = 0; col1 < totalCols; col1++)
            {
                int id1 = idGrid[row1, col1];
                if (id1 == -1) continue;

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

    // ham tinh do sau cua o so voi bon mep vien ban co
    private int GetLayerDepth(int row, int col)
    {
        int distTop = row;
        int distBottom = (totalRows - 1) - row;
        int distLeft = col;
        int distRight = (totalCols - 1) - col;

        return Mathf.Min(distTop, distBottom, distLeft, distRight);
    }

    // ham bat sang cap the goi y trong mot khoang thoi gian ngan
    public bool GetHint()
    {
        StopHint();

        Position[] pair = FindValidPairs();
        if (pair != null)
        {
            hintTileOne = tileObjects[pair[0].row, pair[0].col];
            hintTileTwo = tileObjects[pair[1].row, pair[1].col];

            if (hintTileOne != null) hintTileOne.HighlightHint();
            if (hintTileTwo != null) hintTileTwo.HighlightHint();

            // bat coroutine de tu dong up lai sau 1.2 giay
            hintCoroutine = StartCoroutine(TemporaryHintRoutine(1.2f));
            return true;
        }

        return false;
    }

    // coroutine tu dong khoi phuc lai the bai sau thoi gian goi y
    private IEnumerator TemporaryHintRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (hintTileOne != null)
        {
            hintTileOne.RestoreFromHint();
        }
        if (hintTileTwo != null)
        {
            hintTileTwo.RestoreFromHint();
        }

        hintTileOne = null;
        hintTileTwo = null;
        hintCoroutine = null;
    }

    // ham dung va huy bo hieu ung goi y
    private void StopHint()
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        if (hintTileOne != null)
        {
            hintTileOne.RestoreFromHint();
            hintTileOne = null;
        }
        if (hintTileTwo != null)
        {
            hintTileTwo.RestoreFromHint();
            hintTileTwo = null;
        }
    }

    // ham dao vi tri tat ca cac the bai con lai tren ban
    public void Change()
    {
        StopHint();

        List<int> newBoardLayout = ShuffleTiles();
        int indexID = 0;

        for (int row = 0; row < tileObjects.GetLength(0); row++)
        {
            for (int col = 0; col < tileObjects.GetLength(1); col++)
            {
                MemoryTile tile = tileObjects[row, col];
                if (tile == null) continue;

                int id = newBoardLayout[indexID];
                tile.id = id;
                tile.SetSprite(tileSprites[id]);
                tile.HideTile(); // giu trang thai up sau khi dao bai

                idGrid[row, col] = id;
                indexID++;
            }
        }
    }

    // ham di chuyen the bai tu hang nay sang hang khac trong ma tran
    private void MoveTile(int fromRow, int col, int toRow)
    {
        idGrid[toRow, col] = idGrid[fromRow, col];
        idGrid[fromRow, col] = -1;

        tileObjects[toRow, col] = tileObjects[fromRow, col];
        tileObjects[fromRow, col] = null;

        MemoryTile tileObj = tileObjects[toRow, col];
        if (tileObj != null)
        {
            tileObj.row = toRow;
            tileObj.transform.position = new Vector3(startX + col, startY - toRow, 0.0f);
            tileObj.name = "Tile[" + toRow + ", " + col + "]";
        }
    }

    // ham don the xuong phia duoi lap khoang trong
    private void ShiftDown()
    {
        for (int c = 1; c < totalCols - 1; c++)
        {
            for (int r = totalRows - 2; r >= 1; r--)
            {
                if (idGrid[r, c] == -1)
                {
                    for (int k = r - 1; k >= 1; k--)
                    {
                        if (idGrid[k, c] != -1)
                        {
                            MoveTile(k, c, r);
                            break;
                        }
                    }
                }
            }
        }
    }

    // ham don the len phia tren lap khoang trong
    private void ShiftUp()
    {
        for (int c = 1; c < totalCols - 1; c++)
        {
            for (int r = 1; r < totalRows - 1; r++)
            {
                if (idGrid[r, c] == -1)
                {
                    for (int k = r + 1; k < totalRows - 1; k++)
                    {
                        if (idGrid[k, c] != -1)
                        {
                            MoveTile(k, c, r);
                            break;
                        }
                    }
                }
            }
        }
    }

    // ham kiem tra con nuoc di hop le nao tren ban khong
    private bool HasAnyMoves()
    {
        Position[] pair = FindValidPairs();
        return pair != null;
    }

    // ham lay tong so the bai ban dau
    public int GetTotalTiles()
    {
        return totalTiles;
    }

    // ham lat mo toan bo the tren ban de nguoi choi quan sat
    public void ShowAllTiles(bool isSelectedHighlight = false)
    {
        if (tileObjects == null) return;

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                MemoryTile tile = tileObjects[row, col];
                if (tile != null)
                {
                    tile.ShowTile(isSelectedHighlight);
                }
            }
        }
    }

    // ham up toan bo the tren ban xuong
    public void HideAllTiles()
    {
        if (tileObjects == null) return;

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                MemoryTile tile = tileObjects[row, col];
                if (tile != null)
                {
                    tile.HideTile();
                }
            }
        }
    }
}
