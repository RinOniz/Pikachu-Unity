using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour
{
    // text hien thi tong diem so cua nguoi choi
    [SerializeField] private TextMeshProUGUI scoreText;

    // cac gameobject quan ly giao dien ket qua va ban co
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject board;

    // cac nguon am thanh va hieu ung am thanh trong man choi
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;

    // bien luu tru hai quan co duoc nguoi choi lan luot chon
    private GameObject firstTile;
    private GameObject secondTile;

    // cac bien trang thai cua van dau
    private bool isSelected;
    private bool isGameOver;
    private bool isPause;
    private bool isResultShown = false;

    // thong so diem so va so luong quan co
    private int score;
    private int totalTiles;
    private int clearTiles;

    // ham khoi tao trang thai ban dau cho van dau
    private void Start()
    {
        score = 0;

        isPause = false;
        isGameOver = false;

        clearTiles = 0;
        totalTiles = GetComponent<Board>().GetTotalTiles();

        // thiet lap am luong hieu ung sfx tu bo nho cai dat
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        // phat am thanh khi van dau bat dau
        PlayStartSound();
    }

    // ham update cap nhat diem so va kiem tra dieu kien thang moi frame
    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            scoreText.text = score.ToString();
        }

        CheckWin();
    }

    // ham kiem tra xem da co quan co thu nhat duoc chon hay chua
    public bool IsSelected()
    {
        return isSelected;
    }

    // ham lay ra quan co thu nhat dang duoc chon
    public GameObject GetFirstTile()
    {
        return firstTile;
    }

    // ham luu lai quan co thu nhat khi nguoi choi click chon
    public void SelectFirstTile(GameObject tile)
    {
        if (!isGameOver)
        {
            firstTile = tile;
            isSelected = true;
        }
    }

    // ham xu ly khi nguoi choi click chon quan co thu hai
    public void SelectSecondTile(GameObject tile)
    {
        secondTile = tile;

        // so sanh hai quan co va xoa bo neu hop le
        CompareTwoTiles();

        // reset trang thai lua chon
        ResetTiles();
    }

    // ham phat am thanh hieu ung bat dau game hoac khi dao co
    public void PlayStartSound()
    {
        audioSource.PlayOneShot(startGameSound);
    }

    // ham dat lai hai quan co duoc chon ve trang thai trong
    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;

        isSelected = false;
    }

    // ham so sanh hai quan co xem co cung id va duong noi hop le khong
    private void CompareTwoTiles()
    {
        if (firstTile != null && secondTile != null)
        {
            Tile firstTileData = firstTile.GetComponent<Tile>();
            Tile secondTileData = secondTile.GetComponent<Tile>();

            Position firstPos = new Position(firstTileData.row, firstTileData.col);
            Position secondPos = new Position(secondTileData.row, secondTileData.col);

            Board board = GetComponent<Board>();

            // neu cung hinh anh pokemon va tim duoc duong noi toi da 2 khuc gap
            if (firstTileData.id == secondTileData.id && board.CheckLine(firstPos, secondPos, true))
            {
                // xoa khoi giao dien hai quan co
                Destroy(firstTile);
                Destroy(secondTile);

                // xoa khoi du lieu ban co va thuc hien don o neu co
                board.Clear(firstPos, secondPos);

                // cong diem thuong khi an thanh cong
                AddScore();

                clearTiles += 2;
                audioSource.PlayOneShot(correctSound);

                // kiem tra da an het toan bo quan co tren ban chua
                if (clearTiles >= totalTiles)
                {
                    isGameOver = true;
                    audioSource.PlayOneShot(winSound);
                }
            }
            // neu khong noi duoc hoac khong cung loai thi khoi phuc mau va tru diem
            else
            {
                firstTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                secondTile.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                score -= 25;

                audioSource.PlayOneShot(incorrectSound);
            }
        }
    }

    // ham lay goi y nuoc di hop le va tinh diem phat
    public bool GetHint(int totalGetHintTime)
    {
        Board board = GetComponent<Board>();

        bool hasHint = board.GetHint();

        // neu co goi y thi tru diem theo so lan da su dung
        if (hasHint)
        {
            score -= (10 * totalGetHintTime);
        }

        return hasHint;
    }

    // ham cong diem cho nguoi choi khi an duoc cap quan co
    private void AddScore()
    {
        int point = 50;

        score += point;
    }

    // ham tru diem khi nguoi choi su dung quyen doi vi tri quan co
    public void MinusChangeScore(int totalChanges)
    {
        score -= (25 * totalChanges);
    }

    // ham kiem tra tro choi co dang bi tam dung khong
    public bool IsPause()
    {
        return isPause;
    }

    // ham thay doi trang thai tam dung cua tro choi
    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    // ham dat trang thai game over khi het gio hoac thua cuoc
    public void SetGameOver()
    {
        isGameOver = true;
        audioSource.PlayOneShot(gameOverSound);

        ResetTiles();
    }

    // ham kiem tra tro choi da ket thuc chua
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // ham kiem tra dieu kien ket thuc de hien thi bang ket qua result panel
    private void CheckWin()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;
            board.SetActive(false);

            float remainTime = GetComponent<RoundCountdown>().GetRemainTime();

            // hien thi popup tong ket diem va thoi gian
            resultPanel.GetComponent<ResultPanel>().Show(clearTiles >= totalTiles, score, remainTime);
        }
    }
}
