using UnityEngine;

public class RandomFireBallManager : MonoBehaviour
{
    public float spawnInterval;
    public int numberOfBallsToFire = 1;
    public GameObject fireBallPrefab;
    private GameObject player;

    private float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // calculating the passed time since the last enemy wave spawn
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFireBall();

            timer = 0f;
        }
    }

    void SpawnFireBall()
    {
        // calculating a random direction for the fire ball to take.
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        GameObject fireBall = Instantiate(fireBallPrefab, player.transform.position, Quaternion.identity);

        FireBall fireBallScript = fireBall.GetComponent<FireBall>();
        
        fireBallScript.player = player;
        fireBallScript.SetDirection(randomDirection);
    }
}
