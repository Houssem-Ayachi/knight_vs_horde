using System;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    private PlayerStats Stats => PlayerStats.Instance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        HealthBarSpriteRenderer = HealthBar.GetComponent<SpriteRenderer>();
        playerLevel = GetComponent<PlayerLevel>();
    }

    void OnEnable()
    {
        // S'abonner à l'event de changement de santé
        PlayerStats.OnHealthChanged += UpdateHealthBarSprite;
    }

    void OnDisable()
    {
        // Se désabonner
        PlayerStats.OnHealthChanged -= UpdateHealthBarSprite;
    }

    void Update()
    {
        if (Stats != null && Stats.CurrentHealth > 0)
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
        if (Stats != null)
        {
            rb.linearVelocity = _direction.normalized * Stats.CurrentSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "xp_orb")
        {
            playerLevel.AddXP(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (Stats == null || Stats.CurrentHealth <= 0) return;

        int actualDamage = Stats.CalculateDamageTaken(damage);
        Stats.TakeDamage(actualDamage);

        if (Stats.CurrentHealth <= 0 && !animator.GetBool("isDead"))
        {
            animator.SetBool("isDead", true);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// Met à jour la barre de vie (appelé automatiquement quand la santé change)
    /// </summary>
    void UpdateHealthBarSprite(int currentHealth, int maxHealth)
    {
        if (HealthBarSpriteRenderer == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;
        
        Vector2 hbSize = HealthBarSpriteRenderer.size;
        hbSize.x = healthBarWidth * healthPercentage;

        HealthBarSpriteRenderer.size = hbSize;
    }
}