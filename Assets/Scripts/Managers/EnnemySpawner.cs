using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject player;
    public PoolManager1 poolManager;

    [Header("Parametres de spawn")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistance;
    [SerializeField] private int enemiesPerSpawn;

    [Header("Limite d'ennemis")]
    [SerializeField] private int maxEnemies = 10;

    private float timer = 0;

    void Update()
    {
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
        return poolManager.GetActivePoolItems(EPoolItemType.Enemy);
    }

    void SpawnEnemy()
    {
        Vector2 pos = player.transform.position + (Vector3)(Random.insideUnitCircle.normalized * spawnDistance);

        GameObject enemy = poolManager.GetPoolItem(EPoolItemType.Enemy);

        if(enemy == null)
            return;

        enemy.transform.position = pos;
        enemy.transform.rotation = Quaternion.identity;

        // Informer l'ennemi de son pool
    }
}