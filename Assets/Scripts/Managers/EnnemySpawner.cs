using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Parametres de spawn")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistance;
    [SerializeField] private int enemiesPerSpawn;

    [Header("Limite d'ennemis")]
    [SerializeField] private int maxEnemies = 20;

    [Header("Object Pooling")]
    [SerializeField] private bool usePooling = true;

    private Transform player;
    private float timer = 0;

    void Start()
    {
        // finding the player's gameObject in the scene tree
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // V�rifier que le PoolManager est disponible si le pooling est activ�
        if (PoolManager.Instance == null)
        {
            Debug.LogWarning("PoolManager non trouv� ! Le pooling des ennemis ne fonctionnera pas.");
        }
    }

    void Update()
    {
        if (player == null || enemyPrefabs.Length == 0)
            return;

        // calculating the passed time since the last enemy wave spawn
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
        
        // Fallback : compter les ennemis dans la sc�ne
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
                    Ennemy enemyScript = enemy.GetComponent<Ennemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.SetEnemyPool(pool);
                    }
                }
            }
        }

        // Fallback : m�thode classique sans pooling
        if (enemy == null)
        {
            enemy = Instantiate(enemyPrefabs[index], pos, Quaternion.identity);
            enemy.tag = "Enemy";
        }
    }
}