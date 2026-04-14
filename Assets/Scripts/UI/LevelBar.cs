using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelBar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image xpBarBackground;
    [SerializeField] private Image xpBarFill;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private PlayerLevel playerLevel;

    [Header("Animation")]
    [SerializeField] private float fillSpeed = 5f;

    [Header("Couleurs")]
    [SerializeField] private Color backgroundColor = new Color(0.3f, 0.7f, 1f);   // Bleu
    [SerializeField] private Color fillColor = new Color(1f, 0.2f, 0.2f);    // Rouge

    private float targetFillAmount = 0f;

    void Start()
    {
        if (xpBarBackground != null)
        {
            xpBarBackground.color = backgroundColor;
        }

        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = 0f;
            xpBarFill.color = fillColor;
        }

        if (levelText != null)
            levelText.text = "Nv. 1";

        if (xpText != null)
            xpText.text = "0 / 100";
    }

    void OnEnable()
    {
        playerLevel.OnXPChanged += OnXPChanged;
        playerLevel.OnLevelUp += OnLevelUp;
    }

    void OnDisable()
    {
        playerLevel.OnXPChanged -= OnXPChanged;
        playerLevel.OnLevelUp -= OnLevelUp;
    }

    void Update()
    {
        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = Mathf.Lerp(
                xpBarFill.fillAmount, 
                targetFillAmount, 
                Time.deltaTime * fillSpeed
            );
        }
    }

    void OnXPChanged(int currentXP, int maxXP, float progress)
    {
        targetFillAmount = progress;

        if (xpText != null)
            xpText.text = $"{currentXP} / {maxXP}";
    }

    void OnLevelUp(int newLevel)
    {
        if (levelText != null)
            levelText.text = $"Nv. {newLevel}";

        targetFillAmount = 0f;
        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = 0f;
        }
    }
}