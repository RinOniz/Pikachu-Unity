using System.Collections;
using TMPro;
using UnityEngine;

public class MemoryGameController : MonoBehaviour
{
    // text hien thi tong diem so
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject board;

    // cac hieu ung am thanh trong man choi
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;

    // hai the bai duoc nguoi choi chon lan luot
    private MemoryTile firstTile;
    private MemoryTile secondTile;

    // cac co trang thai dieu phoi van dau
    private bool isSelected;
    private bool isProcessing;
    private bool isPreviewing;
    private bool isGameOver;
    private bool isPause;
    private bool isResultShown = false;

    // thong so diem so va so luong the bai
    private int score;
    private int totalTiles;
    private int clearTiles;

    // ham khoi tao van choi che do tri nho
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

        // cai dat am luong sfx tu playerprefs
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        PlayStartSound();

        // bat dau coroutine dieu phoi nhip do mo the cua che do memory
        StartCoroutine(GameFlowRoutine());
    }

    // ham update cap nhat diem so va kiem tra ket thuc moi frame
    private void Update()
    {
        if (!isPause && !isGameOver && scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        CheckWin();
    }

    // ham kiem tra da chon the thu nhat chua
    public bool IsSelected()
    {
        return isSelected;
    }

    // ham kiem tra he thong co dang trong qua trinh so sanh hoac xu ly the khong
    public bool IsProcessing()
    {
        return isProcessing;
    }

    // ham kiem tra he thong co dang trong giai doan cho nguoi choi nhin the khong
    public bool IsPreviewing()
    {
        return isPreviewing;
    }

    // coroutine dieu phoi luong choi dac trung: 5s mo dau -> vong lap 5s up / 2s mo
    private IEnumerator GameFlowRoutine()
    {
        MemoryBoard memoryBoard = GetComponent<MemoryBoard>();

        // buoc 1: cho nguoi choi xem toan bo the trong 5 giay dau
        isPreviewing = true;
        isProcessing = true;

        yield return null; // cho 1 frame de board khoi tao xong cac the

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

        // up tat ca the xuong de bat dau van dau
        if (memoryBoard != null)
        {
            memoryBoard.HideAllTiles();
        }

        isPreviewing = false;
        isProcessing = false;

        // buoc 2: vong lap chu ky choi 5 giay sau do he mo 2 giay de ghi nho
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

            // doi neu nguoi choi dang do tay so sanh hai the
            while (isProcessing && !isGameOver)
            {
                yield return null;
            }

            if (isGameOver) yield break;

            // up the dang chon do neu co
            if (firstTile != null)
            {
                firstTile.HideTile();
            }
            ResetTiles();

            // he mo toan bo the trong 2 giay de tro giup nguoi choi
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

            // up the lai sau khi het 2 giay he mo
            if (memoryBoard != null)
            {
                memoryBoard.HideAllTiles();
            }

            isPreviewing = false;
            isProcessing = false;
        }
    }

    // ham lay the thu nhat dang duoc chon
    public MemoryTile GetFirstTile()
    {
        return firstTile;
    }

    // ham xu ly khi nguoi choi chon the thu nhat
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

    // ham xu ly khi nguoi choi chon the thu hai
    public void SelectSecondTile(MemoryTile tile)
    {
        if (isGameOver || isPause || isProcessing || isPreviewing)
        {
            return;
        }

        secondTile = tile;
        secondTile.ShowTile(true);

        // bat dau coroutine so sanh hai the vua lat
        StartCoroutine(CompareTwoTilesRoutine());
    }

    // coroutine so sanh hai the bai vua duoc nguoi choi lat mo
    private IEnumerator CompareTwoTilesRoutine()
    {
        isProcessing = true;

        if (firstTile != null && secondTile != null)
        {
            Position firstPos = new Position(firstTile.row, firstTile.col);
            Position secondPos = new Position(secondTile.row, secondTile.col);

            MemoryBoard memoryBoard = GetComponent<MemoryBoard>();
            bool isMatch = (firstTile.id == secondTile.id) && memoryBoard.CheckLine(firstPos, secondPos, true);

            // truong hop dung: cung loai va noi duoc duong di
            if (isMatch)
            {
                if (audioSource != null && correctSound != null)
                {
                    audioSource.PlayOneShot(correctSound);
                }

                AddScore();
                clearTiles += 2;

                // cho nguoi choi nhin tia noi trong 0.25 giay truoc khi xoa
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

                // kiem tra chien thang khi da an het the
                if (clearTiles >= totalTiles)
                {
                    isGameOver = true;
                    if (audioSource != null && winSound != null)
                    {
                        audioSource.PlayOneShot(winSound);
                    }
                }
            }
            // truong hop sai: phat am thanh sai, tru diem va cho 0.75s de nguoi choi ghi nho
            else
            {
                if (audioSource != null && incorrectSound != null)
                {
                    audioSource.PlayOneShot(incorrectSound);
                }

                score = Mathf.Max(0, score - 20);

                // thoi gian cho 0.75 giay de nguoi choi kip quan sat va ghi nho the vua lat
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

    // ham dat lai hai the duoc chon ve rong
    private void ResetTiles()
    {
        firstTile = null;
        secondTile = null;
        isSelected = false;
    }

    // ham phat am thanh bat dau van choi
    public void PlayStartSound()
    {
        if (audioSource != null && startGameSound != null)
        {
            audioSource.PlayOneShot(startGameSound);
        }
    }

    // ham lay goi y cap the co the noi va tru diem phat
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

    // ham cong diem khi nguoi choi lat dung cap the
    private void AddScore()
    {
        score += 50;
    }

    // ham tru diem khi nguoi choi dung quyen dao vi tri the
    public void MinusChangeScore(int totalChanges)
    {
        score = Mathf.Max(0, score - (25 * totalChanges));
    }

    // ham kiem tra van choi co dang tam dung khong
    public bool IsPause()
    {
        return isPause;
    }

    // ham dat trang thai tam dung cho van choi
    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    // ham xu ly khi game over het gio hoac thua cuoc
    public void SetGameOver()
    {
        isGameOver = true;
        StopAllCoroutines();

        if (board != null)
        {
            board.SetActive(false);
        }
        else
        {
            GameObject b = GameObject.Find("Board");
            if (b != null) b.SetActive(false);
        }

        GameObject p = GameObject.Find("Path");
        if (p != null) p.SetActive(false);

        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        ResetTiles();
    }

    // ham kiem tra game da ket thuc chua
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // ham kiem tra ket thuc de hien thi popup ket qua result panel
    private void CheckWin()
    {
        if (isGameOver && !isResultShown)
        {
            isResultShown = true;
            StopAllCoroutines();

            if (board != null)
            {
                board.SetActive(false);
            }
            else
            {
                GameObject b = GameObject.Find("Board");
                if (b != null) b.SetActive(false);
            }

            GameObject pathObj = GameObject.Find("Path");
            if (pathObj != null) pathObj.SetActive(false);

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
