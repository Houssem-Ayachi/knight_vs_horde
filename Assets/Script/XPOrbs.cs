using UnityEngine;

public class XPOrbs : MonoBehaviour
{
    [Header("Valeur XP")]
    [Tooltip("Quantité d'XP donnée au joueur")]
    [SerializeField] private int xpValue = 20;

    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement de l'orbe vers le joueur")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Distance à laquelle l'orbe commence à être attiré par le joueur")]
    [SerializeField] private float attractionRange = 3f;

    private Transform player;
    private bool isBeingAttracted = false;
    private XPOrbPool orbPool;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void OnEnable()
    {
        isBeingAttracted = false;

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
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attractionRange)
        {
            isBeingAttracted = true;
        }

        if (isBeingAttracted)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Donner l'XP au joueur via le GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddXP(xpValue);
                Debug.Log($"Orbe XP ramassé ! +{xpValue} XP");
            }

            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
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
            Debug.LogWarning("Aucun XPOrbPool trouvé, destruction de l'orbe");
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