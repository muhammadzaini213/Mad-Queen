using Slafurry.Systems.Scene;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button exitButton;

    [Header("Scenes")]
    [SerializeField] private SceneReference startScene;
    [SerializeField] private SceneReference settingsScene;
    [SerializeField] private SceneReference aboutScene;

    [Header("Continue")]
    [SerializeField] private string savedSceneKey = "LastScene";

    private void Awake()
    {
        startButton.onClick.AddListener(OnStart);
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        aboutButton.onClick.AddListener(OnAbout);
        exitButton.onClick.AddListener(OnExit);
    }

    private void Start()
    {
        continueButton.interactable = HasSave();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStart);
        continueButton.onClick.RemoveListener(OnContinue);
        settingsButton.onClick.RemoveListener(OnSettings);
        aboutButton.onClick.RemoveListener(OnAbout);
        exitButton.onClick.RemoveListener(OnExit);
    }

    private void OnStart()
    {
        SceneSystem.Load(startScene);
    }

    private void OnContinue()
    {
        if (!HasSave()) return;

        SceneSystem.Load(PlayerPrefs.GetString(savedSceneKey));
    }

    private void OnSettings()
    {
        SceneSystem.Load(settingsScene);
    }

    private void OnAbout()
    {
        SceneSystem.Load(aboutScene);
    }

    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool HasSave()
    {
        return PlayerPrefs.HasKey(savedSceneKey)
               && !string.IsNullOrEmpty(PlayerPrefs.GetString(savedSceneKey));
    }
}