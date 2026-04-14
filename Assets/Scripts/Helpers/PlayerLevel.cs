using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public static PlayerLevel Instance;

    [Header("XP / Leveling")]
    [SerializeField] private int baseXPPerLevel = 30;
    [SerializeField] private float xpScalingMultiplier = 1.1f;

    public int CurrentXP { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    public int XPToNextLevel { get; private set; }
    public float XPProgress => XPToNextLevel > 0 ? (float)CurrentXP / XPToNextLevel : 0f;

    public event Action<int, int, float> OnXPChanged;
    public event Action<int> OnLevelUp;

    private void Awake()
    {
        if(Instance != null)
            Destroy(this);
        Instance = this;

        DontDestroyOnLoad(gameObject);
        ResetStats();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        CurrentXP += amount;

        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            CurrentLevel++;
            XPToNextLevel = Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpScalingMultiplier, CurrentLevel - 1));
            OnLevelUp?.Invoke(CurrentLevel);
        }

        OnXPChanged?.Invoke(CurrentXP, XPToNextLevel, XPProgress);
    }

    public void ResetStats()
    {
        CurrentXP = 0;
        CurrentLevel = 1;
        XPToNextLevel = baseXPPerLevel;
    }
}