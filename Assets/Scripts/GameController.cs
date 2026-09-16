using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject board;

    private GameObject firstTile;
    private GameObject secondTile;

    private bool isSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public GameObject GetFirstTile()
    {
        return firstTile;
    }

    public void SelectFirstTile(GameObject tile)
    {
        firstTile = tile;
        isSelected = true;
    }

    public void SelectSecondTile(GameObject tile)
    {
        secondTile = tile;

        CompareTwoTiles();
        ResetTile();
    }

    private void ResetTile()
    {
        firstTile = null;
        secondTile = null;

        isSelected = false;
    }

    private void CompareTwoTiles()
    {
        if (firstTile != null && secondTile != null)
        {
            Tile firstTileData = firstTile.GetComponent<Tile>();
            Tile secondTileData = secondTile.GetComponent<Tile>();

            Position firstPos = new Position(firstTileData.row, firstTileData.col);
            Position secondPos = new Position(secondTileData.row, secondTileData.col);

            Board board = GetComponent<Board>();

            if (firstTileData.id == secondTileData.id && board.CheckLine(firstPos, secondPos, true))
            {
                Destroy(firstTile);
                Destroy(secondTile);

                board.Clear(firstPos, secondPos);
            }
            else
            {
                firstTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                secondTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
    
    //public void GetHint(int totalGetHintTime)
    //{
    //    Board board = GetComponent<Board>();

    //    bool hasHint = board.GetHint();

    //    if (hasHint)
    //    {
    //        // score;
    //    }

    //    return hasHint;
    //}
}
