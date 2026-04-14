using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public GameObject easySpawner;
    public GameObject mediumSpawner;
    public GameObject hardSpawner;

    public GameObject Attack1Prefab;
    public GameObject Attack2Prefab;

    public PlayerLevel playerLevel;
    public GameObject playerAttacksContainer;

    private EDifficultyLevel difficultyLevel = EDifficultyLevel.EASY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        easySpawner.SetActive(true);

        mediumSpawner.SetActive(false);
        hardSpawner.SetActive(false);

        playerLevel.OnLevelUp += AdjustDifficulty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AdjustDifficulty(int currentLevel)
    {
        if(currentLevel >= 5 && difficultyLevel == EDifficultyLevel.EASY)
        {
            Debug.Log("changed to medium difficulty");

            difficultyLevel = EDifficultyLevel.MEDIUM;

            mediumSpawner.SetActive(true);

            easySpawner.SetActive(false);
            hardSpawner.SetActive(false);

            Instantiate(Attack1Prefab, playerAttacksContainer.transform.position, Quaternion.identity, playerAttacksContainer.transform);
        }

        if(currentLevel >= 10 && difficultyLevel == EDifficultyLevel.MEDIUM)
        {
            Debug.Log("changed to hard difficulty");

            difficultyLevel = EDifficultyLevel.HARD;

            hardSpawner.SetActive(true);

            easySpawner.SetActive(false);
            mediumSpawner.SetActive(false);

            Instantiate(Attack2Prefab, playerAttacksContainer.transform.position, Quaternion.identity, playerAttacksContainer.transform);
        }
    }
}
