using UnityEngine;

public class XPOrbs : MonoBehaviour
{
    [Header("Valeur XP")]
    [Tooltip("Quantité d'XP donnée au joueur")]
    [SerializeField] private int xpValue;

    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement de l'orbe vers le joueur")]
    [SerializeField] private float moveSpeed;

    [Tooltip("Distance à laquelle l'orbe commence à être attiré par le joueur")]
    [SerializeField] private float attractionRange;

    private Transform player;
    private bool isBeingAttracted = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
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
            Debug.Log("Orbe XP ramassé !");
            Destroy(gameObject);
        }
    }

    public void SetXPValue(int value)
    {
        xpValue = value;
    }
}