using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    // Ce script délègue maintenant la gestion de l'XP au GameManager
    // pour que tous les systèmes (HUD, Upgrades, etc.) soient synchronisés

    public int CurrentLevel 
    { 
        get 
        { 
            if (GameManager.Instance != null)
                return GameManager.Instance.CurrentLevel;
            return 1;
        }
    }

    /// <summary>
    /// Ajoute de l'XP via le GameManager (qui gère le level up et les events)
    /// </summary>
    public void AddXP(int amount)
    {
        Debug.Log($"[PlayerLevel] AddXP appelé avec {amount} XP");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddXP(amount);
            Debug.Log($"[PlayerLevel] XP ajouté ! Total: {GameManager.Instance.CurrentXP}/{GameManager.Instance.XPToNextLevel}");
        }
        else
        {
            Debug.LogWarning("[PlayerLevel] GameManager.Instance est NULL !");
        }
    }
}
