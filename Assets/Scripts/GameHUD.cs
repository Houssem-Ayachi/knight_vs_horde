using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// À attacher sur un GameObject "GameHUD" dans la scène "Game".
/// — Timer incrémental (compte à l'infini, pas de limite)
/// — Barre qui complète un cycle toutes les 60 secondes (visuel de progression)
/// — Barre XP + niveau + popup Level Up
/// — Bouton Pause / Retour menu
/// </summary>
public class GameHUD : MonoBehaviour
{
    // ── Timer ─────────────────────────────────────────────────────────────────
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image           timerBarFill;   // cycle 60s
    [SerializeField] private float           timerCycleDuration = 60f; // 1 cycle = 60s

    private static readonly Color ColGreen  = new Color(0.18f, 0.88f, 0.42f);
    private static readonly Color ColYellow = new Color(1.00f, 0.80f, 0.10f);
    private static readonly Color ColRed    = new Color(1.00f, 0.22f, 0.22f);

    // ── XP ────────────────────────────────────────────────────────────────────
    [Header("XP Bar")]
    [SerializeField] private Image           xpBarFill;
    [SerializeField] private TextMeshProUGUI xpValueText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float           xpFillSpeed = 5f;

    // ── Level-Up Popup ────────────────────────────────────────────────────────
    [Header("Level-Up Popup")]
    [SerializeField] private RectTransform   levelUpPopup;
    [SerializeField] private TextMeshProUGUI levelUpLabel;
    [SerializeField] private CanvasGroup     levelUpCanvasGroup;

    // ── Boutons ───────────────────────────────────────────────────────────────
    [Header("Boutons")]
    [SerializeField] private Button menuButton;   // retour menu

    // ── Private ───────────────────────────────────────────────────────────────
    private float     _targetXP;
    private float     _currentXP;
    private Coroutine _levelUpRoutine;

    // ────────────────────────────────────────────────────────────────────────
    #region Lifecycle

    private void OnEnable()
    {
        GameManager.OnTimerUpdate += UpdateTimer;
        GameManager.OnXPChanged   += UpdateXP;
        GameManager.OnLevelUp     += ShowLevelUp;
    }

    private void OnDisable()
    {
        GameManager.OnTimerUpdate -= UpdateTimer;
        GameManager.OnXPChanged   -= UpdateXP;
        GameManager.OnLevelUp     -= ShowLevelUp;
    }

    private void Start()
    {
        if (menuButton) menuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());

        // Init visuals
        if (timerText)   timerText.text   = "00:00";
        if (levelText)   levelText.text   = "LVL 1";
        if (xpValueText) xpValueText.text = "0 / 100 XP";
        if (xpBarFill)   xpBarFill.fillAmount  = 0f;
        if (timerBarFill) timerBarFill.fillAmount = 0f;

        HideLevelUpPopup();
    }

    private void Update()
    {
        // Animation fluide barre XP
        if (xpBarFill && Mathf.Abs(_currentXP - _targetXP) > 0.001f)
        {
            _currentXP           = Mathf.Lerp(_currentXP, _targetXP, Time.deltaTime * xpFillSpeed);
            xpBarFill.fillAmount = _currentXP;
        }
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Timer

    private void UpdateTimer(float elapsed)
    {
        if (timerText)
            timerText.text = FormatTime(elapsed);

        if (!timerBarFill) return;

        // Barre qui boucle toutes les 60 secondes
        float cycleProgress     = (elapsed % timerCycleDuration) / timerCycleDuration;
        timerBarFill.fillAmount = cycleProgress;
        timerBarFill.color      = GetTimerColor(cycleProgress);
    }

    private Color GetTimerColor(float t)
    {
        if (t < 0.5f) return Color.Lerp(ColGreen,  ColYellow, t * 2f);
        else          return Color.Lerp(ColYellow,  ColRed,   (t - 0.5f) * 2f);
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region XP

    private void UpdateXP(int current, int max, float progress)
    {
        _targetXP = progress;
        if (xpValueText) xpValueText.text = $"{current} / {max} XP";
    }

    private void ShowLevelUp(int newLevel)
    {
        if (levelText) levelText.text = $"LVL {newLevel}";

        // Reset barre XP visuellement
        _currentXP = 0f;
        _targetXP  = 0f;
        if (xpBarFill) xpBarFill.fillAmount = 0f;

        if (_levelUpRoutine != null) StopCoroutine(_levelUpRoutine);
        _levelUpRoutine = StartCoroutine(AnimateLevelUp(newLevel));
    }

    private IEnumerator AnimateLevelUp(int newLevel)
    {
        if (!levelUpPopup || !levelUpCanvasGroup) yield break;

        if (levelUpLabel) levelUpLabel.text = $"LEVEL UP — {newLevel} !";
        levelUpPopup.gameObject.SetActive(true);

        // Fade in + scale
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            float p = t / 0.35f;
            levelUpCanvasGroup.alpha = p;
            levelUpPopup.localScale  = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one, p);
            yield return null;
        }

        yield return new WaitForSeconds(1.6f);

        // Fade out
        t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            levelUpCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.4f);
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
    #region Utilitaires

    private static string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60);
        int s = (int)(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    #endregion
}
