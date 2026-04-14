using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Définit les types d'upgrades disponibles
/// </summary>
public enum UpgradeType
{
    WeaponDamage,
    MaxHealth,
    Speed,
    Armor,
    HealthRegen,
    AttackSpeed
}

/// <summary>
/// Classe contenant les données d'une upgrade
/// </summary>
public class UpgradeData
{
    public string title;
    public string description;
    public UpgradeType type;
    public float value;
    public Action applyAction;

    public void ApplyUpgrade()
    {
        applyAction?.Invoke();
    }
}

/// <summary>
/// Génère automatiquement les upgrades basées sur les UpgradeType
/// </summary>
public static class UpgradeFactory
{
    // ???????????????????????????????????????????????????????????????
    // CONFIGURATION DES UPGRADES
    // ???????????????????????????????????????????????????????????????
    
    private static readonly Dictionary<UpgradeType, UpgradeConfig> upgradeConfigs = new Dictionary<UpgradeType, UpgradeConfig>
    {
        { UpgradeType.WeaponDamage, new UpgradeConfig("Lame Affûtée",   "Dégâts +{0}%",             15f) },
        { UpgradeType.MaxHealth,    new UpgradeConfig("Vitalité",       "HP max +{0}",              25f) },
        { UpgradeType.Speed,        new UpgradeConfig("Célérité",       "Vitesse +{0}%",            10f) },
        { UpgradeType.Armor,        new UpgradeConfig("Peau de Fer",    "Dégâts reçus -{0}%",       10f) },
        { UpgradeType.HealthRegen,  new UpgradeConfig("Second Souffle", "Récupère {0} HP",          20f) },
        { UpgradeType.AttackSpeed,  new UpgradeConfig("Frénésie",       "Vitesse d'attaque +{0}%",  15f) }
    };

    private class UpgradeConfig
    {
        public string title;
        public string descriptionFormat;
        public float value;

        public UpgradeConfig(string title, string descriptionFormat, float value)
        {
            this.title = title;
            this.descriptionFormat = descriptionFormat;
            this.value = value;
        }
    }

    public static List<UpgradeData> GenerateAllUpgrades()
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();

        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            UpgradeData upgrade = CreateUpgrade(type);
            if (upgrade != null)
            {
                upgrades.Add(upgrade);
            }
        }

        return upgrades;
    }

    private static UpgradeData CreateUpgrade(UpgradeType type)
    {
        if (!upgradeConfigs.ContainsKey(type))
        {
            Debug.LogWarning($"[UpgradeFactory] Pas de config pour {type}");
            return null;
        }

        UpgradeConfig config = upgradeConfigs[type];

        return new UpgradeData
        {
            type = type,
            title = config.title,
            description = string.Format(config.descriptionFormat, config.value),
            value = config.value,
            applyAction = () => ApplyUpgradeEffect(type, config.value)
        };
    }

    private static void ApplyUpgradeEffect(UpgradeType type, float value)
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("[UpgradeFactory] PlayerStats.Instance est NULL !");
            return;
        }

        switch (type)
        {
            case UpgradeType.WeaponDamage:
                PlayerStats.Instance.IncreaseWeaponDamage(value);
                Debug.Log($"?? Dégâts +{value}%");
                break;

            case UpgradeType.MaxHealth:
                PlayerStats.Instance.IncreaseMaxHealth((int)value);
                Debug.Log($"?? HP max +{value}");
                break;

            case UpgradeType.Speed:
                PlayerStats.Instance.IncreaseSpeed(value);
                Debug.Log($"? Vitesse +{value}%");
                break;

            case UpgradeType.Armor:
                PlayerStats.Instance.IncreaseArmor(value);
                Debug.Log($"??? Armure +{value}%");
                break;

            case UpgradeType.HealthRegen:
                PlayerStats.Instance.Heal((int)value);
                Debug.Log($"?? Soigné de {value} HP");
                break;

            case UpgradeType.AttackSpeed:
                PlayerStats.Instance.IncreaseAttackSpeed(value);
                Debug.Log($"?? Vitesse d'attaque +{value}%");
                break;
        }
    }

    public static List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> allUpgrades = GenerateAllUpgrades();

        // Mélanger (Fisher-Yates shuffle)
        for (int i = allUpgrades.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = allUpgrades[i];
            allUpgrades[i] = allUpgrades[j];
            allUpgrades[j] = temp;
        }

        int resultCount = Mathf.Min(count, allUpgrades.Count);
        return allUpgrades.GetRange(0, resultCount);
    }
}
