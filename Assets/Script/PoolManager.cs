using UnityEngine;

/// <summary>
/// Gestionnaire centralisé pour tous les pools d'objets du jeu.
/// Singleton persistant entre les scènes.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [Header("XP Orb Pooling")]
    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private int xpOrbPoolSize = 100;

    [Header("Enemy Pooling (Optional)")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemyPoolSize = 50;
    [SerializeField] private bool useEnemyPooling = false;

    private XPOrbPool xpOrbPool;
    private EnemyPool[] enemyPools;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        // Initialiser le pool d'orbes XP
        if (xpOrbPrefab != null)
        {
            GameObject poolObject = new GameObject("XPOrb Pool");
            poolObject.transform.SetParent(transform);

            xpOrbPool = poolObject.AddComponent<XPOrbPool>();
            xpOrbPool.Initialize(xpOrbPrefab, xpOrbPoolSize, true);
            Debug.Log($"XPOrb Pool initialisé avec {xpOrbPoolSize} objets.");
        }
        else
        {
            Debug.LogWarning("XPOrb Prefab non assigné dans le PoolManager !");
        }

        // Initialiser les pools d'ennemis (optionnel)
        if (useEnemyPooling && enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            enemyPools = new EnemyPool[enemyPrefabs.Length];

            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                if (enemyPrefabs[i] != null)
                {
                    GameObject poolObject = new GameObject($"Enemy Pool - {enemyPrefabs[i].name}");
                    poolObject.transform.SetParent(transform);

                    EnemyPool pool = poolObject.AddComponent<EnemyPool>();
                    pool.Initialize(enemyPrefabs[i], enemyPoolSize, true);

                    enemyPools[i] = pool;
                    Debug.Log($"Enemy Pool '{enemyPrefabs[i].name}' initialisé avec {enemyPoolSize} objets.");
                }
            }
        }
    }

    /// <summary>Récupère le pool d'orbes XP.</summary>
    public XPOrbPool GetXPOrbPool()
    {
        return xpOrbPool;
    }

    /// <summary>Récupère un pool d'ennemis spécifique par index.</summary>
    public EnemyPool GetEnemyPool(int index)
    {
        if (enemyPools != null && index >= 0 && index < enemyPools.Length)
        {
            return enemyPools[index];
        }
        return null;
    }

    /// <summary>Récupère tous les pools d'ennemis.</summary>
    public EnemyPool[] GetAllEnemyPools()
    {
        return enemyPools;
    }

    /// <summary>Spawn un orbe XP à une position donnée.</summary>
    public GameObject SpawnXPOrb(Vector3 position, int xpValue = 20)
    {
        if (xpOrbPool == null)
        {
            Debug.LogError("XPOrb Pool non initialisé !");
            return null;
        }

        GameObject orb = xpOrbPool.GetOrb(position);
        if (orb != null)
        {
            XPOrbs orbScript = orb.GetComponent<XPOrbs>();
            if (orbScript != null)
            {
                orbScript.SetOrbPool(xpOrbPool);
                orbScript.SetXPValue(xpValue);
            }
        }
        return orb;
    }
}
