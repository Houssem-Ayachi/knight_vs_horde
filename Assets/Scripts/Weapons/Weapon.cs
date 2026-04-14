using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 50;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Ennemy>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
