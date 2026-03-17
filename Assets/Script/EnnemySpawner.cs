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

    private Transform player;
    private float timer;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (player == null || enemyPrefabs.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
            int toSpawn = Mathf.Min(enemiesPerSpawn, maxEnemies - currentEnemies);

            for (int i = 0; i < toSpawn; i++)
            {
                SpawnEnemy();
            }
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        Vector2 pos = player.position + (Vector3)(Random.insideUnitCircle.normalized * spawnDistance);
        GameObject enemy = Instantiate(enemyPrefabs[index], pos, Quaternion.identity);
        enemy.tag = "Enemy"; 
    }
}