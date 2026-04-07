using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnnemiesData : ScriptableObject
{
    [Header("Paramètres de déplacement")]
    [Tooltip("Vitesse de déplacement de l'ennemi")]
    public float moveSpeed = 3f;

    [Header("Paramètres de vie")]
    [Tooltip("Points de vie maximum de l'ennemi")]
    public int maxHealth = 50;

    [Header("Paramètres de dégâts")]
    [Tooltip("Dégâts infligés au joueur par contact")]
    public int damageToPlayer = 10;

    [Header("Récompenses")]
    [Tooltip("Points d'XP donnés au joueur quand cet ennemi meurt")]
    public int xpReward = 20;

    [Header("Feedback visuel")]
    [Tooltip("Durée de l'immobilisation et du changement de couleur lors d'un coup")]
    public float hitStunDuration = 0.5f;

    [Tooltip("Couleur affichée quand l'ennemi subit des dégâts")]
    public Color hitColor = Color.white;

    [Header("Apparence")]
    [Tooltip("Couleur du sprite de l'ennemi")]
    public Color enemyColor = Color.red;

    [Tooltip("Échelle du sprite de l'ennemi")]
    public Vector3 enemyScale = Vector3.one;
}