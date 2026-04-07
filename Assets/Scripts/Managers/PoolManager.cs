using UnityEngine;

/// <summary>
/// Gestionnaire centralis� pour tous les pools d'objets du jeu.
/// Singleton persistant entre les sc�nes.
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
        }
        else
        {
            Debug.LogWarning("XPOrb Prefab non assign� dans le PoolManager !");
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
                    Debug.Log($"Enemy Pool '{enemyPrefabs[i].name}' initialis� avec {enemyPoolSize} objets.");
                }
            }
        }
    }

    /// <summary>R�cup�re le pool d'orbes XP.</summary>
    public XPOrbPool GetXPOrbPool()
    {
        return xpOrbPool;
    }

    /// <summary>R�cup�re un pool d'ennemis sp�cifique par index.</summary>
    public EnemyPool GetEnemyPool(int index)
    {
        if (enemyPools != null && index >= 0 && index < enemyPools.Length)
        {
            return enemyPools[index];
        }
        return null;
    }

    /// <summary>R�cup�re tous les pools d'ennemis.</summary>
    public EnemyPool[] GetAllEnemyPools()
    {
        return enemyPools;
    }

    /// <summary>Spawn un orbe XP � une position donn�e.</summary>
    public GameObject SpawnXPOrb(Vector3 position, int xpValue = 20)
    {
        GameObject orb = xpOrbPool.GetOrb(position);

        orb.SetActive(true);

        if (orb != null)
        {
            XPOrb orbScript = orb.GetComponent<XPOrb>();
            if (orbScript != null)
            {
                orbScript.SetOrbPool(xpOrbPool);
                orbScript.SetXPValue(xpValue);
            }
        }
        return orb;
    }
}
