using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private int health;
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

        if(_direction.x != 0)
        {
            spriteRenderer.flipX = _direction.x < 0;
        }

        if(!_direction.Equals(Vector2.zero))
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Enemy")
        {
            UpdateHealth();
        }
    }

    void ApplyHit()
    {

    }

    void UpdateHealth()
    {
        int damage = 50;

        health -= damage;

        if(health <= 0 && !animator.GetBool("isDead"))
        {
            animator.SetBool("isDead", true);
        }

        UpdateHealthBarSprite(damage);
    }

    void UpdateHealthBarSprite(int damage)
    {
        float healthBarReductionAmount = healthBarWidth / 100 * damage;

        Vector2 hbSize = HealthBarSpriteRenderer.size;
        hbSize.x = Math.Clamp(hbSize.x - healthBarReductionAmount, 0, 31);

        HealthBarSpriteRenderer.size = hbSize;
    }
}
