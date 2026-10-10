using Slafurry.Systems.Pause;
using Slafurry.Systems.Scene;
using UnityEngine;
using UnityEngine.UI;

public class PauseHUD : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Button pauseButton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scenes")]
    [SerializeField] private SceneReference mainMenuScene;

    [Header("Pause")]
    [SerializeField] private string pauseKey = "PauseMenu";

    private void Awake()
    {
        pauseButton.onClick.AddListener(OnPause);
        resumeButton.onClick.AddListener(OnResume);
        mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void Start()
    {
        ShowPauseMenu(false);
    }

    private void OnDestroy()
    {
        pauseButton.onClick.RemoveListener(OnPause);
        resumeButton.onClick.RemoveListener(OnResume);
        mainMenuButton.onClick.RemoveListener(OnMainMenu);
    }

    private void OnPause()
    {
        Pause.On(pauseKey);
        ShowPauseMenu(true);
    }

    private void OnResume()
    {
        ShowPauseMenu(false);
        Pause.Off(pauseKey);
    }

    private void OnMainMenu()
    {
        SceneSystem.Load(mainMenuScene);
        Pause.Off(pauseKey);
    }

    private void ShowPauseMenu(bool show)
    {
        pauseMenu.SetActive(show);
        pauseButton.gameObject.SetActive(!show);
    }
}