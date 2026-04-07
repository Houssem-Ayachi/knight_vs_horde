using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// À attacher sur un GameObject "MenuManager" dans la scène "Main".
/// Gère uniquement les boutons du menu principal.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Boutons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (playButton) playButton.onClick.AddListener(OnPlay);
        if (quitButton) quitButton.onClick.AddListener(OnQuit);
    }

    public void OnPlay()  => GameManager.Instance.StartGame();
    public void OnQuit()  => GameManager.Instance.QuitGame();
}
