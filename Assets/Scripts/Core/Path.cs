using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    // prefab diem sang de ghep thanh duong noi
    [SerializeField] private GameObject pointPrefab;

    // danh sach cac diem sang dang duoc bat
    private List<SpriteRenderer> activePoints = new List<SpriteRenderer>();
    private GameObject[,] paths;

    // kich thuoc ma tran luoi diem noi
    private const int totalRows = 10;
    private const int totalCols = 18;

    // toa do goc bat dau de sinh luoi diem
    private float startX = -9.2f;
    private float startY = 4.5f;

    // bo dem thoi gian de tu dong an duong noi sau mot khoang thoi gian
    private float timer = 0.0f;

    // ham khoi tao va sinh toan bo diem noi tren luoi
    private void Start()
    {
        paths = new GameObject[totalRows, totalCols];

        // khoi tao ma tran diem sang
        Spawn();
    }

    // ham update kiem tra bo dem thoi gian de tat duong noi
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            // khi het thoi gian hien thi thi an duong noi di
            if (timer <= 0)
            {
                HidePath(); 
            }
        }
    }

    // ham sinh truoc toan bo cac diem sang tren ma tran 10x18
    private void Spawn()
    {
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                Vector3 position = new Vector3(startX + col, startY - row, 0.0f);

                // khoi tao doi tuong diem sang tai toa do
                GameObject point = Instantiate(pointPrefab, position, Quaternion.identity) as GameObject;
                point.name = "Point[" + row + ", " + col + "]";
                point.transform.SetParent(gameObject.transform);

                // mac dinh an diem sang di
                SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
                pointRenderer.enabled = false;
                pointRenderer.sortingOrder = 3;

                paths[row, col] = point;
            }
        }
    }

    // ham ve duong noi di qua cac goc toa do danh sach truyen vao
    public void DrawPath(List<Position> corners)
    {
        // an duong cu truoc khi ve duong moi
        HidePath();

        // duyet qua tung doan thang giua cac diem goc
        for (int i = 0; i < corners.Count - 1; i++)
        {
            Position p1 = corners[i];
            Position p2 = corners[i + 1];

            // bat diem dau cua doan thang
            EnablePoint(p1.row, p1.col);

            // noi suy va bat cac diem nam giua hai goc
            if (p1.row == p2.row) // doan thang chay theo chieu ngang
            {
                int step = p2.col > p1.col ? 1 : -1;

                for (int c = p1.col + step; c != p2.col; c += step)
                {
                    EnablePoint(p1.row, c);
                }
            }
            else if (p1.col == p2.col) // doan thang chay theo chieu doc
            {
                int step = p2.row > p1.row ? 1 : -1;

                for (int r = p1.row + step; r != p2.row; r += step)
                {
                    EnablePoint(r, p1.col);
                }
            }

            // bat diem ket thuc cua doan thang
            EnablePoint(p2.row, p2.col);
        }

        // thiet lap thoi gian hien thi duong noi la 0.5 giay
        timer = 0.5f;
    }

    // ham phu tro bat mot diem sang an toan va them vao danh sach quan ly
    private void EnablePoint(int row, int col)
    {
        // kiem tra gioi han de tranh loi tran mang
        if (row >= 0 && row < totalRows && col >= 0 && col < totalCols)
        {
            SpriteRenderer sr = paths[row, col].GetComponent<SpriteRenderer>();

            // neu diem chua bat thi bat len va them vao danh sach
            if (!sr.enabled)
            {
                sr.enabled = true;
                activePoints.Add(sr);
            }
        }
    }

    // ham don dep va tat toan bo cac diem sang dang hien thi
    private void HidePath()
    {
        foreach (SpriteRenderer sr in activePoints)
        {
            sr.enabled = false;
        }

        // xoa sach danh sach cac diem dang hoat dong
        activePoints.Clear();

        timer = 0.0f;
    }
}
