using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère toute l'interface : Start Screen, HUD (timer + XP bar), Game Over Screen.
/// Attacher sur un GameObject "UIManager" dans la scène.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ── Panels ────────────────────────────────────────────────────────────────
    [Header("Panels")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    // ── Timer ─────────────────────────────────────────────────────────────────
    [Header("Timer HUD")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image           timerBarFill;
    [SerializeField] private Image           timerBarGlow;       // (optionnel) image overlay pour le glow
    [SerializeField] private float           timerBarDuration = 180f; // 3 minutes = barre pleine

    // Couleurs de progression du timer (vert → jaune → rouge)
    private static readonly Color ColorTimerStart  = new Color(0.18f, 0.88f, 0.42f); // vert
    private static readonly Color ColorTimerMiddle = new Color(1.00f, 0.80f, 0.10f); // jaune
    private static readonly Color ColorTimerEnd    = new Color(1.00f, 0.22f, 0.22f); // rouge

    // ── XP Bar ────────────────────────────────────────────────────────────────
    [Header("XP Bar HUD")]
    [SerializeField] private Image           xpBarFill;
    [SerializeField] private TextMeshProUGUI xpValueText;   // ex: "340 / 500"
    [SerializeField] private TextMeshProUGUI levelText;     // ex: "LVL 3"
    [SerializeField] private float           xpFillSpeed = 4f;

    // Level-up popup
    [Header("Level-Up Popup")]
    [SerializeField] private RectTransform   levelUpPopup;
    [SerializeField] private TextMeshProUGUI levelUpLabel;  // "LEVEL UP !"
    [SerializeField] private CanvasGroup     levelUpCanvasGroup;

    // ── Start Screen ──────────────────────────────────────────────────────────
    [Header("Start Screen")]
    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private Button          startButton;

    // ── Game Over Screen ───────────────────────────────────────────────────────
    [Header("Game Over Screen")]
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI finalLevelText;
    [SerializeField] private TextMeshProUGUI finalKillsText;
    [SerializeField] private Button          restartButton;
    [SerializeField] private Button          quitButton;

    // ── Private state ─────────────────────────────────────────────────────────
    private float   _targetXPFill   = 0f;
    private float   _currentXPFill  = 0f;
    private Coroutine _levelUpRoutine;
    private bool    _timerPulsing   = false;

    // ────────────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void OnEnable()
    {
        GameManager.OnGameStart      += HandleGameStart;
        GameManager.OnGameOver       += HandleGameOver;
        GameManager.OnTimerUpdate    += UpdateTimer;
        GameManager.OnXPChanged      += UpdateXPBar;
        GameManager.OnLevelUp        += HandleLevelUp;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart      -= HandleGameStart;
        GameManager.OnGameOver       -= HandleGameOver;
        GameManager.OnTimerUpdate    -= UpdateTimer;
        GameManager.OnXPChanged      -= UpdateXPBar;
        GameManager.OnLevelUp        -= HandleLevelUp;
    }

    private void Start()
    {
        SetupButtons();
        ShowStartScreen();
    }

    private void Update()
    {
        // Animation fluide de la barre XP
        if (Mathf.Abs(_currentXPFill - _targetXPFill) > 0.001f)
        {
            _currentXPFill        = Mathf.Lerp(_currentXPFill, _targetXPFill, Time.deltaTime * xpFillSpeed);
            xpBarFill.fillAmount  = _currentXPFill;
        }
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Screen Management

    private void ShowStartScreen()
    {
        startScreenPanel.SetActive(true);
        hudPanel        .SetActive(false);
        gameOverPanel   .SetActive(false);
    }

    private void HandleGameStart()
    {
        startScreenPanel.SetActive(false);
        gameOverPanel   .SetActive(false);
        hudPanel        .SetActive(true);

        // Reset visuals
        _currentXPFill        = 0f;
        _targetXPFill         = 0f;
        xpBarFill.fillAmount  = 0f;
        timerBarFill.fillAmount = 0f;

        if (timerBarFill) timerBarFill.color = ColorTimerStart;
        if (levelText)    levelText.text      = "LVL 1";
        if (xpValueText)  xpValueText.text    = "0 / 100";
        if (timerText)    timerText.text       = "00:00";

        HideLevelUpPopup();
    }

    private void HandleGameOver()
    {
        hudPanel     .SetActive(false);
        gameOverPanel.SetActive(true);

        // Remplir l'écran game over avec les stats
        var gm = GameManager.Instance;
        if (finalTimeText)  finalTimeText.text  = FormatTime(gm.ElapsedTime);
        if (finalLevelText) finalLevelText.text  = $"Niveau {gm.CurrentLevel}";
        if (finalKillsText) finalKillsText.text  = $"{gm.KillCount} Ennemis tués";

        StartCoroutine(AnimateGameOverIn());
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Timer

    private void UpdateTimer(float elapsed)
    {
        if (timerText)
            timerText.text = FormatTime(elapsed);

        if (!timerBarFill) return;

        float t         = Mathf.Clamp01(elapsed / timerBarDuration);
        timerBarFill.fillAmount = t;
        timerBarFill.color      = GetTimerColor(t);

        // Pulse quand le timer dépasse 80 %
        if (t >= 0.8f && !_timerPulsing)
        {
            _timerPulsing = true;
            StartCoroutine(PulseTimerBar());
        }
    }

    /// Dégradé vert → jaune → rouge
    private Color GetTimerColor(float t)
    {
        if (t < 0.5f)
            return Color.Lerp(ColorTimerStart,  ColorTimerMiddle, t * 2f);
        else
            return Color.Lerp(ColorTimerMiddle, ColorTimerEnd,    (t - 0.5f) * 2f);
    }

    private IEnumerator PulseTimerBar()
    {
        while (GameManager.Instance != null && GameManager.Instance.State == GameState.Playing)
        {
            float t = Mathf.PingPong(Time.time * 3f, 1f);
            if (timerBarFill) timerBarFill.color = Color.Lerp(ColorTimerEnd, Color.white, t * 0.3f);
            yield return null;
        }
        _timerPulsing = false;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region XP Bar

    private void UpdateXPBar(int current, int max, float progress)
    {
        _targetXPFill = progress;

        if (xpValueText) xpValueText.text = $"{current} / {max} XP";
    }

    private void HandleLevelUp(int newLevel)
    {
        if (levelText) levelText.text = $"LVL {newLevel}";

        // Reset barre XP visuellement
        _currentXPFill       = 0f;
        _targetXPFill        = 0f;
        xpBarFill.fillAmount = 0f;

        // Afficher le popup Level Up
        if (_levelUpRoutine != null) StopCoroutine(_levelUpRoutine);
        _levelUpRoutine = StartCoroutine(ShowLevelUpPopup(newLevel));
    }

    private IEnumerator ShowLevelUpPopup(int newLevel)
    {
        if (!levelUpPopup || !levelUpCanvasGroup) yield break;

        if (levelUpLabel) levelUpLabel.text = $"LEVEL UP — {newLevel} !";

        levelUpPopup.gameObject.SetActive(true);

        // Fade in + scale up
        float duration = 0.35f;
        float elapsed  = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p  = elapsed / duration;
            levelUpCanvasGroup.alpha        = Mathf.Lerp(0f, 1f, p);
            levelUpPopup.localScale         = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one, p);
            yield return null;
        }

        yield return new WaitForSeconds(1.6f);

        // Fade out
        elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime;
            levelUpCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.4f);
            yield return null;
        }

        HideLevelUpPopup();
    }

    private void HideLevelUpPopup()
    {
        if (!levelUpPopup) return;
        levelUpPopup.gameObject.SetActive(false);
        if (levelUpCanvasGroup) levelUpCanvasGroup.alpha = 0f;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Game Over Animation

    private IEnumerator AnimateGameOverIn()
    {
        // Petit délai puis scale depuis 0
        yield return new WaitForSecondsRealtime(0.1f);

        var rt    = gameOverPanel.GetComponent<RectTransform>();
        var cg    = gameOverPanel.GetComponent<CanvasGroup>();

        if (rt && cg)
        {
            float dur = 0.5f, t = 0f;
            cg.alpha = 0f;
            rt.localScale = Vector3.one * 0.85f;

            while (t < dur)
            {
                t          += Time.unscaledDeltaTime;
                float p     = t / dur;
                cg.alpha        = Mathf.Lerp(0f, 1f, p);
                rt.localScale   = Vector3.Lerp(Vector3.one * 0.85f, Vector3.one, p);
                yield return null;
            }
            cg.alpha      = 1f;
            rt.localScale = Vector3.one;
        }
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Buttons

    private void SetupButtons()
    {
        if (startButton)   startButton  .onClick.AddListener(() => GameManager.Instance.StartGame());
        if (restartButton) restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        if (quitButton)    quitButton   .onClick.AddListener(() => GameManager.Instance.QuitGame());
    }

    // Appelables aussi depuis l'Inspector via OnClick()
    public void OnStartButton()   => GameManager.Instance.StartGame();
    public void OnRestartButton() => GameManager.Instance.RestartGame();
    public void OnQuitButton()    => GameManager.Instance.QuitGame();

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Utilities

    private static string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60);
        int s = (int)(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    #endregion
}
