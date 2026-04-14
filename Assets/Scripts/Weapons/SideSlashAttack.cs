using UnityEngine;

public class SideSlashAttack : MonoBehaviour
{
    // how far from the player the attack should be
    public float offsetFromPlayer = 0.6f;
    public int damage = 50;

    SpriteRenderer spriteRenderer;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided with smth");
        if(collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Ennemy>().TakeDamage(damage);
        }
    }

    public void initAttack()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
    }

    public void StartAttack(Vector2 position, int xDirection = 1)
    {
        transform.position = new Vector2(
            position.x + (xDirection * offsetFromPlayer),
            position.y
        );

        if(xDirection == 1)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        gameObject.SetActive(true);

        animator.Play("SideSlashAttack");
    }

    // referenced by the animation clip.
    public void OnAnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
