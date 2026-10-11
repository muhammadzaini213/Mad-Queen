using System.Collections;
using Slafurry.Systems.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Slafurry.Systems.Bootstrap
{
    /// <summary>
    /// Placed in the Bootstrap scene (the first scene in Build Settings).
    /// Its job: make sure every GameSystem&lt;T&gt; (SceneLoader, AudioSystem, etc.)
    /// has finished Initialize() + PostInitialize(), then load the target scene.
    /// </summary>
    public class BootstrapLoader : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Name of the scene to load after all systems are ready.")]
        [SerializeField] private string targetSceneName = "MainMenu";

        [Header("Systems To Wait For")]
        [Tooltip("References to all GameSystems that must be ready before changing scene. " +
                 "Drag their components here (SceneLoader, AudioSystem, etc.).")]
        [SerializeField] private MonoBehaviour[] systemsToWaitFor;

        [Header("Optional")]
        [Tooltip("Extra delay (seconds) after all systems are ready, before loading the scene. Useful for a splash screen minimum duration.")]
        [SerializeField] private float minimumDelay = 0f;

        private void Start()
        {
            StartCoroutine(BootstrapRoutine());
        }

        private IEnumerator BootstrapRoutine()
        {
            // Wait 1 frame so every other system's Awake()/OnSingletonAwake()
            // (ordered earlier via Script Execution Order) finishes first.
            yield return null;

            // Initialize each system in order.
            foreach (var systemObj in systemsToWaitFor)
            {
                if (systemObj == null)
                {
                    Debug.LogWarning("[BootstrapLoader] A system slot is empty in the inspector, skipped.");
                    continue;
                }

                if (systemObj is IGameSystemLifecycle lifecycle)
                {
                    yield return StartCoroutine(lifecycle.Initialize());
                    lifecycle.PostInitialize();
                }
                else
                {
                    Debug.LogWarning($"[BootstrapLoader] {systemObj.GetType().Name} is not a GameSystem, skipped from the lifecycle wait.");
                }
            }

            if (minimumDelay > 0f)
                yield return new WaitForSecondsRealtime(minimumDelay);

            Debug.Log($"[BootstrapLoader] All systems ready. Loading scene '{targetSceneName}'.");

            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogError("[BootstrapLoader] targetSceneName is empty! Set it in the inspector.");
                yield break;
            }

            SceneSystem.Load(targetSceneName);
        }
    }

    /// <summary>
    /// Helper interface so BootstrapLoader can call Initialize()/PostInitialize()
    /// without knowing the generic type T of GameSystem&lt;T&gt;.
    /// Implement this in GameSystem&lt;T&gt; (see note below).
    /// </summary>
    public interface IGameSystemLifecycle
    {
        IEnumerator Initialize();
        void PostInitialize();
    }
}