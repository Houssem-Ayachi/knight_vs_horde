using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private int currentLevel = 1;
    private int xpAmountToNextLevel = 100;
    private int currentXpCollected = 0;

    public int CurrentLevel { get { return currentLevel; }}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddXP(int amount)
    {
        currentXpCollected += amount;

        if(currentXpCollected >= xpAmountToNextLevel)
        {
            int remainingXp = currentXpCollected - xpAmountToNextLevel;

            currentLevel++;

            currentXpCollected = remainingXp;

            Debug.Log("level up!! -> " + currentLevel);

            // TODO: increase the xpAmountToNextLevel attribute
        }
    }
}
