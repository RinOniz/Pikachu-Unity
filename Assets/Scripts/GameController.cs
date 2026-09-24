using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject board;

    private GameObject firstTile;
    private GameObject secondTile;

    private bool isSelected;
    private bool isPause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        isPause = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //if (!isPause && !isGameOver)
        //{
        //    scoreText.text = score.ToString();
        //}

        //CheckWin();
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

                //AddScore();

                //clearTiles += 2;
                //audioSource.PlayOneShot(correctSound);

                //if (clearTiles >= totalTiles)
                //{
                //    isGameOver = true;
                //    audioSource.PlayOneShot(winSound);
                //}
            }
            else
            {
                firstTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                secondTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                //score -= 30;

                //audioSource.PlayOneShot(incorrectSound);
            }
        }
    }

    public bool GetHint(int totalGetHintTime)
    {
        Board board = GetComponent<Board>();

        bool hasHint = board.GetHint();

        if (hasHint)
        {
            //score -= (10 * totalGetHintTime);
        }

        return hasHint;
    }

    public bool IsPause()
    {
        return isPause;
    }

    public void SetPause(bool pause)
    {
        isPause = pause;
    }
}
