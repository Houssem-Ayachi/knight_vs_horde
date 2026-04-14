using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Gère l'affichage du menu d'upgrade au level up
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject upgradeButtonPrefab;

    [Header("Configuration")]
    [SerializeField] private int numberOfChoices = 3;

    private List<GameObject> activeButtons = new List<GameObject>();

    // Event déclenché quand une upgrade est sélectionnée
    public static event Action<UpgradeData> OnUpgradeSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[UpgradeManager] Awake - Instance créée");
    }

    private void OnEnable()
    {
        GameManager.OnLevelUp += ShowUpgradeMenu;
        Debug.Log("[UpgradeManager] OnEnable - Abonné à OnLevelUp");
    }

    private void OnDisable()
    {
        GameManager.OnLevelUp -= ShowUpgradeMenu;
        Debug.Log("[UpgradeManager] OnDisable - Désabonné de OnLevelUp");
    }

    private void Start()
    {
        // Masquer le panel au démarrage
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Debug.Log("[UpgradeManager] Start - Initialisé");
    }

    /// <summary>
    /// Affiche le menu avec des upgrades aléatoires
    /// </summary>
    public void ShowUpgradeMenu(int newLevel)
    {
        Debug.Log($"[UpgradeManager] ShowUpgradeMenu appelé ! Niveau: {newLevel}");

        if (upgradePanel == null || upgradeButtonPrefab == null)
        {
            Debug.LogError("[UpgradeManager] Références manquantes !");
            return;
        }

        // Pause
        Time.timeScale = 0f;

        // Nettoyer les anciens boutons
        ClearButtons();

        // Obtenir des upgrades aléatoires via la Factory
        List<UpgradeData> randomUpgrades = UpgradeFactory.GetRandomUpgrades(numberOfChoices);

        // Créer les boutons
        foreach (UpgradeData upgrade in randomUpgrades)
        {
            CreateUpgradeButton(upgrade);
        }

        // Afficher
        upgradePanel.SetActive(true);
        Debug.Log($"[UpgradeManager] Panel activé avec {randomUpgrades.Count} upgrades !");
    }

    private void CreateUpgradeButton(UpgradeData upgrade)
    {
        GameObject buttonObj = Instantiate(upgradeButtonPrefab, buttonsContainer);
        activeButtons.Add(buttonObj);

        UpgradeButton buttonScript = buttonObj.GetComponent<UpgradeButton>();
        if (buttonScript != null)
        {
            buttonScript.Setup(upgrade, this);
        }
    }

    private void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button != null)
                Destroy(button);
        }
        activeButtons.Clear();
    }

    /// <summary>
    /// Appelé quand le joueur clique sur une upgrade
    /// </summary>
    public void SelectUpgrade(UpgradeData upgrade)
    {
        Debug.Log($"[UpgradeManager] Upgrade sélectionnée: {upgrade.title}");

        // Appliquer l'upgrade
        upgrade.ApplyUpgrade();

        // Fermer le menu
        ClearButtons();
        upgradePanel.SetActive(false);

        // Reprendre le jeu
        Time.timeScale = 1f;

        // Notifier les autres systèmes
        OnUpgradeSelected?.Invoke(upgrade);
    }

    // Méthode de test
    [ContextMenu("Test - Ouvrir Menu Upgrade")]
    public void TestOpenMenu()
    {
        ShowUpgradeMenu(1);
    }
}