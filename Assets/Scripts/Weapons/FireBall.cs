using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Vector2 direction = Vector2.zero;
    public float speed;
    public GameObject player;

    private Rigidbody2D rb;
    private Weapon weaponComponent;

    // the distance (from the player) at which the fire ball should be destroyed.
    private float deathDoor = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        weaponComponent = GetComponent<Weapon>();
        weaponComponent.onHit = handleEnemyHit;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;

        if(isAtDeathDoor())
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;

        // calculating the angle of this new direction to make the fire ball face it.
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;

        // applying the angle to the Z axis of the game object.
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // checks if the fire ball is too faraway from the player.
    private bool isAtDeathDoor()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        return distanceFromPlayer >= deathDoor;
    }

    private void handleEnemyHit()
    {

    }
}
