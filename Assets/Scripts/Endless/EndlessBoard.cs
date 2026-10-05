using System.Collections.Generic;
using UnityEngine;

public class EndlessBoard : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private int initialRows = 4; // Bắt đầu với 4 hàng ở đáy (hàng 5, 6, 7, 8)

    private Sprite[] tileSprites;
    private GameObject[,] tileObjects;
    private int[,] idGrid;

    private const int totalRows = 10; // Hàng 0 và 9 là biên rỗng
    private const int totalCols = 18; // Cột 0 và 17 là biên rỗng
    private const int playableCols = 16; // Cột 1 đến 16

    private float startX = -9.2f;
    private float startY = 4.5f;

    private List<int> remainingPairs = new List<int>();
    private Path pathController;

    private GameObject hintTileOne;
    private GameObject hintTileTwo;

    private bool isGravity = false;
    private int rowsPushedCount = 0;
    private int pairsClearedCount = 0;

    private void Start()
    {
        // 0: Classic, 1: Funny, 3: Tetris/Endless
        int mode = PlayerPrefs.GetInt("GameMode", 3);
        if (mode == 1)
        {
            tileSprites = Resources.LoadAll<Sprite>("Funny");
        }
        else
        {
            tileSprites = Resources.LoadAll<Sprite>("Pokemon");
        }

        if (tileSprites == null || tileSprites.Length == 0)
        {
            tileSprites = Resources.LoadAll<Sprite>("Pokemon");
        }

        // Đọc chế độ Trọng lực: 0 = NoGravity, 1 = Gravity
        isGravity = (PlayerPrefs.GetInt("TetrisGravity", 0) == 1);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null)
        {
            pathController = pathObj.GetComponent<Path>();
        }

        InitializeBoard();
    }

    private void InitializeBoard()
    {
        tileObjects = new GameObject[totalRows, totalCols];
        idGrid = new int[totalRows, totalCols];

        for (int r = 0; r < totalRows; r++)
        {
            for (int c = 0; c < totalCols; c++)
            {
                idGrid[r, c] = -1;
                tileObjects[r, c] = null;
            }
        }

        remainingPairs.Clear();

        // Tạo các hàng khởi đầu ở đáy bàn cờ
        // initialRows = 4 -> từ hàng (9 - initialRows) đến hàng 8
        int startRow = Mathf.Clamp(totalRows - 1 - initialRows, 1, 8);
        for (int r = startRow; r <= 8; r++)
        {
            SpawnRowAt(r);
        }

        // Đảm bảo ban đầu có ít nhất 1 nước đi
        int safety = 30;
        while (!HasAnyMoves() && safety-- > 0)
        {
            Change();
        }
    }

    private void SpawnRowAt(int targetRow)
    {
        // Mỗi hàng có 16 ô (cột 1 đến 16) = 8 cặp
        List<int> rowIds = new List<int>();
        for (int p = 0; p < playableCols / 2; p++)
        {
            int pairId = Random.Range(0, tileSprites.Length);
            rowIds.Add(pairId);
            rowIds.Add(pairId);
            remainingPairs.Add(pairId);
        }

        // Shuffle hàng này
        for (int i = 0; i < rowIds.Count; i++)
        {
            int temp = rowIds[i];
            int rand = Random.Range(i, rowIds.Count);
            rowIds[i] = rowIds[rand];
            rowIds[rand] = temp;
        }

        for (int c = 1; c <= playableCols; c++)
        {
            int valId = rowIds[c - 1];
            Vector3 pos = new Vector3(startX + c, startY - targetRow, 0.0f);

            GameObject newTileObj = Instantiate(tilePrefab, pos, Quaternion.identity);
            newTileObj.name = "Tile[" + targetRow + ", " + c + "]";
            newTileObj.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
            if (board != null)
            {
                newTileObj.transform.SetParent(board.transform);
            }

            EndlessTile tileData = newTileObj.GetComponent<EndlessTile>();
            if (tileData == null)
            {
                tileData = newTileObj.AddComponent<EndlessTile>();
            }
            tileData.row = targetRow;
            tileData.col = c;
            tileData.id = valId;

            SpriteRenderer spriteRenderer = newTileObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 2;
                spriteRenderer.sprite = tileSprites[valId];
            }

            tileObjects[targetRow, c] = newTileObj;
            idGrid[targetRow, c] = valId;
        }
    }

    /// <summary>
    /// Kiểm tra xem có quân cờ nào đang ở hàng nguy hiểm (hàng 1) hay không.
    /// Nếu có, việc đẩy thêm hàng sẽ làm tràn đỉnh -> Thua cuộc.
    /// </summary>
    public bool IsDangerRowOccupied()
    {
        for (int c = 1; c <= playableCols; c++)
        {
            if (idGrid[1, c] != -1)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Đẩy toàn bộ các hàng từ dưới lên trên.
    /// Hàng mới xuất hiện ở đáy (hàng 8).
    /// Trả về false nếu hàng 1 đã có quân (tràn đỉnh -> Game Over).
    /// </summary>
    public bool PushRowFromBottom()
    {
        // Kiểm tra tràn đỉnh
        if (IsDangerRowOccupied())
        {
            return false; // Tràn bảng -> Game Over
        }

        ClearHintTiles();

        // Dịch chuyển các hàng 2..8 lên hàng 1..7
        for (int r = 1; r <= 7; r++)
        {
            for (int c = 1; c <= playableCols; c++)
            {
                idGrid[r, c] = idGrid[r + 1, c];
                tileObjects[r, c] = tileObjects[r + 1, c];

                if (tileObjects[r, c] != null)
                {
                    EndlessTile tileData = tileObjects[r, c].GetComponent<EndlessTile>();
                    if (tileData != null)
                    {
                        tileData.row = r;
                        tileData.col = c;
                    }
                    tileObjects[r, c].name = "Tile[" + r + ", " + c + "]";
                    tileObjects[r, c].transform.position = new Vector3(startX + c, startY - r, 0.0f);
                }
            }
        }

        // Hàng 8 rỗng tạm thời
        for (int c = 1; c <= playableCols; c++)
        {
            idGrid[8, c] = -1;
            tileObjects[8, c] = null;
        }

        // Sinh hàng mới ở hàng 8
        SpawnRowAt(8);

        rowsPushedCount++;

        // Đảm bảo luôn có nước đi
        if (remainingPairs.Count > 0 && !HasAnyMoves())
        {
            Change();
        }

        return true;
    }

    public void Clear(Position firstPos, Position secondPos)
    {
        int valueID = idGrid[firstPos.row, firstPos.col];

        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        ClearHintTiles();

        remainingPairs.Remove(valueID);
        pairsClearedCount++;

        // Chế độ Trọng lực: các ô phía trên tụt xuống lấp đầy ô trống
        if (isGravity)
        {
            ApplyGravity();
        }

        // Nếu hết nước đi thì tự động đảo cờ
        if (remainingPairs.Count > 0 && !HasAnyMoves())
        {
            Change();
        }
    }

    /// <summary>
    /// Áp dụng trọng lực: các ô ở trên rơi xuống các vị trí trống bên dưới trong cùng cột.
    /// </summary>
    private void ApplyGravity()
    {
        for (int c = 1; c <= playableCols; c++)
        {
            for (int r = 8; r >= 1; r--)
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

    private void MoveTile(int fromRow, int col, int toRow)
    {
        idGrid[toRow, col] = idGrid[fromRow, col];
        idGrid[fromRow, col] = -1;

        tileObjects[toRow, col] = tileObjects[fromRow, col];
        tileObjects[fromRow, col] = null;

        GameObject tileObj = tileObjects[toRow, col];
        if (tileObj != null)
        {
            EndlessTile tileData = tileObj.GetComponent<EndlessTile>();
            if (tileData != null)
            {
                tileData.row = toRow;
            }

            tileObj.transform.position = new Vector3(startX + col, startY - toRow, 0.0f);
            tileObj.name = "Tile[" + toRow + ", " + col + "]";
        }
    }

    public void Change()
    {
        // Thu thập toàn bộ các ô hiện đang có trên bàn cờ
        List<Position> activePositions = new List<Position>();
        List<int> currentIds = new List<int>();

        for (int r = 1; r <= 8; r++)
        {
            for (int c = 1; c <= playableCols; c++)
            {
                if (idGrid[r, c] != -1 && tileObjects[r, c] != null)
                {
                    activePositions.Add(new Position(r, c));
                    currentIds.Add(idGrid[r, c]);
                }
            }
        }

        if (currentIds.Count == 0) return;

        // Trộn các ID hiện có
        for (int i = 0; i < currentIds.Count; i++)
        {
            int temp = currentIds[i];
            int rand = Random.Range(i, currentIds.Count);
            currentIds[i] = currentIds[rand];
            currentIds[rand] = temp;
        }

        // Gán lại cho các vị trí
        for (int i = 0; i < activePositions.Count; i++)
        {
            Position pos = activePositions[i];
            int newId = currentIds[i];

            GameObject tile = tileObjects[pos.row, pos.col];
            if (tile != null)
            {
                EndlessTile tileData = tile.GetComponent<EndlessTile>();
                if (tileData != null)
                {
                    tileData.id = newId;
                }

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null && newId >= 0 && newId < tileSprites.Length)
                {
                    sr.sprite = tileSprites[newId];
                }

                idGrid[pos.row, pos.col] = newId;
            }
        }

        ClearHintTiles();
    }

    public bool GetHint()
    {
        Position[] pair = FindValidPairs();

        if (pair != null)
        {
            hintTileOne = tileObjects[pair[0].row, pair[0].col];
            hintTileTwo = tileObjects[pair[1].row, pair[1].col];

            if (hintTileOne != null)
            {
                EndlessTile t1 = hintTileOne.GetComponent<EndlessTile>();
                if (t1 != null) t1.HighlightHint();
            }
            if (hintTileTwo != null)
            {
                EndlessTile t2 = hintTileTwo.GetComponent<EndlessTile>();
                if (t2 != null) t2.HighlightHint();
            }

            return true;
        }

        return false;
    }

    public void ClearHintTiles()
    {
        if (hintTileOne != null)
        {
            EndlessTile t1 = hintTileOne.GetComponent<EndlessTile>();
            if (t1 != null) t1.RestoreColor();
        }
        if (hintTileTwo != null)
        {
            EndlessTile t2 = hintTileTwo.GetComponent<EndlessTile>();
            if (t2 != null) t2.RestoreColor();
        }

        hintTileOne = null;
        hintTileTwo = null;
    }

    public bool CheckLine(Position startPos, Position endPos, bool drawPath)
    {
        if (startPos.row == endPos.row && CheckLineCol(startPos.row, startPos.col, endPos.col))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, endPos });
            }
            return true;
        }

        if (startPos.col == endPos.col && CheckLineRow(startPos.col, startPos.row, endPos.row))
        {
            if (drawPath && pathController != null)
            {
                pathController.DrawPath(new List<Position> { startPos, endPos });
            }
            return true;
        }

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

    public Position[] FindValidPairs()
    {
        Position[] adjacent = FindAdjacentPairs();
        if (adjacent != null) return adjacent;

        Position[] edge = FindEdgePairs();
        if (edge != null) return edge;

        return FindNormalPairs();
    }

    private Position[] FindAdjacentPairs()
    {
        for (int row = 1; row <= 8; row++)
        {
            for (int col = 1; col <= playableCols; col++)
            {
                int id = idGrid[row, col];
                if (id == -1) continue;

                if (col + 1 <= playableCols && idGrid[row, col + 1] == id)
                {
                    if (CheckLine(new Position(row, col), new Position(row, col + 1), false))
                    {
                        return new Position[] { new Position(row, col), new Position(row, col + 1) };
                    }
                }

                if (row + 1 <= 8 && idGrid[row + 1, col] == id)
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

        for (int row = 1; row <= 8; row++)
        {
            for (int col = 1; col <= playableCols; col++)
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
        for (int row1 = 1; row1 <= 8; row1++)
        {
            for (int col1 = 1; col1 <= playableCols; col1++)
            {
                int id1 = idGrid[row1, col1];
                if (id1 == -1) continue;

                for (int row2 = row1; row2 <= 8; row2++)
                {
                    int startCol = (row1 == row2) ? col1 + 1 : 1;

                    for (int col2 = startCol; col2 <= playableCols; col2++)
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

    public bool HasAnyMoves()
    {
        return FindValidPairs() != null;
    }

    public int GetPairsCleared() => pairsClearedCount;
    public int GetRowsPushed() => rowsPushedCount;
    public bool GetIsGravity() => isGravity;
}
