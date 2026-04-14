using UnityEngine;

public class SideSlashAttackManager : MonoBehaviour
{
    public float attackInterval = 2f;

    [SerializeField] private GameObject sideSlashAttackPrefab;
    private Player player;
    private GameObject sideSlashAttackInstance;
    private SideSlashAttack sideSlashAttackScript;

    private float timer = 0;

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        sideSlashAttackInstance = Instantiate(sideSlashAttackPrefab, transform.position, Quaternion.identity, transform);

        sideSlashAttackInstance.SetActive(false);

        sideSlashAttackScript = sideSlashAttackInstance.GetComponent<SideSlashAttack>();
    }

    void Update()
    {
        // calculating the passed time since the last enemy wave spawn
        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            SpawnSideSlash();

            timer = 0f;
        }
    }


    void SpawnSideSlash()
    {
        sideSlashAttackScript.initAttack();
        sideSlashAttackScript.StartAttack(player.transform.position, player.xFacingDirection);
    }
}
