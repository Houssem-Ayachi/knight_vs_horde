using System.Collections;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnnemiesData enemyData;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyPool enemyPool;
    private int currentHealth;
    private Color originalColor;

    private bool isBouncing = false;

    public float bounceDistance = 50;

    void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData non assigné sur " + gameObject.name);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // ApplyAppearance();
        originalColor = enemyData.enemyColor;
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

        // Restaurer la couleur originale
        if (spriteRenderer != null && enemyData != null)
        {
            // spriteRenderer.color = enemyData.enemyColor;
            originalColor = enemyData.enemyColor;
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

    void ApplyAppearance()
    {
        if (spriteRenderer != null)
        {
            // spriteRenderer.color = enemyData.enemyColor;
        }
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
        if(collision.tag != "Weapon")
            return;

        int damageTaken = collision.gameObject.GetComponent<Weapon>().damage;
        TakeDamage(damageTaken);

        Bounce(collision.transform.position);
    }

    public void Die()
    {
        // Lâcher un orbe XP en utilisant le PoolManager
        if (PoolManager.Instance != null && enemyData != null)
        {
            PoolManager.Instance.SpawnXPOrb(transform.position, enemyData.xpReward);
        }
        else
        {
            Debug.LogWarning("PoolManager non trouvé ! Impossible de créer un orbe XP.");
        }

        // Enregistrer le kill dans le GameManager
        if (GameManager.Instance != null && enemyData != null)
        {
            GameManager.Instance.RegisterKill(enemyData.xpReward);
        }

        ReturnToPool();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Effet de hit : changement de couleur et immobilisation
        // TODO: apply the hitStunEffect later when i fix it.
        // StartCoroutine(HitStunEffect());

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

    // private IEnumerator HitStunEffect()
    // {
    //     if (enemyData == null) yield break;

    //     // Marquer comme étourdi pour arrêter le mouvement
    //     isStunned = true;

    //     // Arrêter le mouvement
    //     if (rb != null)
    //     {
    //         rb.linearVelocity = Vector2.zero;
    //     }

    //     // Changer la couleur en blanc (ou la couleur définie)
    //     if (spriteRenderer != null)
    //     {
    //         spriteRenderer.color = enemyData.hitColor;
    //     }

    //     // Attendre la durée du stun
    //     yield return new WaitForSeconds(enemyData.hitStunDuration);

    //     // Restaurer la couleur originale
    //     if (spriteRenderer != null)
    //     {
    //         spriteRenderer.color = originalColor;
    //     }

    //     // Permettre à nouveau le mouvement
    //     isStunned = false;
    // }

    void ReturnToPool()
    {
        // Arrêter toutes les coroutines en cours (pour éviter les bugs)
        StopAllCoroutines();
        isBouncing = false;

        if (enemyPool == null)
        {
            enemyPool = GetComponentInParent<EnemyPool>();

            if (enemyPool == null)
            {
                enemyPool = FindFirstObjectByType<EnemyPool>();
            }
        }

        if (enemyPool != null)
        {
            enemyPool.ReturnEnemy(gameObject);
        }
        else
        {
            Debug.LogWarning("Aucun EnemyPool trouvé, destruction de l'ennemi");
            Destroy(gameObject);
        }
    }

    public void SetEnemyPool(EnemyPool pool)
    {
        enemyPool = pool;
    }
}