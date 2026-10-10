using Slafurry.Systems.Scene;
using UnityEngine;
using UnityEngine.UI;

public class AboutMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button backButton;

    [Header("Scenes")]
    [SerializeField] private SceneReference mainMenuScene;

    private void Awake()
    {
        backButton.onClick.AddListener(BacktoMain);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(BacktoMain);
    }

    private void BacktoMain()
    {
        SceneSystem.Load(mainMenuScene);
    }
}