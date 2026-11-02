using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    // cac gameobject ban co va prefab quan co
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;

    // tap sprite hinh anh cho tung che do co dien va hai huoc
    [SerializeField] private Sprite[] classicSprites;
    [SerializeField] private Sprite[] funnySprites;

    private Sprite[] tileSprites;
    private GameObject[,] tileObjects;
    private GameObject hintTileOne;
    private GameObject hintTileTwo;
    private List<int> remainingPairs;
    private Path pathController;

    // tong so quan co tren ban choi 8x16
    public static int totalTiles = 8 * 16;

    // kich thuoc ma tran bao gom ca vien dem xung quanh 10x18
    private const int totalRows = 10;
    private const int totalCols = 18;
    private int[,] idGrid;

    // toa do goc de sinh luoi quan co
    private float startX = -9.2f;
    private float startY = 4.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        int mode = PlayerPrefs.GetInt("GameMode", 0);

        // nap bo sprite hinh anh theo che do choi duoc chon
        if (mode == 0)
        {
            classicSprites = Resources.LoadAll<Sprite>("Pokemon");
            tileSprites = classicSprites;
        }
        else if (mode == 1)
        {
            funnySprites = Resources.LoadAll<Sprite>("Funny");
            tileSprites = funnySprites;
        }
        else
        {
            classicSprites = Resources.LoadAll<Sprite>("Pokemon");
            tileSprites = classicSprites;
        }

        // tim kiem bo dieu khien ve duong noi path
        GameObject pathObj = GameObject.Find("Path");

        if (pathObj != null)
        {
            pathController = pathObj.GetComponent<Path>();
        }

        // sinh ngau nhien cac cap, xao tron va tao ban co
        PickRandomPairs();
        CreateBoard(ShuffleTiles());
        ClearHintTiles();
    }

    // ham chon ngau nhien cac cap id pokemon cho van choi
    private void PickRandomPairs()
    {
        remainingPairs = new List<int>();

        // chon mot nua tong so quan co de tao thanh cac cap giong nhau
        for (int i = 0; i < totalTiles / 2; i++)
        {
            int pairID = Random.Range(0, tileSprites.Length);

            remainingPairs.Add(pairID);
        }
    }

    // ham nhan doi cac cap va xao tron ngau nhien thu tu
    private List<int> ShuffleTiles()
    {
        List<int> duplicatedList = new List<int>();

        // nhan doi moi id thanh mot cap hai quan co
        for (int i = 0; i < remainingPairs.Count; i++)
        {
            duplicatedList.AddRange(new List<int> { remainingPairs[i], remainingPairs[i] });
        }

        // thuat toan xao tron vi tri cac phan tu trong danh sach
        for (int i = 0; i < duplicatedList.Count; i++)
        {
            int temp = duplicatedList[i];
            int randomId = Random.Range(i, duplicatedList.Count);

            duplicatedList[i] = duplicatedList[randomId];
            duplicatedList[randomId] = temp;
        }

        return duplicatedList;
    }

    // ham khoi tao ma tran va sinh cac gameobject quan co tren ban
    private void CreateBoard(List<int> duplicatedList)
    {
        tileObjects = new GameObject[totalRows, totalCols];
        idGrid = new int[totalRows, totalCols];

        int i = 0;

        // duyet qua toan bo ma tran gom vien dem de dat gia tri
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                // cac o vien ngoai cung luon la o trong co id la -1
                if (row == 0 || row == totalRows - 1 || col == 0 || col == totalCols - 1)
                {
                    tileObjects[row, col] = null;
                    idGrid[row, col] = -1;
                }
                // cac o ben trong duoc sinh quan co va gan hinh anh
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

    // ham kiem tra thuat toan duong noi onet toi da 2 khuc gap giua hai diem
    public bool CheckLine(Position startPos, Position endPos, bool drawPath)
    {
        // truong hop 1: noi thang theo chieu ngang chu i
        if (startPos.row == endPos.row && CheckLineCol(startPos.row, startPos.col, endPos.col))
        {
            if (drawPath)
            {
                pathController.DrawPath(new List<Position> { startPos, endPos });
            }

            return true;
        }

        // truong hop 2: noi thang theo chieu doc chu i
        if (startPos.col == endPos.col && CheckLineRow(startPos.col, startPos.row, endPos.row))
        {
            if (drawPath)
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
            if (drawPath)
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
            if (drawPath)
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
                    if (drawPath)
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
                    if (drawPath)
                    {
                        pathController.DrawPath(new List<Position> { startPos, new Position(startPos.row, col), new Position(endPos.row, col), endPos });
                    }

                    return true;
                }
            }
        }
        return false;
    }

    // ham kiem tra doan duong doc giua hai hang co bi vuong quan co nao khong
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

    // ham kiem tra doan duong ngang giua hai cot co bi vuong quan co nao khong
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

    // ham xoa hai quan co khi an dung va xu ly co che don o hoac xao co
    public void Clear(Position firstPos, Position secondPos)
    {
        int valueID = idGrid[firstPos.row, firstPos.col];

        // cap nhat ma tran logic sang o trong
        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        // xoa trang thai goi y cu
        ClearHintTiles();
        RestoreHintTilesColor();

        // bot cap id ra khoi danh sach cap con lai
        remainingPairs.Remove(valueID);

        int gameMode = PlayerPrefs.GetInt("GameMode", 0);

        // xu ly rieng cho che do funny mode: an la tu dong xao lai toan bo ban co
        if (gameMode == 1)
        {
            if (remainingPairs.Count > 0)
            {
                Change();

                // dam bao sau khi xao van phai co it nhat mot nuoc di hop le
                int safetyLimit = 50;
                while (remainingPairs.Count > 0 && HasAnyMoves() == false && safetyLimit-- > 0)
                {
                    Change();
                }

                Hint hint = GetComponent<Hint>();
                if (hint != null)
                {
                    hint.ChangeText("HINT");
                }
            }
        }
        // xu ly cho che do normal mode theo tung cap do
        else
        {
            int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

            // level 2 ap dung trong luc don cac quan co xuong duoi
            if (currentLevel == 2)
            {
                ShiftDown();
            }
            // level 3 ap dung trong luc don cac quan co len tren
            else if (currentLevel == 3)
            {
                ShiftUp();
            }

            // neu khong con nuoc di hop le nao thi tu dong dao ban co
            if (remainingPairs.Count > 0 && HasAnyMoves() == false)
            {
                Debug.Log("Het duong, tu dong Change");

                Change();
            }
        }
    }

    // ham tim kiem cap quan co hop le theo thu tu uu tien lien ke, mep, vet can
    public Position[] FindValidPairs()
    {
        // uu tien tim cap nam sat canh nhau
        Position[] adjacentPair = FindAdjacentPairs();

        if (adjacentPair != null)
        {
            Debug.Log("Lien ke");

            return adjacentPair;
        }

        // uu tien tiep theo tim cap nam o ria ngoai mep ban co
        Position[] edgePair = FindEdgePairs();
        if (edgePair != null)
        {
            Debug.Log("Mep");

            return edgePair;
        }

        // neu khong co thi quet toan bo ban co theo thuat toan vet can
        Debug.Log("Vet can");

        return FindNormalPairs();
    }

    // ham tim kiem cac cap o giong nhau va nam lien ke truc tiep
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

                // kiem tra o ben phai
                if (col + 1 < totalCols - 1 && idGrid[row, col + 1] == id)
                {
                    if (CheckLine(new Position(row, col), new Position(row, col + 1), false))
                    {
                        return new Position[] { new Position(row, col), new Position(row, col + 1) };
                    }
                }

                // kiem tra o ben duoi
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

    // ham tim kiem cap quan co nam o ria ngoai cung cua ban co
    private Position[] FindEdgePairs()
    {
        List<Position> edgeTiles = new List<Position>();

        // loc ra danh sach cac o nam gan mep ngoai
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

        // sap xep danh sach theo do sau tang dan de uu tien o ngoai ria
        edgeTiles.Sort((p1, p2) =>
        {
            int depth1 = GetLayerDepth(p1.row, p1.col);
            int depth2 = GetLayerDepth(p2.row, p2.col);

            return depth1.CompareTo(depth2);
        });

        // kiem tra noi tung cap o ria voi nhau
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

    // ham vet can duyet toan bo ban co de tim bat ky cap quan co nao co the noi
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

                // so sanh voi cac o con lai tren ban co
                for (int row2 = row1; row2 < totalRows; row2++)
                {
                    int startCol = (row1 == row2) ? col1 + 1 : 0;

                    for (int col2 = startCol; col2 < totalCols; col2++)
                    {
                        int id2 = idGrid[row2, col2];

                        // neu cung loai pokemon thi thu tim duong noi
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

    // ham tinh toan khoang cach ngan nhat tu o den bon mep vien ban co
    private int GetLayerDepth(int row, int col)
    {
        int distTop = row;
        int distBottom = (totalRows - 1) - row;
        int distLeft = col;
        int distRight = (totalCols - 1) - col;

        return Mathf.Min(distTop, distBottom, distLeft, distRight);
    }

    // ham bat hieu ung goi y lam sang hai quan co co the noi duoc
    public bool GetHint()
    {
        Position[] pair = FindValidPairs();

        if (pair != null)
        {
            hintTileOne = tileObjects[pair[0].row, pair[0].col];
            hintTileTwo = tileObjects[pair[1].row, pair[1].col];

            // doi mau nen cua hai quan co sang mau xanh la de bao hieu
            Color color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

            return true;
        }

        return false;
    }

    // ham xao lai toan bo cac quan co con lai tren ban
    public void Change()
    {
        List<int> newBoardLayout = ShuffleTiles();

        int indexID = 0;

        // gan lai id moi va sprite moi cho cac quan co con tren ban
        for (int row = 0; row < tileObjects.GetLength(0); row++)
        {
            for (int col = 0; col < tileObjects.GetLength(1); col++)
            {
                GameObject tile = tileObjects[row, col];

                if (tile == null)
                {
                    continue;
                }

                int id = newBoardLayout[indexID];

                Tile tileData = tile.GetComponent<Tile>();
                tileData.id = id;

                SpriteRenderer spriteRender = tile.GetComponent<SpriteRenderer>();
                spriteRender.sprite = tileSprites[tileData.id];

                idGrid[row, col] = id;
                indexID++;
            }
        }

        // xoa bo cac goi y cu
        ClearHintTiles();
    }

    // ham di chuyen vi tri quan co tu hang nay sang hang khac trong ma tran
    private void MoveTile(int fromRow, int col, int toRow)
    {
        // cap nhat ma tran id
        idGrid[toRow, col] = idGrid[fromRow, col];
        idGrid[fromRow, col] = -1;

        // cap nhat ma tran gameobject
        tileObjects[toRow, col] = tileObjects[fromRow, col];
        tileObjects[fromRow, col] = null;

        GameObject tileObj = tileObjects[toRow, col];

        Tile tileData = tileObj.GetComponent<Tile>();
        tileData.row = toRow;

        // cap nhat toa do vat ly cua quan co
        tileObj.transform.position = new Vector3(startX + col, startY - toRow, 0.0f);
        tileObj.name = "Tile[" + toRow + ", " + col + "]";
    }

    // ham don cac quan co xuong phia duoi khi co khoang trong
    private void ShiftDown()
    {
        for (int c = 1; c < totalCols - 1; c++)
        {
            for (int r = totalRows - 2; r >= 1; r--)
            {
                if (idGrid[r, c] == -1)
                {
                    // tim quan co phia tren de keo xuong lap cho trong
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

    // ham don cac quan co len phia tren khi co khoang trong
    private void ShiftUp()
    {
        for (int c = 1; c < totalCols - 1; c++)
        {
            for (int r = 1; r < totalRows - 1; r++)
            {
                if (idGrid[r, c] == -1)
                {
                    // tim quan co phia duoi de day len lap cho trong
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

    // ham khoi phuc lai mau trang goc cho hai quan co duoc goi y
    private void RestoreHintTilesColor()
    {
        if (hintTileOne != null && hintTileTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }
    }

    // ham reset bien tham chieu den cac quan co goi y
    private void ClearHintTiles()
    {
        if (hintTileOne != null && hintTileTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }

        hintTileOne = null;
        hintTileTwo = null;
    }

    // ham kiem tra xem hien tai co con bat ky nuoc di hop le nao khong
    private bool HasAnyMoves()
    {
        Position[] pair = FindValidPairs();

        return pair != null;
    }

    // ham lay tong so quan co tren ban
    public int GetTotalTiles()
    {
        return totalTiles;
    }
}
