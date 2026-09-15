using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;

    private List<SpriteRenderer> activePoints = new List<SpriteRenderer>();
    private GameObject[,] paths;

    private const int totalRows = 10;
    private const int totalCols = 18;

    private float startX = -9.2f;
    private float startY = 4.5f;
    private float timer = 0.0f;

    private void Start()
    {
        paths= new GameObject[totalRows, totalCols];

        Spawn();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                HidePath(); 
            }
        }
    }

    private void Spawn()
    {
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                Vector3 position = new Vector3(startX + col, startY - row, 0.0f);

                GameObject point = Instantiate(pointPrefab, position, Quaternion.identity) as GameObject;
                point.name = "Point[" + row + ", " + col + "]";
                point.transform.SetParent(gameObject.transform);

                SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
                pointRenderer.enabled = false;
                pointRenderer.sortingOrder = 3;

                paths[row, col] = point;
            }
        }
    }

    public void DrawPath(List<Position> corners)
    {
        HidePath();

        for (int i = 0; i < corners.Count - 1; i++)
        {
            Position p1 = corners[i];
            Position p2 = corners[i + 1];

            EnablePoint(p1.row, p1.col);

            // 2. Nội suy và bật các điểm ở giữa
            if (p1.row == p2.row) // Chạy theo chiều ngang
            {
                int step = p2.col > p1.col ? 1 : -1;

                for (int c = p1.col + step; c != p2.col; c += step)
                {
                    EnablePoint(p1.row, c);
                }
            }
            else if (p1.col == p2.col) // Chạy theo chiều dọc
            {
                int step = p2.row > p1.row ? 1 : -1;

                for (int r = p1.row + step; r != p2.row; r += step)
                {
                    EnablePoint(r, p1.col);
                }
            }

            // 3. Bật điểm kết thúc của đoạn thẳng
            EnablePoint(p2.row, p2.col);
        }

        timer = 0.5f;
    }

    // Hàm phụ trợ bật 1 điểm an toàn và đưa vào List
    private void EnablePoint(int row, int col)
    {
        // Kiểm tra an toàn, tránh lỗi IndexOutOfRangeException nếu path đi ra viền ngoài lưới
        if (row >= 0 && row < totalRows && col >= 0 && col < totalCols)
        {
            SpriteRenderer sr = paths[row, col].GetComponent<SpriteRenderer>();

            // Nếu điểm này chưa bật thì mới bật và thêm vào List
            if (!sr.enabled)
            {
                sr.enabled = true;
                activePoints.Add(sr);
            }
        }
    }

    // Hàm dọn dẹp tối ưu
    private void HidePath()
    {
        foreach (SpriteRenderer sr in activePoints)
        {
            sr.enabled = false;
        }

        activePoints.Clear(); // Dọn sạch List sau khi đã tắt

        timer = 0.0f;
    }
}
