using UnityEngine;

public class Ennemies : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnemyData enemyData;

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
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * enemyData.moveSpeed;
    }

    void ApplyAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyData.enemyColor;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }

    // Méthode appelée quand l'ennemi meurt
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

    // Méthode pour prendre des dégâts (à appeler depuis un système de combat)
    public void TakeDamage(int damage)
    {
        // Pour l'instant, on fait mourir l'ennemi directement
        Die();
    }
}