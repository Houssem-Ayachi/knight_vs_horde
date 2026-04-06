using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Custom/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Parametres de deplacement")]
    [Tooltip("Vitesse de deplacement de l'ennemi")]
    public float moveSpeed;

    [Header("Parametres de vie")]
    [Tooltip("Points de vie maximum de l'ennemi")]
    public int health;

    [Header("Parametres de degets")]
    [Tooltip("Degets infliges au joueur par contact")]
    public int damageToPlayer;

    [Tooltip("Intervalle en secondes entre chaque deget inflige")]
    public float damageInterval;

    [Header("Apparence")]
    [Tooltip("Couleur du sprite de l'ennemi")]
    public Color enemyColor;
}