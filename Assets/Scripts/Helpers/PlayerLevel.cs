using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
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
    /// Ajoute de l'XP via le GameManager (qui g�re le level up et les events)
    /// </summary>
    public void AddXP(int amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddXP(amount);
        }
    }
}
