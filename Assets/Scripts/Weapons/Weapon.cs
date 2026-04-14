using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Action onHit;

    public int damage = 50;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Ennemy>().TakeDamage(damage);

            onHit?.Invoke();
        }
    }
}
