using System.Collections;
using UnityEngine;

public class Ennemies : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnnemiesData enemyData;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyPool enemyPool;
    private int currentHealth;
    private bool isStunned = false;
    private Color originalColor;

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
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Aucun joueur trouvé ! Assurez-vous que le joueur a le tag 'Player'");
        }

        ApplyAppearance();
        originalColor = enemyData.enemyColor;
    }

    void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        currentHealth = enemyData != null ? enemyData.maxHealth : 1;
        isStunned = false;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // Restaurer la couleur originale
        if (spriteRenderer != null && enemyData != null)
        {
            spriteRenderer.color = enemyData.enemyColor;
            originalColor = enemyData.enemyColor;
        }
    }

    void Update()
    {
        if (player != null && !isStunned)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * enemyData.moveSpeed;
    }

    void ApplyAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyData.enemyColor;
        }
        transform.localScale = enemyData.enemyScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(enemyData.damageToPlayer);
                Debug.Log($"L'ennemi inflige {enemyData.damageToPlayer} dégâts au joueur !");
            }
            // L'ennemi NE MEURT PLUS automatiquement, seulement si TakeDamage() le tue
        }
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
        Debug.Log($"{gameObject.name} prend {damage} dégâts. Vie restante : {currentHealth}/{enemyData.maxHealth}");
        
        // Effet de hit : changement de couleur et immobilisation
        StartCoroutine(HitStunEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitStunEffect()
    {
        if (enemyData == null) yield break;

        // Marquer comme étourdi pour arrêter le mouvement
        isStunned = true;

        // Arrêter le mouvement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Changer la couleur en blanc (ou la couleur définie)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyData.hitColor;
        }

        // Attendre la durée du stun
        yield return new WaitForSeconds(enemyData.hitStunDuration);

        // Restaurer la couleur originale
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Permettre à nouveau le mouvement
        isStunned = false;
    }

    void ReturnToPool()
    {
        // Arrêter toutes les coroutines en cours (pour éviter les bugs)
        StopAllCoroutines();
        isStunned = false;

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