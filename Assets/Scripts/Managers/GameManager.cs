using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string SCENE_MENU = "Main";
    private const string SCENE_GAME = "Game";

    public GameState State { get; private set; } = GameState.Menu;
    public float ElapsedTime { get; private set; }
    public int KillCount { get; private set; }

    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action<float> OnTimerUpdate;
    public static event Action<int> OnKillCountChanged;
    [SerializeField] private PlayerLevel playerLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStats();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == SCENE_GAME && State != GameState.Playing)
            State = GameState.Playing;
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        ElapsedTime += Time.deltaTime;
        OnTimerUpdate?.Invoke(ElapsedTime);
    }

    public void StartGame()
    {
        ResetStats();
        playerLevel.ResetStats();
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_GAME);
        OnGameStart?.Invoke();
    }

    public void TriggerGameOver()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        ResetStats();
        playerLevel.ResetStats();
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SCENE_GAME);
        OnGameStart?.Invoke();
    }

    public void ReturnToMenu()
    {
        ResetStats();
        playerLevel.ResetStats();
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

    public void RegisterKill(int xpReward = 5)
    {
        if (State != GameState.Playing) return;
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
        playerLevel.AddXP(xpReward);
    }

    private void ResetStats()
    {
        ElapsedTime = 0f;
        KillCount = 0;
        State = GameState.Menu;
    }
}