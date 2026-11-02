using TMPro;
using UnityEngine;

public class EndlessGameController : MonoBehaviour
{
    // cac text hien thi thoi gian sinh ton va dem nguoc hang tiep theo
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI nextRowCountdownText;
    [SerializeField] private TextMeshProUGUI nextRowLabelText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private EndlessBoard board;
    [SerializeField] private GameObject boardContainer;
    [SerializeField] private GameObject dangerLine;

    // cac thanh phan am thanh
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;

    // hai quan co duoc chon
    private GameObject firstTile;
    private GameObject secondTile;

    // cac co trang thai cua van choi
    private bool isSelected = false;
    private bool isGameOver = false;
    private bool isPause = false;
    private bool isResultShown = false;

    // bien theo doi thoi gian sinh ton va bo dem nguoc day hang
    private float survivalTime = 0f;
    private float nextRowTimer = 20f;
    private const float ROW_PUSH_INTERVAL = 20f;

    // ham khoi tao van choi che do tetris sinh ton
    private void Start()
    {
        survivalTime = 0f;
        nextRowTimer = ROW_PUSH_INTERVAL;
        isPause = false;
        isGameOver = false;
        isResultShown = false;
        isSelected = false;

        if (nextRowLabelText != null)
        {
            nextRowLabelText.text = "NEXT";
        }

        if (board == null)
        {
            board = GetComponent<EndlessBoard>();
        }

        // thiet lap am luong va phat am thanh bat dau
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            if (startGameSound != null)
            {
                audioSource.PlayOneShot(startGameSound);
            }
        }
    }

    // ham update tinh toan thoi gian sinh ton va dem nguoc day hang moi frame
    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            // buoc 1: dong ho dem tien thoi gian sinh ton
            survivalTime += Time.deltaTime;
            if (survivalTimeText != null)
            {
                int minutes = Mathf.FloorToInt(survivalTime / 60f);
                int seconds = Mathf.FloorToInt(survivalTime % 60f);
                survivalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            // buoc 2: bo dem nguoc 20s de day hang moi tu day len
            nextRowTimer -= Time.deltaTime;
            if (nextRowCountdownText != null)
            {
                float displayTime = Mathf.Max(0f, nextRowTimer);
                nextRowCountdownText.text = string.Format("{0:0.0}s", displayTime);

                // doi mau canh bao khi sap het gio day hang
                if (displayTime > 5f)
                {
                    nextRowCountdownText.color = new Color(231f / 255f, 76f / 255f, 60f / 255f);
                }
                else if (displayTime > 2f)
                {
                    nextRowCountdownText.color = new Color(243f / 255f, 156f / 255f, 18f / 255f);
                }
                else
                {
                    nextRowCountdownText.color = new Color(231f / 255f, 76f / 255f, 60f / 255f);
                }
            }

            // khi bo dem ve 0 thi day hang moi tu day len
            if (nextRowTimer <= 0f)
            {
                nextRowTimer = ROW_PUSH_INTERVAL;
                bool pushSuccess = board.PushRowFromBottom();

                // neu day len ma bi cham vao hang nguy hiem thi game over
                if (!pushSuccess)
                {
                    SetGameOver();
                }
            }
        }

        CheckGameOverState();
    }

    public bool IsSelected() => isSelected;
    public GameObject GetFirstTile() => firstTile;

    // ham luu lai quan co thu nhat khi nguoi choi click
    public void SelectFirstTile(GameObject tile)
    {
        if (!isGameOver && !isPause)
        {
            firstTile = tile;
            isSelected = true;
        }
    }

    // ham xu ly khi nguoi choi click quan co thu hai
    public void SelectSecondTile(GameObject tile)
    {
        if (!isGameOver && !isPause)
        {
            secondTile = tile;
            CompareTwoTiles();
            ResetTiles();
        }
    }

    // ham dat lai hai quan co ve rong
    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;
        isSelected = false;
    }

    // ham so sanh hai quan co va xoa bo neu hop le
    private void CompareTwoTiles()
    {
        if (firstTile != null && secondTile != null)
        {
            EndlessTile t1 = firstTile.GetComponent<EndlessTile>();
            EndlessTile t2 = secondTile.GetComponent<EndlessTile>();

            if (t1 == null || t2 == null)
            {
                ResetTiles();
                return;
            }

            Position pos1 = new Position(t1.row, t1.col);
            Position pos2 = new Position(t2.row, t2.col);

            // kiem tra cung id va duong noi hop le
            if (t1.id == t2.id && board.CheckLine(pos1, pos2, true))
            {
                Destroy(firstTile);
                Destroy(secondTile);

                // xoa khoi ban co va thuc hien roi tu do neu co trong luc
                board.Clear(pos1, pos2);

                if (audioSource != null && correctSound != null)
                {
                    audioSource.PlayOneShot(correctSound);
                }
            }
            else
            {
                t1.RestoreColor();
                t2.RestoreColor();

                if (audioSource != null && incorrectSound != null)
                {
                    audioSource.PlayOneShot(incorrectSound);
                }
            }
        }
    }

    // ham lay goi y nuoc di tren ban co
    public bool GetHint()
    {
        if (isGameOver || isPause) return false;
        return board.GetHint();
    }

    // ham dao lai vi tri cac quan co tren ban
    public void ChangeBoard()
    {
        if (isGameOver || isPause) return;
        board.Change();
    }

    public bool IsPause() => isPause;

    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    // ham chuyen doi qua lai trang thai tam dung
    public void TogglePause()
    {
        isPause = !isPause;
    }

    public bool IsGameOver() => isGameOver;

    // ham thiet lap trang thai ket thuc van choi game over
    public void SetGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        ResetTiles();
    }

    // ham kiem tra de hien thi bang ket qua sinh ton
    private void CheckGameOverState()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;

            // an ban co pokemon, vach do nguy hiem va duong noi de result panel hien thi sach se
            if (boardContainer != null)
            {
                boardContainer.SetActive(false);
            }
            else
            {
                GameObject b = GameObject.Find("Board");
                if (b != null) b.SetActive(false);
            }

            if (dangerLine != null)
            {
                dangerLine.SetActive(false);
            }
            else
            {
                GameObject d = GameObject.Find("DangerLine");
                if (d != null) d.SetActive(false);
            }

            GameObject pathObj = GameObject.Find("Path");
            if (pathObj != null) pathObj.SetActive(false);

            if (resultPanel != null)
            {
                EndlessResultPanel panelScript = resultPanel.GetComponent<EndlessResultPanel>();
                if (panelScript != null)
                {
                    panelScript.Show(survivalTime, board.GetPairsCleared(), board.GetRowsPushed());
                }
                else
                {
                    resultPanel.SetActive(true);
                }
            }
        }
    }
}
