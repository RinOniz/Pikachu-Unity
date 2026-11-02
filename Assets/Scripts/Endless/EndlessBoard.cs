using System.Collections.Generic;
using UnityEngine;

public class EndlessBoard : MonoBehaviour
{
    // cac gameobject ban co va prefab quan co
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;

    // so luong hang khoi dau o day ban co
    [SerializeField] private int initialRows = 4; // bat dau voi 4 hang o day tu hang 5 den hang 8

    private Sprite[] tileSprites;
    private GameObject[,] tileObjects;
    private int[,] idGrid;

    // kich thuoc ma tran 10x18 voi cac bien dem xung quanh
    private const int totalRows = 10; // hang 0 va 9 la bien rong
    private const int totalCols = 18; // cot 0 va 17 la bien rong
    private const int playableCols = 16; // vung choi gom cot 1 den 16

    private float startX = -9.2f;
    private float startY = 4.5f;

    private List<int> remainingPairs = new List<int>();
    private Path pathController;

    private GameObject hintTileOne;
    private GameObject hintTileTwo;

    // co xac dinh co ap dung trong luc tetris hay khong
    private bool isGravity = false;
    private int rowsPushedCount = 0;
    private int pairsClearedCount = 0;

    // ham khoi tao nap sprite va tao cac hang khoi dau
    private void Start()
    {
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

        // doc tuy chon trong luc: 0 = khong trong luc, 1 = co trong luc tut xuong
        isGravity = (PlayerPrefs.GetInt("TetrisGravity", 0) == 1);

        GameObject pathObj = GameObject.Find("Path");
        if (pathObj != null)
        {
            pathController = pathObj.GetComponent<Path>();
        }

        InitializeBoard();
    }

    // ham khoi tao ma tran va sinh cac hang quan co ban dau o day
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

        // tao cac hang khoi dau o day ban co tu hang 5 den 8
        int startRow = Mathf.Clamp(totalRows - 1 - initialRows, 1, 8);
        for (int r = startRow; r <= 8; r++)
        {
            SpawnRowAt(r);
        }

        // dam bao ban dau co it nhat mot nuoc di hop le
        int safety = 30;
        while (!HasAnyMoves() && safety-- > 0)
        {
            Change();
        }
    }

    // ham sinh mot hang quan co moi tai chi so hang chi dinh
    private void SpawnRowAt(int targetRow)
    {
        // moi hang co 16 o tuong ung 8 cap quan co
        List<int> rowIds = new List<int>();
        for (int p = 0; p < playableCols / 2; p++)
        {
            int pairId = Random.Range(0, tileSprites.Length);
            rowIds.Add(pairId);
            rowIds.Add(pairId);
            remainingPairs.Add(pairId);
        }

        // xao tron thu tu cac quan co trong hang
        for (int i = 0; i < rowIds.Count; i++)
        {
            int temp = rowIds[i];
            int rand = Random.Range(i, rowIds.Count);
            rowIds[i] = rowIds[rand];
            rowIds[rand] = temp;
        }

        // khoi tao gameobject cho tung o trong hang
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

    // ham kiem tra xem hang nguy hiem tren cung hang 1 da co quan co chua
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

    // ham day toan bo cac hang len tren mot nac va sinh hang moi o day
    public bool PushRowFromBottom()
    {
        // kiem tra neu hang tren cung da co quan co thi tran bang thua cuoc
        if (IsDangerRowOccupied())
        {
            return false;
        }

        ClearHintTiles();

        // dich chuyen cac hang tu 2 den 8 len cac hang 1 den 7
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

        // dat hang 8 ve trang thai rong
        for (int c = 1; c <= playableCols; c++)
        {
            idGrid[8, c] = -1;
            tileObjects[8, c] = null;
        }

        // sinh hang moi tai hang 8 o day
        SpawnRowAt(8);

        rowsPushedCount++;

        // neu het nuoc di thi tu dong dao vi tri
        if (remainingPairs.Count > 0 && !HasAnyMoves())
        {
            Change();
        }

        return true;
    }

    // ham xoa hai quan co khi an thanh cong va xu ly trong luc roi
    public void Clear(Position firstPos, Position secondPos)
    {
        int valueID = idGrid[firstPos.row, firstPos.col];

        // cap nhat ma tran logic sang o trong
        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        ClearHintTiles();

        remainingPairs.Remove(valueID);
        pairsClearedCount++;

        // neu che do co trong luc duoc bat thi cac o phia tren roi xuong lap khoang trong
        if (isGravity)
        {
            ApplyGravity();
        }

        // neu het nuoc di thi tu dong dao co
        if (remainingPairs.Count > 0 && !HasAnyMoves())
        {
            Change();
        }
    }

    // ham ap dung trong luc roi: cac quan co phia tren tut xuong cac vi tri trong ben duoi
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

    // ham di chuyen quan co tu hang nay sang hang khac trong cung cot
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

    // ham xao tron lai toan bo cac quan co hien co tren ban
    public void Change()
    {
        List<Position> activePositions = new List<Position>();
        List<int> currentIds = new List<int>();

        // thu thap cac vi tri dang co quan co
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

        // tron ngau nhien danh sach id
        for (int i = 0; i < currentIds.Count; i++)
        {
            int temp = currentIds[i];
            int rand = Random.Range(i, currentIds.Count);
            currentIds[i] = currentIds[rand];
            currentIds[rand] = temp;
        }

        // gan lai id va sprite cho tung quan co
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

    // ham tim va bat sang cap quan co goi y
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

    // ham khoi phuc lai mau cho cac quan co goi y
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

    // ham kiem tra thuat toan duong noi onet toi da 2 khuc gap giua hai diem
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

    // ham kiem tra doan duong doc giua hai hang co trong khong
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

    // ham kiem tra doan duong ngang giua hai cot co trong khong
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

    // ham tim kiem cap quan co hop le theo thu tu uu tien
    public Position[] FindValidPairs()
    {
        Position[] adjacent = FindAdjacentPairs();
        if (adjacent != null) return adjacent;

        Position[] edge = FindEdgePairs();
        if (edge != null) return edge;

        return FindNormalPairs();
    }

    // ham tim cap quan co lien ke sat nhau
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

    // ham tim cap quan co o ria ngoai mep ban co
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

        // sap xep do sau tu ria vao trong
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

    // ham vet can toan bo ban co de tim cap noi duoc
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

    // ham tinh khoang cach den bon mep vien
    private int GetLayerDepth(int row, int col)
    {
        int distTop = row;
        int distBottom = (totalRows - 1) - row;
        int distLeft = col;
        int distRight = (totalCols - 1) - col;

        return Mathf.Min(distTop, distBottom, distLeft, distRight);
    }

    // ham kiem tra xem co con nuoc di hop le nao tren ban khong
    public bool HasAnyMoves()
    {
        return FindValidPairs() != null;
    }

    // ham lay so cap quan co da an thanh cong
    public int GetPairsCleared() => pairsClearedCount;

    // ham lay so hang da duoc day tu day len
    public int GetRowsPushed() => rowsPushedCount;

    // ham kiem tra co che trong luc co dang bat khong
    public bool GetIsGravity() => isGravity;
}
