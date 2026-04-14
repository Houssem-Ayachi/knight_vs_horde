using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed;
    private int health;
    private int maxHealth;
    private float healthBarWidth = 0.31f;

    public PlayerDefaults playerDefaultStats;
    public GameObject HealthBar;
    public int xFacingDirection = 1;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 _direction = Vector2.zero;
    private Animator animator;
    private SpriteRenderer HealthBarSpriteRenderer;
    private PlayerLevel playerLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        HealthBarSpriteRenderer = HealthBar.GetComponent<SpriteRenderer>();
        playerLevel = GetComponent<PlayerLevel>();

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

            xFacingDirection = _direction.x < 0 ? -1 : 1;
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "xp_orb")
        {
            playerLevel.AddXP(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0) return; // Ne pas prendre de d�g�ts si d�j� mort

        health -= damage;

        if (health <= 0 && !animator.GetBool("isDead"))
        {
            // player is dead
            animator.SetBool("isDead", true);
            health = 0; // S'assurer que la vie ne devient pas n�gative
            
            // D�clencher le Game Over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
            // stop player from moving
            rb.linearVelocity = Vector2.up;
        }

        UpdateHealthBarSprite(damage);
    }
    
    // R�duit la taille du sprite de la barre de vie en fonction des d�g�ts subis
    void UpdateHealthBarSprite(int damage)
    {
        // Utiliser maxHealth au lieu de 100 cod� en dur
        float healthBarReductionAmount = healthBarWidth / maxHealth * damage;

        Vector2 hbSize = HealthBarSpriteRenderer.size;
        hbSize.x = Math.Clamp(hbSize.x - healthBarReductionAmount, 0, healthBarWidth);

        HealthBarSpriteRenderer.size = hbSize;
    }
}