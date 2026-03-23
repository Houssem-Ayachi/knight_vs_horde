using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, GameOver }

/// <summary>
/// Singleton persistant entre les scènes — gère l'état, le timer incrémental et l'XP.
/// Noms de scènes attendus : "Main" (menu) et "Game" (jeu).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ── Noms des scènes ──────────────────────────────────────────────────────
    private const string SCENE_MENU = "Main";
    private const string SCENE_GAME = "Game";

    // ── XP Settings ─────────────────────────────────────────────────────────
    [Header("XP / Leveling")]
    [SerializeField] private int   baseXPPerLevel      = 100;
    [SerializeField] private float xpScalingMultiplier = 1.5f;

    // ── Public read-only state ───────────────────────────────────────────────
    public GameState State         { get; private set; } = GameState.Menu;
    public float     ElapsedTime   { get; private set; }
    public int       CurrentXP     { get; private set; }
    public int       CurrentLevel  { get; private set; } = 1;
    public int       XPToNextLevel { get; private set; }
    public int       KillCount     { get; private set; }

    public float XPProgress => XPToNextLevel > 0 ? (float)CurrentXP / XPToNextLevel : 0f;

    // ── Events ───────────────────────────────────────────────────────────────
    public static event Action                OnGameStart;
    public static event Action                OnGameOver;
    public static event Action<float>         OnTimerUpdate;      // secondes écoulées
    public static event Action<int, int, float> OnXPChanged;      // current, max, progress
    public static event Action<int>           OnLevelUp;          // nouveau niveau
    public static event Action<int>           OnKillCountChanged; // total kills

    // ────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStats();
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        ElapsedTime += Time.deltaTime;
        OnTimerUpdate?.Invoke(ElapsedTime);
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Lance le jeu et charge la scène Game.</summary>
    public void StartGame()
    {
        ResetStats();
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_GAME);
        OnGameStart?.Invoke();
    }

    /// <summary>Déclenche le Game Over et revient au menu.</summary>
    public void TriggerGameOver()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    /// <summary>Rejouer — recharge la scène Game.</summary>
    public void RestartGame()
    {
        ResetStats();
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_GAME);
        OnGameStart?.Invoke();
    }

    /// <summary>Retourner au menu principal.</summary>
    public void ReturnToMenu()
    {
        ResetStats();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_MENU);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>Ajouter des points XP (depuis les ennemis tués, etc.)</summary>
    public void AddXP(int amount)
    {
        if (State != GameState.Playing || amount <= 0) return;

        CurrentXP += amount;

        // Level-up loop (en cas de gros gain d'XP d'un coup)
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP     -= XPToNextLevel;
            CurrentLevel++;
            XPToNextLevel  = Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpScalingMultiplier, CurrentLevel - 1));
            OnLevelUp?.Invoke(CurrentLevel);
        }

        OnXPChanged?.Invoke(CurrentXP, XPToNextLevel, XPProgress);
    }

    /// <summary>Enregistrer un kill ennemi (appeler depuis l'ennemi mort)</summary>
    public void RegisterKill(int xpReward = 20)
    {
        if (State != GameState.Playing) return;
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
        AddXP(xpReward);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private void ResetStats()
    {
        ElapsedTime   = 0f;
        CurrentXP     = 0;
        CurrentLevel  = 1;
        KillCount     = 0;
        XPToNextLevel = baseXPPerLevel;
        State         = GameState.Menu;
    }
}
