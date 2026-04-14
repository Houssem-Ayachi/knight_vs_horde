using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDefaults", menuName = "Game/PlayerDefaults")]
public class PlayerDefaults : ScriptableObject
{
    [Header("Statistiques du joueur")]
    [Tooltip("Points de vie maximum du joueur")]
    public int health = 100;
    
    [Tooltip("Vitesse de déplacement du joueur")]
    public float speed = 5f;
}
