using System.Collections;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnnemiesData enemyData;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Poolable poolable;
    private int currentHealth;
    private bool isBouncing = false;

    public float bounceDistance = 50;

    [SerializeField] private Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        poolable = GetComponent<Poolable>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultMaterial = spriteRenderer.material;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        currentHealth = enemyData != null ? enemyData.maxHealth : 1;
        isBouncing = false;

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if(!isBouncing)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * enemyData.moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerTransform = collision.gameObject.GetComponent<Player>();
            if (playerTransform != null)
            {
                playerTransform.TakeDamage(enemyData.damageToPlayer);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public void Die()
    {
        GameObject xpOrb = PoolManager1.Instance.GetPoolItem(EPoolItemType.XpOrb);

        spriteRenderer.color = Color.white;

        // place the orb at the same position as this current enemy.
        xpOrb.transform.position = transform.position;

        poolable.ReturnToPool();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // TODO: make the enemy flash with a red color to indicate a hit.

        FlashWhite();
        Bounce(playerTransform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Bounce(Vector3 colliderPosition)
    {
        Vector2 bounceDir = (transform.position - colliderPosition).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(bounceDir * bounceDistance);

        isBouncing = true;
        Invoke(nameof(StopBouncing), 0.5f);
    }

    private void StopBouncing()
    {
        isBouncing = false;
    }

    public void FlashWhite()
    {
        Debug.Log("flashing white");
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.color = Color.softRed;
        yield return new WaitForSeconds(1f);
        spriteRenderer.color = Color.white;
    }
}