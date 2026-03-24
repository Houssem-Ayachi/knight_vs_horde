using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    [Header("Déclenchement")]
    [Tooltip("XP total nécessaire pour déclencher cette vague")]
    public int xpThreshold = 0;

    [Header("Configuration de spawn")]
    [Tooltip("Types d'ennemis à spawner dans cette vague")]
    public GameObject[] enemyTypes;

    [Tooltip("Pourcentage de chance de spawn pour chaque type (doit totaliser 100)")]
    public int[] spawnWeights;

    [Tooltip("Nombre d'ennemis par spawn")]
    public int enemiesPerSpawn = 3;

    [Tooltip("Intervalle entre les spawns (en secondes)")]
    public float spawnInterval = 2f;

    [Tooltip("Nombre maximum d'ennemis actifs simultanément")]
    public int maxEnemies = 20;

    [Header("Statut")]
    public bool isActive = false;
    public bool isCompleted = false;
}

/// <summary>
/// Gestionnaire de vagues d'ennemis basé sur l'XP accumulée du joueur.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Configuration des vagues")]
    [SerializeField] private Wave[] waves;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] private bool usePooling = true;

    private Transform player;
    private int currentWaveIndex = 0;
    private float spawnTimer = 0f;
    private int totalXPGained = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // S'abonner aux events du GameManager
        GameManager.OnXPChanged += OnXPGained;
    }

    void OnDestroy()
    {
        GameManager.OnXPChanged -= OnXPGained;
    }

    void Update()
    {
        if (player == null || waves.Length == 0)
            return;

        // Vérifier si on doit activer une nouvelle vague
        CheckWaveActivation();

        // Spawner des ennemis pour la vague active
        if (currentWaveIndex < waves.Length && waves[currentWaveIndex].isActive)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= waves[currentWaveIndex].spawnInterval)
            {
                SpawnEnemiesForCurrentWave();
                spawnTimer = 0f;
            }
        }
    }

    void OnXPGained(int currentXP, int maxXP, float progress)
    {
        // Calculer l'XP total accumulé depuis le début
        // Note: currentXP est l'XP vers le prochain niveau, pas le total
        // On utilise le GameManager pour obtenir le total réel
        if (GameManager.Instance != null)
        {
            totalXPGained = CalculateTotalXP();
            Debug.Log($"XP total accumulé : {totalXPGained}");
        }
    }
    // Cette méthode calcule l'XP total accumulé en fonction du niveau actuel et de l'XP vers le prochain niveau
    int CalculateTotalXP()
    {
        if (GameManager.Instance == null) return 0;

        int level = GameManager.Instance.CurrentLevel;
        int currentXP = GameManager.Instance.CurrentXP;
        int baseXP = 100; // Doit correspondre à la valeur dans GameManager
        float multiplier = 1.5f; // Doit correspondre à la valeur dans GameManager

        // Calculer l'XP total nécessaire pour atteindre le niveau actuel
        int totalXP = 0;
        for (int i = 1; i < level; i++)
        {
            totalXP += Mathf.RoundToInt(baseXP * Mathf.Pow(multiplier, i - 1));
        }

        // Ajouter l'XP actuel vers le prochain niveau
        totalXP += currentXP;

        return totalXP;
    }

    void CheckWaveActivation()
    {
        for (int i = currentWaveIndex; i < waves.Length; i++)
        {
            if (totalXPGained >= waves[i].xpThreshold && !waves[i].isActive)
            {
                ActivateWave(i);
                break;
            }
        }
    }
    // Active la vague spécifiée et désactive les vagues précédentes
    void ActivateWave(int waveIndex)
    {
        if (waveIndex >= waves.Length) return;

        waves[waveIndex].isActive = true;
        currentWaveIndex = waveIndex;

        Debug.Log($"🌊 VAGUE {waveIndex + 1} ACTIVÉE ! (XP Seuil: {waves[waveIndex].xpThreshold})");

        // Désactiver les vagues précédentes
        for (int i = 0; i < waveIndex; i++)
        {
            waves[i].isActive = false;
            waves[i].isCompleted = true;
        }
    }
    // Spawner des ennemis pour la vague active en respectant la limite d'ennemis
    void SpawnEnemiesForCurrentWave()
    {
        Wave currentWave = waves[currentWaveIndex];

        // Vérifier la limite d'ennemis
        int currentEnemies = CountActiveEnemies();
        int toSpawn = Mathf.Min(currentWave.enemiesPerSpawn, currentWave.maxEnemies - currentEnemies);

        for (int i = 0; i < toSpawn; i++)
        {
            SpawnRandomEnemy(currentWave);
        }
    }
    // Spawner un ennemi aléatoire basé sur les types et les poids de la vague
    void SpawnRandomEnemy(Wave wave)
    {
        if (wave.enemyTypes.Length == 0) return;

        // Sélectionner un type d'ennemi basé sur les poids
        int enemyIndex = SelectWeightedRandom(wave.spawnWeights);
        if (enemyIndex >= wave.enemyTypes.Length) enemyIndex = 0;

        GameObject enemyPrefab = wave.enemyTypes[enemyIndex];
        Vector2 spawnPos = GetRandomSpawnPosition();

        GameObject enemy = null;

        if (usePooling && PoolManager.Instance != null)
        {
            // Trouver l'index du prefab dans le PoolManager
            int poolIndex = GetPoolIndexForPrefab(enemyPrefab);
            if (poolIndex >= 0)
            {
                EnemyPool pool = PoolManager.Instance.GetEnemyPool(poolIndex);
                if (pool != null)
                {
                    enemy = pool.GetEnemy();
                    if (enemy != null)
                    {
                        enemy.transform.position = spawnPos;
                        enemy.transform.rotation = Quaternion.identity;

                        Ennemies enemyScript = enemy.GetComponent<Ennemies>();
                        if (enemyScript != null)
                        {
                            enemyScript.SetEnemyPool(pool);
                        }
                    }
                }
            }
        }

        // Fallback : instantiation classique
        if (enemy == null)
        {
            enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.tag = "Enemy";
        }
    }

    // Méthode pour sélectionner un index basé sur des poids
    int SelectWeightedRandom(int[] weights)
    {
        if (weights == null || weights.Length == 0) return 0;

        int totalWeight = 0;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }

        return 0;
    }
    // Méthode pour trouver l'index du pool correspondant à un prefab donné
    int GetPoolIndexForPrefab(GameObject prefab)
    {
        if (PoolManager.Instance == null) return -1;

        // Cette méthode suppose que les prefabs dans le PoolManager sont dans le même ordre
        // Tu devras peut-être ajuster selon ton implémentation
        EnemyPool[] pools = PoolManager.Instance.GetAllEnemyPools();
        if (pools == null) return -1;

        for (int i = 0; i < pools.Length; i++)
        {
            // Comparaison par nom de prefab (à améliorer si nécessaire)
            if (pools[i] != null && pools[i].name.Contains(prefab.name))
            {
                return i;
            }
        }

        return -1;
    }

    Vector2 GetRandomSpawnPosition()
    {
        if (player == null) return Vector2.zero;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)player.position + randomDirection * spawnDistance;
    }

    int CountActiveEnemies()
    {
        if (usePooling && PoolManager.Instance != null)
        {
            EnemyPool[] pools = PoolManager.Instance.GetAllEnemyPools();
            if (pools != null)
            {
                int count = 0;
                foreach (EnemyPool pool in pools)
                {
                    if (pool != null)
                        count += pool.GetActiveCount();
                }
                return count;
            }
        }

        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Méthode utilitaire pour obtenir des infos sur la vague actuelle
    public Wave GetCurrentWave()
    {
        if (currentWaveIndex < waves.Length)
            return waves[currentWaveIndex];
        return null;
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
}
