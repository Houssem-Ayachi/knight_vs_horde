using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script pour le prefab du bouton d'upgrade
/// </summary>
public class UpgradeButton : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private UpgradeData currentUpgrade;
    private UpgradeManager upgradeManager;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    /// <summary>
    /// Configure le bouton avec les données de l'upgrade
    /// </summary>
    public void Setup(UpgradeData upgrade, UpgradeManager manager)
    {
        currentUpgrade = upgrade;
        upgradeManager = manager;

        // Titre
        if (titleText != null)
            titleText.text = upgrade.title;

        // Description
        if (descriptionText != null)
            descriptionText.text = upgrade.description;

        // Bouton
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (upgradeManager != null && currentUpgrade != null)
        {
            upgradeManager.SelectUpgrade(currentUpgrade);
        }
    }
}
