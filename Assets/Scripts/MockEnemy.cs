using UnityEngine;

public class MockEnemy : MonoBehaviour
{
    private bool isBouncing = false;

    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(!isBouncing)
        {
            rb.linearVelocity = Vector2.right * 4;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag != "Player") return;

        Vector2 bounceDir = (transform.position - collision.transform.position).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(bounceDir * 100);

        isBouncing = true;
        Invoke(nameof(StopBouncing), 0.5f);
    }

    private void StopBouncing()
    {
        isBouncing = false;
    }
}
