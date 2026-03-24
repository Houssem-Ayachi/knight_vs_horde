using UnityEngine;

public class Ennemies : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnnemiesData enemyData;

    [Header("XP Drop")]
    [SerializeField] private GameObject xpOrbPrefab;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData non assigné sur " + gameObject.name);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Trouver le joueur dans la scène
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Aucun joueur trouvé ! Assurez-vous que le joueur a le tag 'Player'");
        }

        // Appliquer l'apparence depuis le ScriptableObject
        ApplyAppearance();
    }

    void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Force le calcul en 2D (ignore Z)
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
            Debug.Log("Collision avec le joueur !");
            Die();
        }
    }

    public void Die()
    {
        // Lâcher un orbe XP
        if (xpOrbPrefab != null)
        {
            Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
        }

        // Détruire l'ennemi
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Die();
    }
}