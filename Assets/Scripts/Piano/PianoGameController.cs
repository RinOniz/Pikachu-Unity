using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PianoGameController : MonoBehaviour
{
    // instance singleton dung chung
    public static PianoGameController Instance;

    // cac thanh phan giao dien nguoi dung
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI notesHitText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject[] lifeIcons;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Button menuButton;

    // am thanh tro choi
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;

    // sprite nen the quan co
    [Header("Prefabs & Resources")]
    [SerializeField] private Sprite cardBackgroundSprite;

    private Sprite[] allPokemonSprites;
    private int[] activePokemonIds = new int[12];
    private PianoReferenceCard[] referenceCards = new PianoReferenceCard[12];

    private List<PianoTile> activeFallingTiles = new List<PianoTile>();

    // toa do 4 lan roi o trung tam man hinh
    private readonly float[] laneX = new float[] { -2.55f, -1.35f, -0.15f, 1.05f };
    private const float HIT_LINE_Y = -3.2f;
    private const float HIT_WINDOW_MIN = -3.95f;
    private const float HIT_WINDOW_MAX = -2.35f;

    // thong so toc do roi va tan suat sinh not (tang dan nhe nhang)
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

    // danh sach 12 phim bam tuong ung hai tay trai va phai
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

    // ham khoi tao singleton instance
    private void Awake()
    {
        Instance = this;
    }

    // ham khoi tao man choi piano tiles
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
        spawnTimer = 0.5f; // bat dau roi not dau tien sau 0.5s

        // khoi tao am luong va phat am thanh bat dau
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

        // nap tai nguyen sprite, tao the tham chieu va ke 4 lan roi
        LoadSprites();
        SetupReferenceCards();
        CreateVisualLanes();
        UpdateUI();
    }

    // ham nap cac sprite pokemon va chon ngau nhien 12 con cho van choi
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

        // chon ngau nhien 12 con pokemon khac nhau hoan toan
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allPokemonSprites.Length; i++) availableIndices.Add(i);

        for (int i = 0; i < 12; i++)
        {
            int randPos = Random.Range(0, availableIndices.Count);
            activePokemonIds[i] = availableIndices[randPos];
            availableIndices.RemoveAt(randPos);
        }
    }

    // ham tao 12 the tham chieu chia lam hai cum trai va phai
    private void SetupReferenceCards()
    {
        GameObject refRoot = new GameObject("ReferenceCards");

        // cum ben trai: q w e hang tren va a s d hang duoi
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

        // cum ben phai: u i o hang tren va j k l hang duoi
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

    // ham tao hinh anh 4 lan roi, vach phan dinh hit line va vien phat sang
    private void CreateVisualLanes()
    {
        GameObject lanesObj = new GameObject("Lanes");
        Sprite dotSprite = Resources.Load<Sprite>("UI/Dot");

        // buoc 1: tao nen mo cho toan bo khu vuc 4 lan roi
        GameObject trackBg = new GameObject("TrackBackdrop");
        trackBg.transform.SetParent(lanesObj.transform);
        trackBg.transform.position = new Vector3(-0.75f, 0.4f, 0f);
        trackBg.transform.localScale = new Vector3(4.88f, 9.2f, 1f);

        SpriteRenderer trackSr = trackBg.AddComponent<SpriteRenderer>();
        trackSr.sprite = dotSprite;
        trackSr.color = new Color(0f, 0f, 0f, 0.45f);
        trackSr.sortingOrder = 1;

        // buoc 2: ke 5 duong vien doc ngan cach 4 lan roi
        for (int i = 0; i <= 4; i++)
        {
            float lineX = -3.15f + (i * 1.2f);
            GameObject lineObj = new GameObject("LaneLine_" + i);
            lineObj.transform.SetParent(lanesObj.transform);
            lineObj.transform.position = new Vector3(lineX, 0.4f, 0f);

            bool isBorder = (i == 0 || i == 4);
            lineObj.transform.localScale = new Vector3(isBorder ? 0.085f : 0.055f, 9.2f, 1f);

            SpriteRenderer sr = lineObj.AddComponent<SpriteRenderer>();
            sr.sprite = dotSprite;
            sr.color = isBorder ? new Color(1f, 1f, 1f, 0.95f) : new Color(1f, 1f, 1f, 0.75f);
            sr.sortingOrder = 2;
        }

        // buoc 3: tao khung phan dinh mo tai vi tri hit line o moi lan
        for (int i = 0; i < 4; i++)
        {
            GameObject targetBox = new GameObject("HitTargetBox_" + i);
            targetBox.transform.SetParent(lanesObj.transform);
            targetBox.transform.position = new Vector3(laneX[i], HIT_LINE_Y, 0f);
            targetBox.transform.localScale = new Vector3(1.08f, 1.08f, 1f);

            SpriteRenderer boxSr = targetBox.AddComponent<SpriteRenderer>();
            boxSr.sprite = cardBackgroundSprite != null ? cardBackgroundSprite : dotSprite;
            boxSr.color = new Color(1f, 1f, 1f, 0.08f);
            boxSr.sortingOrder = 2;
        }

        // buoc 4: tao vach phan dinh mau xanh neon phat sang o day
        GameObject hitLineObj = new GameObject("HitLine");
        hitLineObj.transform.SetParent(lanesObj.transform);
        hitLineObj.transform.position = new Vector3(-0.75f, HIT_LINE_Y, 0f);
        hitLineObj.transform.localScale = new Vector3(4.88f, 0.18f, 1f);

        SpriteRenderer hitSr = hitLineObj.AddComponent<SpriteRenderer>();
        hitSr.sprite = dotSprite;
        hitSr.color = new Color(46f / 255f, 245f / 255f, 115f / 255f, 1.0f);
        hitSr.sortingOrder = 5;

        // buoc 5: tao loi trang phat sang manh o giua thanh phan dinh
        GameObject hitCoreObj = new GameObject("HitLine_Core");
        hitCoreObj.transform.SetParent(lanesObj.transform);
        hitCoreObj.transform.position = new Vector3(-0.75f, HIT_LINE_Y, 0f);
        hitCoreObj.transform.localScale = new Vector3(4.88f, 0.06f, 1f);

        SpriteRenderer coreSr = hitCoreObj.AddComponent<SpriteRenderer>();
        coreSr.sprite = dotSprite;
        coreSr.color = new Color(230f / 255f, 255f / 255f, 240f / 255f, 0.95f);
        coreSr.sortingOrder = 6;
    }

    // ham update kiem tra roi not, bat phim va cap nhat giao dien moi frame
    private void Update()
    {
        if (isGameOver || isPause) return;

        survivalTime += Time.deltaTime;

        // sinh not roi tu do theo chu ky thoi gian
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnFallingTile();
        }

        // lang nghe va xu ly su kien bam 12 phim tren ban phim
        HandleInput();

        // cap nhat diem so va so mang
        UpdateUI();
    }

    // ham sinh mot not nhac pokemon roi tu dinh xuong o mot trong 4 lan
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

    // ham kiem tra bam phim trong so 12 phim tuong ung
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

    // ham xu ly khi nguoi choi bam mot phim cu the
    private void OnKeyPressed(int cardIndex)
    {
        PianoReferenceCard card = referenceCards[cardIndex];
        int targetPokeId = card.pokemonId;

        // tim not roi khop voi pokemon nay dang o gan vach phan dinh nhat
        PianoTile bestMatchingTile = null;
        float lowestY = float.MaxValue;

        // don dep danh sach cac tile da bi huy
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

        // neu tim thay not hop le trong vung phan dinh -> bat trung
        if (bestMatchingTile != null)
        {
            card.HighlightPressed();
            activeFallingTiles.Remove(bestMatchingTile);
            bestMatchingTile.OnHit();

            notesHit++;
            score += 100;

            // tang toc do roi va tan suat xuat hien nhe nhang theo tien do
            currentFallSpeed = Mathf.Min(3.8f, 2.3f + (notesHit * 0.02f));
            spawnInterval = Mathf.Max(0.92f, 1.55f - (notesHit * 0.012f));

            if (audioSource != null && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        // neu bam sai khi khong co not tuong ung -> phat am thanh va tru mang
        else
        {
            card.HighlightError();
            TakeDamage();
        }
    }

    // ham xu ly khi not bi roi qua vach ma nguoi choi bo lo
    public void OnTileMiss(PianoTile tile)
    {
        activeFallingTiles.Remove(tile);
        TakeDamage();
    }

    // ham tru mang cua nguoi choi khi bam sai hoac bo lo not
    private void TakeDamage()
    {
        if (isGameOver) return;

        lives--;
        if (audioSource != null && incorrectSound != null)
        {
            audioSource.PlayOneShot(incorrectSound);
        }

        // neu het 3 mang thi ket thuc van dau
        if (lives <= 0)
        {
            SetGameOver();
        }
    }

    // ham ket thuc van dau va hien thi popup result panel
    private void SetGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // don dep toan bo cac not dang roi
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

    // ham cap nhat diem so va icon trai tim the hien so mang
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (notesHitText != null)
        {
            notesHitText.text = notesHit.ToString();
        }

        if (livesText != null)
        {
            livesText.text = Mathf.Max(0, lives).ToString();
        }

        // bat tat cac bieu tuong trai tim theo so mang con lai
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

    // ham cho button pause chuyen doi trang thai tam dung
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
