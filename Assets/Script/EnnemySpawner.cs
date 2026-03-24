/*
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Paramètres de spawn")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistance;
    [SerializeField] private int enemiesPerSpawn;

    [Header("Limite d'ennemis")]
    [SerializeField] private int maxEnemies = 20;

    [Header("Object Pooling")]
    [SerializeField] private bool usePooling = true;

    private Transform player;
    private float timer;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // Vérifier que le PoolManager est disponible si le pooling est activé
        if (usePooling && PoolManager.Instance == null)
        {
            Debug.LogWarning("PoolManager non trouvé ! Le pooling des ennemis ne fonctionnera pas.");
        }
    }

    void Update()
    {
        if (player == null || enemyPrefabs.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            int currentEnemies = CountActiveEnemies();
            int toSpawn = Mathf.Min(enemiesPerSpawn, maxEnemies - currentEnemies);

            for (int i = 0; i < toSpawn; i++)
            {
                SpawnEnemy();
            }
            timer = 0f;
        }
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
        
        // Fallback : compter les ennemis dans la scène
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        Vector2 pos = player.position + (Vector3)(Random.insideUnitCircle.normalized * spawnDistance);

        GameObject enemy = null;

        if (usePooling && PoolManager.Instance != null)
        {
            // Utiliser le pool du PoolManager
            EnemyPool pool = PoolManager.Instance.GetEnemyPool(index);
            if (pool != null)
            {
                enemy = pool.GetEnemy();
                if (enemy != null)
                {
                    enemy.transform.position = pos;
                    enemy.transform.rotation = Quaternion.identity;
                    
                    // Informer l'ennemi de son pool
                    Ennemies enemyScript = enemy.GetComponent<Ennemies>();
                    if (enemyScript != null)
                    {
                        enemyScript.SetEnemyPool(pool);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Aucun pool trouvé pour l'ennemi à l'index {index}. Assurez-vous que le PoolManager a les bons prefabs assignés.");
            }
        }
        
        // Fallback : méthode classique sans pooling
        if (enemy == null)
        {
            enemy = Instantiate(enemyPrefabs[index], pos, Quaternion.identity);
            enemy.tag = "Enemy";
        }
    }
}
*/