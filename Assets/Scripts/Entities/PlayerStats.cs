using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Stats de base")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private int baseMaxHealth = 100;
    [SerializeField] private float baseWeaponDamage = 1f;
    [SerializeField] private float baseArmor = 0f;
    [SerializeField] private float baseAttackSpeed = 1f;

    // Stats actuelles
    public float CurrentSpeed { get; private set; }
    public int CurrentMaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public float WeaponDamageMultiplier { get; private set; }
    public float Armor { get; private set; }
    public float AttackSpeedMultiplier { get; private set; }

    // Event déclenché quand la santé change
    public static event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetStats();
    }

    public void ResetStats()
    {
        CurrentSpeed = baseSpeed;
        CurrentMaxHealth = baseMaxHealth;
        CurrentHealth = baseMaxHealth;
        WeaponDamageMultiplier = baseWeaponDamage;
        Armor = baseArmor;
        AttackSpeedMultiplier = baseAttackSpeed;
    }

    // ═══════════════════════════════════════════════════════════════
    // MÉTHODES D'AMÉLIORATION (appelées par les upgrades)
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Augmente les dégâts des armes (en pourcentage)
    /// </summary>
    public void IncreaseWeaponDamage(float percentageIncrease)
    {
        WeaponDamageMultiplier += percentageIncrease / 100f;
    }

    /// <summary>
    /// Augmente les HP max et soigne
    /// </summary>
    public void IncreaseMaxHealth(int amount)
    {
        CurrentMaxHealth += amount;
        CurrentHealth += amount;
        
        // Notifier que la santé a changé
        OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
    }

    /// <summary>
    /// Augmente la vitesse de déplacement (en pourcentage)
    /// </summary>
    public void IncreaseSpeed(float percentageIncrease)
    {
        CurrentSpeed += baseSpeed * (percentageIncrease / 100f);
    }

    /// <summary>
    /// Augmente l'armure (réduit les dégâts reçus)
    /// </summary>
    public void IncreaseArmor(float amount)
    {
        Armor = Mathf.Min(Armor + amount, 75f);
    }

    /// <summary>
    /// Augmente la vitesse d'attaque (en pourcentage)
    /// </summary>
    public void IncreaseAttackSpeed(float percentageIncrease)
    {
        AttackSpeedMultiplier += percentageIncrease / 100f;
    }

    /// <summary>
    /// Soigne le joueur
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, CurrentMaxHealth);
        
        // Notifier que la santé a changé
        OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
    }

    // ═══════════════════════════════════════════════════════════════
    // MÉTHODES DE COMBAT
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Inflige des dégâts au joueur
    /// </summary>
    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        
        // Notifier que la santé a changé
        OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
    }

    /// <summary>
    /// Calcule les dégâts infligés avec le multiplicateur
    /// </summary>
    public int CalculateDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * WeaponDamageMultiplier);
    }

    /// <summary>
    /// Calcule les dégâts reçus après réduction d'armure
    /// </summary>
    public int CalculateDamageTaken(int incomingDamage)
    {
        float reduction = 1f - (Armor / 100f);
        return Mathf.Max(1, Mathf.RoundToInt(incomingDamage * reduction)); // Minimum 1 dégât
    }
}