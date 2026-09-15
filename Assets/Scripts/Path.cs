using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;

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
            timer = timer > 0 ? timer : 0.0f;

            if (timer == 0.0f)
            {
                for (int row = 0; row < totalRows; row++)
                {
                    for (int col = 0; col < totalCols; col++)
                    {
                        GameObject point = paths[row, col];

                        SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();

                        if (pointRenderer.enabled)
                        {
                            pointRenderer.enabled = false;
                        }
                    }
                }
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

    public void DrawPath(List<Position> positions)
    {
        foreach (Position position in positions)
        {
            int row = position.row;
            int col = position.col;

            GameObject point = paths[row, col];

            SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
            pointRenderer.enabled = true;
        }

        timer = 0.7f;
    }
}
