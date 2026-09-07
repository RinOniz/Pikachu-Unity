using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite[] classicSprites;
    
    private Sprite[] tileSprites;

    private GameObject[,] tileObjects;
     
    private const int totalRows = 10;
    private const int totalCols = 18;

    public static int totalTiles = 8 * 16;

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

        CreateBoard();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void CreateBoard()
    {
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                if (row == 0 || row == totalRows - 1 || col == 0 || col == totalCols - 1)
                {
                    continue;
                }
                else
                {
                    Vector3 position = new Vector3(startX + col, startY - row, 0.0f);
                    GameObject newTileObj = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;

                    newTileObj.name = "Tile[" + row + ", " + col + "]";
                    newTileObj.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
                    newTileObj.transform.SetParent(board.transform);
                }
            }
        }
    }
}
