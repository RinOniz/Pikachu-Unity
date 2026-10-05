using System.Collections;
using TMPro;
using UnityEngine;

public class MemoryGameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject board;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;

    private MemoryTile firstTile;
    private MemoryTile secondTile;

    private bool isSelected;
    private bool isProcessing;
    private bool isPreviewing;
    private bool isGameOver;
    private bool isPause;
    private bool isResultShown = false;

    private int score;
    private int totalTiles;
    private int clearTiles;

    private void Start()
    {
        score = 0;
        isPause = false;
        isGameOver = false;
        isProcessing = false;
        isPreviewing = false;
        clearTiles = 0;

        MemoryBoard memoryBoard = GetComponent<MemoryBoard>();
        if (memoryBoard != null)
        {
            totalTiles = memoryBoard.GetTotalTiles();
        }

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        PlayStartSound();

        StartCoroutine(GameFlowRoutine());
    }

    private void Update()
    {
        if (!isPause && !isGameOver && scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        CheckWin();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }

    public bool IsPreviewing()
    {
        return isPreviewing;
    }

    private IEnumerator GameFlowRoutine()
    {
        MemoryBoard memoryBoard = GetComponent<MemoryBoard>();

        // 1. Cho người chơi xem toàn bộ các ô trong 5 giây đầu
        isPreviewing = true;
        isProcessing = true;

        yield return null; // Chờ 1 frame để Board tạo xong các tile

        if (memoryBoard != null)
        {
            memoryBoard.ShowAllTiles(false);
        }

        float initialTimer = 5.0f;
        while (initialTimer > 0)
        {
            initialTimer -= Time.deltaTime;
            yield return null;
        }

        if (memoryBoard != null)
        {
            memoryBoard.HideAllTiles();
        }

        isPreviewing = false;
        isProcessing = false;

        // 2. Vòng lặp: chơi 5s -> lật 2s -> chơi 5s...
        while (!isGameOver)
        {
            float playTimer = 0.0f;
            while (playTimer < 5.0f)
            {
                if (isGameOver) yield break;

                if (!isPause)
                {
                    playTimer += Time.deltaTime;
                }
                yield return null;
            }

            // Đợi nếu đang dở so sánh thẻ
            while (isProcessing && !isGameOver)
            {
                yield return null;
            }

            if (isGameOver) yield break;

            if (firstTile != null)
            {
                firstTile.HideTile();
            }
            ResetTiles();

            isPreviewing = true;
            isProcessing = true;

            if (memoryBoard != null)
            {
                memoryBoard.ShowAllTiles(false);
            }

            float peekTimer = 2.0f;
            while (peekTimer > 0)
            {
                if (isGameOver) yield break;

                if (!isPause)
                {
                    peekTimer -= Time.deltaTime;
                }
                yield return null;
            }

            if (memoryBoard != null)
            {
                memoryBoard.HideAllTiles();
            }

            isPreviewing = false;
            isProcessing = false;
        }
    }

    public MemoryTile GetFirstTile()
    {
        return firstTile;
    }

    public void SelectFirstTile(MemoryTile tile)
    {
        if (isGameOver || isPause || isProcessing || isPreviewing)
        {
            return;
        }

        firstTile = tile;
        firstTile.ShowTile(true);
        isSelected = true;
    }

    public void SelectSecondTile(MemoryTile tile)
    {
        if (isGameOver || isPause || isProcessing || isPreviewing)
        {
            return;
        }

        secondTile = tile;
        secondTile.ShowTile(true);

        StartCoroutine(CompareTwoTilesRoutine());
    }

    private IEnumerator CompareTwoTilesRoutine()
    {
        isProcessing = true;

        if (firstTile != null && secondTile != null)
        {
            Position firstPos = new Position(firstTile.row, firstTile.col);
            Position secondPos = new Position(secondTile.row, secondTile.col);

            MemoryBoard memoryBoard = GetComponent<MemoryBoard>();
            bool isMatch = (firstTile.id == secondTile.id) && memoryBoard.CheckLine(firstPos, secondPos, true);

            if (isMatch)
            {
                if (audioSource != null && correctSound != null)
                {
                    audioSource.PlayOneShot(correctSound);
                }

                AddScore();
                clearTiles += 2;

                // Để người chơi thấy đường nối vẽ ra một xíu trước khi xóa
                yield return new WaitForSeconds(0.25f);

                if (firstTile != null)
                {
                    Destroy(firstTile.gameObject);
                }
                if (secondTile != null)
                {
                    Destroy(secondTile.gameObject);
                }

                memoryBoard.Clear(firstPos, secondPos);

                if (clearTiles >= totalTiles)
                {
                    isGameOver = true;
                    if (audioSource != null && winSound != null)
                    {
                        audioSource.PlayOneShot(winSound);
                    }
                }
            }
            else
            {
                if (audioSource != null && incorrectSound != null)
                {
                    audioSource.PlayOneShot(incorrectSound);
                }

                score = Mathf.Max(0, score - 20);

                // Thời gian chờ để người chơi kịp quan sát và ghi nhớ thẻ vừa mở (0.75s)
                yield return new WaitForSeconds(0.75f);

                if (firstTile != null)
                {
                    firstTile.HideTile();
                }
                if (secondTile != null)
                {
                    secondTile.HideTile();
                }
            }
        }

        ResetTiles();
        isProcessing = false;
    }

    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;
        isSelected = false;
    }

    public void PlayStartSound()
    {
        if (audioSource != null && startGameSound != null)
        {
            audioSource.PlayOneShot(startGameSound);
        }
    }

    public bool GetHint(int totalGetHintTime)
    {
        if (isProcessing || isPreviewing || isGameOver || isPause)
        {
            return false;
        }

        MemoryBoard memoryBoard = GetComponent<MemoryBoard>();
        if (memoryBoard == null)
        {
            return false;
        }

        bool hasHint = memoryBoard.GetHint();
        if (hasHint)
        {
            score = Mathf.Max(0, score - (10 * totalGetHintTime));
        }

        return hasHint;
    }

    private void AddScore()
    {
        score += 50;
    }

    public void MinusChangeScore(int totalChanges)
    {
        score = Mathf.Max(0, score - (25 * totalChanges));
    }

    public bool IsPause()
    {
        return isPause;
    }

    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    public void SetGameOver()
    {
        isGameOver = true;
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        ResetTiles();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    private void CheckWin()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;
            if (board != null)
            {
                board.SetActive(false);
            }

            float remainTime = 0f;
            MemoryRoundCountdown countdown = GetComponent<MemoryRoundCountdown>();
            if (countdown != null)
            {
                remainTime = countdown.GetRemainTime();
            }

            if (resultPanel != null)
            {
                MemoryResultPanel panel = resultPanel.GetComponent<MemoryResultPanel>();
                if (panel != null)
                {
                    panel.Show(clearTiles >= totalTiles, score, remainTime);
                }
                else
                {
                    ResultPanel oldPanel = resultPanel.GetComponent<ResultPanel>();
                    if (oldPanel != null)
                    {
                        oldPanel.Show(clearTiles >= totalTiles, score, remainTime);
                    }
                }
            }
        }
    }
}
