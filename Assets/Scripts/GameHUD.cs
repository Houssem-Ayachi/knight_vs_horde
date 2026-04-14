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

    // ── Boutons ───────────────────────────────────────────────────────────────
    [Header("Boutons")]
    [SerializeField] private Button menuButton;   // retour menu

    // ────────────────────────────────────────────────────────────────────────
    #region Lifecycle

    private void OnEnable()
    {
        GameManager.OnTimerUpdate += UpdateTimer;
    }

    private void OnDisable()
    {
        GameManager.OnTimerUpdate -= UpdateTimer;
    }

    private void Start()
    {
        if (menuButton) menuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());

        // Init visuals
        if (timerText)   timerText.text   = "00:00";
        if (timerBarFill) timerBarFill.fillAmount = 0f;
    }

    private void Update()
    {

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
    #region Utilitaires

    private static string FormatTime(float seconds)
    {
        int m = (int)(seconds / 60);
        int s = (int)(seconds % 60);
        return $"{m:00}:{s:00}";
    }

    #endregion
}
