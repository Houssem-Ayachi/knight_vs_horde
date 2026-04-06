using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelTracker : MonoBehaviour
{

    public GameObject player;
    private Text levelText;
    private PlayerLevel playerLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelText = GetComponent<Text>();

        Debug.Log(levelText);

        playerLevel = player.GetComponent<PlayerLevel>();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdatePlayerLevelText();
    }

    private void UpdatePlayerLevelText()
    {
        string newLevelText = "";
        if(playerLevel.CurrentLevel < 10)
        {
            newLevelText = "0" + playerLevel.CurrentLevel;
        }
        else
        {
            newLevelText = playerLevel.CurrentLevel.ToString();
        }

        levelText.text = newLevelText;
    }
}
