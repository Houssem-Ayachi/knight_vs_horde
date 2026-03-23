using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnnemiesData : ScriptableObject
{
    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement de l'ennemi")]
    public float moveSpeed;

    [Header("Paramètres de vie")]
    [Tooltip("Points de vie maximum de l'ennemi")]
    public int maxHealth;

    [Header("Paramètres de dégâts")]
    [Tooltip("Dégâts infligés au joueur par contact")]
    public int damageToPlayer;

    [Tooltip("Intervalle en secondes entre chaque dégât infligé")]
    public float damageInterval;

    [Header("Apparence")]
    [Tooltip("Couleur du sprite de l'ennemi")]
    public Color enemyColor;

    [Tooltip("Échelle du sprite de l'ennemi")]
    public Vector3 enemyScale;
}