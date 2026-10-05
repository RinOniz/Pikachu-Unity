using TMPro;
using UnityEngine;

public class EndlessGameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI nextRowCountdownText;
    [SerializeField] private TextMeshProUGUI nextRowLabelText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private EndlessBoard board;
    [SerializeField] private GameObject boardContainer;
    [SerializeField] private GameObject dangerLine;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;

    private GameObject firstTile;
    private GameObject secondTile;

    private bool isSelected = false;
    private bool isGameOver = false;
    private bool isPause = false;
    private bool isResultShown = false;

    private float survivalTime = 0f;
    private float nextRowTimer = 20f;
    private const float ROW_PUSH_INTERVAL = 20f;

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

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            if (startGameSound != null)
            {
                audioSource.PlayOneShot(startGameSound);
            }
        }
    }

    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            // 1. Đồng hồ đếm TIẾN (Survival Time)
            survivalTime += Time.deltaTime;
            if (survivalTimeText != null)
            {
                int minutes = Mathf.FloorToInt(survivalTime / 60f);
                int seconds = Mathf.FloorToInt(survivalTime % 60f);
                survivalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            // 2. Bộ đếm NGƯỢC 10s thêm hàng mới
            nextRowTimer -= Time.deltaTime;
            if (nextRowCountdownText != null)
            {
                float displayTime = Mathf.Max(0f, nextRowTimer);
                nextRowCountdownText.text = string.Format("{0:0.0}s", displayTime);

                if (displayTime > 5f)
                {
                    nextRowCountdownText.color = new Color(231f / 255f, 76f / 255f, 60f / 255f); // Đỏ cảnh báo
                }
                else if (displayTime > 2f)
                {
                    nextRowCountdownText.color = new Color(243f / 255f, 156f / 255f, 18f / 255f); // Cam
                }
                else
                {
                    nextRowCountdownText.color = new Color(231f / 255f, 76f / 255f, 60f / 255f); // Đỏ cảnh báo
                }
            }

            if (nextRowTimer <= 0f)
            {
                nextRowTimer = ROW_PUSH_INTERVAL;
                bool pushSuccess = board.PushRowFromBottom();
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

    public void SelectFirstTile(GameObject tile)
    {
        if (!isGameOver && !isPause)
        {
            firstTile = tile;
            isSelected = true;
        }
    }

    public void SelectSecondTile(GameObject tile)
    {
        if (!isGameOver && !isPause)
        {
            secondTile = tile;
            CompareTwoTiles();
            ResetTiles();
        }
    }

    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;
        isSelected = false;
    }

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

            if (t1.id == t2.id && board.CheckLine(pos1, pos2, true))
            {
                Destroy(firstTile);
                Destroy(secondTile);

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

    public bool GetHint()
    {
        if (isGameOver || isPause) return false;
        return board.GetHint();
    }

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

    public void TogglePause()
    {
        isPause = !isPause;
    }

    public bool IsGameOver() => isGameOver;

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

    private void CheckGameOverState()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;

            // Ẩn bàn cờ pokemon, vạch đỏ nguy hiểm và đường nối để ResultPanel hiện lên sạch sẽ
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
