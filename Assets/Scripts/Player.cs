using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private int health;
    private int maxHealth;
    private float healthBarWidth = 0.31f;

    public PlayerDefaults playerDefaultStats;
    public GameObject HealthBar;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 _direction = Vector2.zero;
    private Animator animator;
    private SpriteRenderer HealthBarSpriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        HealthBarSpriteRenderer = HealthBar.GetComponent<SpriteRenderer>();

        speed = playerDefaultStats.speed;
        health = playerDefaultStats.health;
        maxHealth = playerDefaultStats.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            CalculateDirection();
        }
    }

    private void CalculateDirection()
    {
        _direction = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (_direction.x != 0)
        {
            spriteRenderer.flipX = _direction.x < 0;
        }

        if (!_direction.Equals(Vector2.zero))
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = _direction.normalized * speed;
    }
    
    // Applique les dégâts au joueur et met à jour la barre de vie
    public void TakeDamage(int damage)
    {
        if (health <= 0) return; // Ne pas prendre de dégâts si déjà mort

        health -= damage;
        Debug.Log($"Joueur prend {damage} dégâts. Vie restante : {health}/{maxHealth}");

        if (health <= 0 && !animator.GetBool("isDead"))
        {
            animator.SetBool("isDead", true);
            health = 0; // S'assurer que la vie ne devient pas négative
            
            // Déclencher le Game Over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }

        UpdateHealthBarSprite(damage);
    }
    
    // Réduit la taille du sprite de la barre de vie en fonction des dégâts subis
    void UpdateHealthBarSprite(int damage)
    {
        // Utiliser maxHealth au lieu de 100 codé en dur
        float healthBarReductionAmount = healthBarWidth / maxHealth * damage;

        Vector2 hbSize = HealthBarSpriteRenderer.size;
        hbSize.x = Math.Clamp(hbSize.x - healthBarReductionAmount, 0, healthBarWidth);

        HealthBarSpriteRenderer.size = hbSize;
    }
}