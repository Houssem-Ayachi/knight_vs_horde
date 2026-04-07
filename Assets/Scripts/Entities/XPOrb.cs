using Unity.Mathematics;
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("Valeur XP")]
    [Tooltip("Quantite d'XP donnee au joueur")]
    [SerializeField] private int xpValue = 20;

    [Header("Parametres de deplacement")]
    [Tooltip("Vitesse de deplacement de l'orbe vers le joueur")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Distance a laquelle l'orbe commence a etre attire par le joueur")]
    [SerializeField] private float attractionRange = 3f;

    private Transform player;
    private Rigidbody2D rb;

    private bool shouldFollowPlayer = false;

    public float speedMultiplier = 1.05f;
    private XPOrbPool orbPool;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void OnEnable()
    {
        shouldFollowPlayer = false;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attractionRange)
        {
            shouldFollowPlayer = true;
        }
    }

    void FixedUpdate()
    {
        if(!shouldFollowPlayer)
            return;

       FollowPlayer(); 
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        moveSpeed = math.clamp(moveSpeed * speedMultiplier, 0, 20);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Donner l'XP au joueur via le GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddXP(xpValue);
            }

            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        // TODO: i think this if statement is not needed caus when the object is fetched from the pool this orbPool attribute is set inside the PoolManager.SpawnXPOrb method
        if (orbPool == null)
        {
            orbPool = GetComponentInParent<XPOrbPool>();

            if (orbPool == null && PoolManager.Instance != null)
            {
                orbPool = PoolManager.Instance.GetXPOrbPool();
            }
        }

        if (orbPool != null)
        {
            orbPool.ReturnOrb(gameObject);
        }
        else
        {
            Debug.LogWarning("Aucun XPOrbPool trouv�, destruction de l'orbe");
            Destroy(gameObject);
        }
    }

    public void SetOrbPool(XPOrbPool pool)
    {
        orbPool = pool;
    }

    public void SetXPValue(int value)
    {
        xpValue = value;
    }
}