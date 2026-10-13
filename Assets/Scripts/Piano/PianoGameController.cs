using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PianoGameController : MonoBehaviour
{
    public static PianoGameController Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI notesHitText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject[] lifeIcons;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Button menuButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Prefabs & Resources")]
    [SerializeField] private Sprite cardBackgroundSprite;

    private Sprite[] allPokemonSprites;
    private int[] activePokemonIds = new int[12];
    private PianoReferenceCard[] referenceCards = new PianoReferenceCard[12];

    private List<PianoTile> activeFallingTiles = new List<PianoTile>();

    // 4 làn rơi ở trung tâm
    private readonly float[] laneX = new float[] { -2.55f, -1.35f, -0.15f, 1.05f };
    private const float HIT_LINE_Y = -3.2f;
    private const float HIT_WINDOW_MIN = -3.95f;
    private const float HIT_WINDOW_MAX = -2.35f;

    // Tiến trình rơi & tốc độ (tăng dần nhẹ nhàng)
    private float currentFallSpeed = 2.3f;
    private float spawnInterval = 1.55f;
    private float spawnTimer = 0f;

    private int score = 0;
    private int notesHit = 0;
    private int lives = 3;
    private float survivalTime = 0f;

    private bool isGameOver = false;
    private bool isPause = false;
    private bool isResultShown = false;

    // Cấu hình 12 phím bấm chuẩn cho 2 tay
    private readonly KeyCode[] boundKeys = new KeyCode[]
    {
        KeyCode.Q, KeyCode.W, KeyCode.E,
        KeyCode.A, KeyCode.S, KeyCode.D,
        KeyCode.U, KeyCode.I, KeyCode.O,
        KeyCode.J, KeyCode.K, KeyCode.L
    };

    private readonly string[] keyLabels = new string[]
    {
        "Q", "W", "E",
        "A", "S", "D",
        "U", "I", "O",
        "J", "K", "L"
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        score = 0;
        notesHit = 0;
        lives = 3;
        survivalTime = 0f;
        isGameOver = false;
        isPause = false;
        isResultShown = false;

        currentFallSpeed = 2.3f;
        spawnInterval = 1.55f;
        spawnTimer = 0.5f; // Bắt đầu rơi nốt đầu tiên sau 0.5s

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            if (startGameSound != null)
            {
                audioSource.PlayOneShot(startGameSound);
            }
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        }

        LoadSprites();
        SetupReferenceCards();
        CreateVisualLanes();
        UpdateUI();
    }

    private void LoadSprites()
    {
        allPokemonSprites = Resources.LoadAll<Sprite>("Pokemon");
        if (allPokemonSprites == null || allPokemonSprites.Length == 0)
        {
            allPokemonSprites = Resources.LoadAll<Sprite>("Funny");
        }

        if (cardBackgroundSprite == null)
        {
            var tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
            if (tilePrefab != null)
            {
                var bgTransform = tilePrefab.transform.Find("ItemBackground");
                if (bgTransform != null)
                {
                    cardBackgroundSprite = bgTransform.GetComponent<SpriteRenderer>()?.sprite;
                }
            }
        }

        if (cardBackgroundSprite == null)
        {
            cardBackgroundSprite = Resources.Load<Sprite>("Buttons/Tile");
        }

        // Chọn ngẫu nhiên 12 con Pokemon khác nhau
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allPokemonSprites.Length; i++) availableIndices.Add(i);

        for (int i = 0; i < 12; i++)
        {
            int randPos = Random.Range(0, availableIndices.Count);
            activePokemonIds[i] = availableIndices[randPos];
            availableIndices.RemoveAt(randPos);
        }
    }

    private void SetupReferenceCards()
    {
        GameObject refRoot = new GameObject("ReferenceCards");

        // Khu vực bên Trái (Q, W, E trên / A, S, D dưới)
        // Vị trí X từ -7.0 đến -4.4, Y: 1.2f và -0.8f
        float leftStartX = -6.8f;
        float spacingX = 1.25f;

        for (int i = 0; i < 6; i++)
        {
            int col = i % 3;
            int row = i / 3;
            float posX = leftStartX + (col * spacingX);
            float posY = row == 0 ? 1.15f : -0.75f;

            GameObject cardObj = new GameObject("RefCard_Left_" + keyLabels[i]);
            cardObj.transform.SetParent(refRoot.transform);
            cardObj.transform.position = new Vector3(posX, posY, 0f);
            cardObj.transform.localScale = new Vector3(0.95f, 0.95f, 1f);

            PianoReferenceCard card = cardObj.AddComponent<PianoReferenceCard>();
            int pokeId = activePokemonIds[i];
            card.Init(boundKeys[i], keyLabels[i], pokeId, allPokemonSprites[pokeId], cardBackgroundSprite);
            referenceCards[i] = card;
        }

        // Khu vực bên Phải (U, I, O trên / J, K, L dưới)
        // Vị trí X từ 2.8 đến 5.4, Y: 1.2f và -0.8f
        float rightStartX = 2.9f;

        for (int i = 0; i < 6; i++)
        {
            int idx = 6 + i;
            int col = i % 3;
            int row = i / 3;
            float posX = rightStartX + (col * spacingX);
            float posY = row == 0 ? 1.15f : -0.75f;

            GameObject cardObj = new GameObject("RefCard_Right_" + keyLabels[idx]);
            cardObj.transform.SetParent(refRoot.transform);
            cardObj.transform.position = new Vector3(posX, posY, 0f);
            cardObj.transform.localScale = new Vector3(0.95f, 0.95f, 1f);

            PianoReferenceCard card = cardObj.AddComponent<PianoReferenceCard>();
            int pokeId = activePokemonIds[idx];
            card.Init(boundKeys[idx], keyLabels[idx], pokeId, allPokemonSprites[pokeId], cardBackgroundSprite);
            referenceCards[idx] = card;
        }
    }

    private void CreateVisualLanes()
    {
        GameObject lanesObj = new GameObject("Lanes");

        // Dot sprite để kẻ vạch
        Sprite dotSprite = Resources.Load<Sprite>("UI/Dot");

        // 1. Kẻ 5 đường viền dọc ngăn cách 4 làn
        for (int i = 0; i <= 4; i++)
        {
            float lineX = -3.15f + (i * 1.2f);
            GameObject lineObj = new GameObject("LaneLine_" + i);
            lineObj.transform.SetParent(lanesObj.transform);
            lineObj.transform.position = new Vector3(lineX, 0.4f, 0f);
            lineObj.transform.localScale = new Vector3(0.04f, 9.0f, 1f);

            SpriteRenderer sr = lineObj.AddComponent<SpriteRenderer>();
            sr.sprite = dotSprite;
            sr.color = new Color(1f, 1f, 1f, 0.22f); // Màu trắng mờ
            sr.sortingOrder = 1;
        }

        // 2. Vạch phán định phát sáng (Hit Line) ở đáy
        GameObject hitLineObj = new GameObject("HitLine");
        hitLineObj.transform.SetParent(lanesObj.transform);
        hitLineObj.transform.position = new Vector3(-0.75f, HIT_LINE_Y, 0f);
        hitLineObj.transform.localScale = new Vector3(5.0f, 0.12f, 1f);

        SpriteRenderer hitSr = hitLineObj.AddComponent<SpriteRenderer>();
        hitSr.sprite = dotSprite;
        hitSr.color = new Color(46f / 255f, 204f / 255f, 113f / 255f, 0.85f); // Xanh ngọc phát sáng
        hitSr.sortingOrder = 4;
    }

    private void Update()
    {
        if (isGameOver || isPause) return;

        survivalTime += Time.deltaTime;

        // Sinh nốt rơi tự do
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnFallingTile();
        }

        // Lắng nghe 12 phím bấm
        HandleInput();

        // Cập nhật giao diện
        UpdateUI();
    }

    private void SpawnFallingTile()
    {
        int randLane = Random.Range(0, 4);
        int randCardIndex = Random.Range(0, 12);
        int pokeId = activePokemonIds[randCardIndex];

        float spawnX = laneX[randLane];
        float spawnY = 4.8f;

        GameObject tileObj = new GameObject("FallingTile_" + pokeId);
        tileObj.transform.position = new Vector3(spawnX, spawnY, 0f);
        tileObj.transform.localScale = new Vector3(0.88f, 0.88f, 1f);

        PianoTile tile = tileObj.AddComponent<PianoTile>();
        tile.Init(pokeId, randLane, currentFallSpeed, allPokemonSprites[pokeId], cardBackgroundSprite);

        activeFallingTiles.Add(tile);
    }

    private void HandleInput()
    {
        for (int i = 0; i < 12; i++)
        {
            if (Input.GetKeyDown(boundKeys[i]))
            {
                OnKeyPressed(i);
            }
        }
    }

    private void OnKeyPressed(int cardIndex)
    {
        PianoReferenceCard card = referenceCards[cardIndex];
        int targetPokeId = card.pokemonId;

        // Tìm nốt rơi khớp với Pokemon này đang ở gần vạch Hit Line nhất
        PianoTile bestMatchingTile = null;
        float lowestY = float.MaxValue;

        // Dọn dẹp danh sách các tile đã bị huỷ
        activeFallingTiles.RemoveAll(t => t == null);

        foreach (var tile in activeFallingTiles)
        {
            if (tile.pokemonId == targetPokeId)
            {
                float y = tile.transform.position.y;
                if (y >= HIT_WINDOW_MIN && y <= HIT_WINDOW_MAX)
                {
                    if (y < lowestY)
                    {
                        lowestY = y;
                        bestMatchingTile = tile;
                    }
                }
            }
        }

        if (bestMatchingTile != null)
        {
            // BẮT TRÚNG (HIT)!
            card.HighlightPressed();
            activeFallingTiles.Remove(bestMatchingTile);
            bestMatchingTile.OnHit();

            notesHit++;
            score += 100;

            // Tăng tốc nhẹ nhàng hợp lý
            currentFallSpeed = Mathf.Min(3.8f, 2.3f + (notesHit * 0.02f));
            spawnInterval = Mathf.Max(0.92f, 1.55f - (notesHit * 0.012f));

            if (audioSource != null && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        else
        {
            // BẤM SAI (Không có nốt tương ứng ở vạch đệm)
            card.HighlightError();
            TakeDamage();
        }
    }

    public void OnTileMiss(PianoTile tile)
    {
        activeFallingTiles.Remove(tile);
        TakeDamage();
    }

    private void TakeDamage()
    {
        if (isGameOver) return;

        lives--;
        if (audioSource != null && incorrectSound != null)
        {
            audioSource.PlayOneShot(incorrectSound);
        }

        if (lives <= 0)
        {
            SetGameOver();
        }
    }

    private void SetGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Dọn sạch nốt rơi
        foreach (var t in activeFallingTiles)
        {
            if (t != null) Destroy(t.gameObject);
        }
        activeFallingTiles.Clear();

        if (resultPanel != null && !isResultShown)
        {
            isResultShown = true;
            PianoResultPanel panelScript = resultPanel.GetComponent<PianoResultPanel>();
            if (panelScript != null)
            {
                panelScript.Show(score, notesHit, survivalTime);
            }
            else
            {
                resultPanel.SetActive(true);
            }
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (notesHitText != null)
        {
            notesHitText.text = "HITS: " + notesHit;
        }

        if (livesText != null)
        {
            livesText.text = "LIVES: " + Mathf.Max(0, lives);
        }

        if (lifeIcons != null && lifeIcons.Length >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                if (lifeIcons[i] != null)
                {
                    lifeIcons[i].SetActive(i < lives);
                }
            }
        }
    }

    public void TogglePause()
    {
        isPause = !isPause;
        if (pauseText != null)
        {
            pauseText.text = isPause ? "RESUME" : "PAUSE";
        }
    }

    public bool IsPause() => isPause;
    public bool IsGameOver() => isGameOver;
}
