using UnityEngine;

public class MockEnemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private GameObject xpOrbPrefab;

    private float moveSpeed;
    private int health;
    private int damageToPlayer;

    private bool isBouncing = false;

    public float bounceDistance = 200;

    private Rigidbody2D rb;
    private GameObject playerObj;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<Player>();

        moveSpeed = enemyData.moveSpeed;
        health = enemyData.health;
        damageToPlayer = enemyData.damageToPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(!isBouncing)
        {
            FollowPlayer();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag != "Player")
            return;

        Bounce(collision.transform.position);

        player.TakeDamage(damageToPlayer);

        ApplyDamage(50);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Weapon")
            return;

        int damageTaken = collision.gameObject.GetComponent<Weapon>().damage;
        ApplyDamage(damageTaken);

        Bounce(collision.transform.position);
    }

    private void Bounce(Vector3 colliderPosition)
    {
        Vector2 bounceDir = (transform.position - colliderPosition).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(bounceDir * bounceDistance);

        isBouncing = true;
        Invoke(nameof(StopBouncing), 0.5f);
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    // Méthode appelée quand l'ennemi meurt
    private void Die()
    {
        // Lâcher un orbe XP
        if (xpOrbPrefab != null)
        {
            // TODO: maybe change it from simple instantiation to pooling?
            Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
        }

        // TODO: if the pooling solution is implemented, instead of destroying the object return it to the pool
        // Détruire l'ennemi
        Destroy(gameObject);
    }

    private void StopBouncing()
    {
        isBouncing = false;
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
}
