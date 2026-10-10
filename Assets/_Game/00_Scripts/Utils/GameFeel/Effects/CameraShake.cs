using Unity.Cinemachine;
using UnityEngine;

namespace Slafurry.Utils.GameFeel
{
    public class CameraShake : MonoBehaviour, IGameFeelEffect
    {
        [Header("Impulse Source")]
        [SerializeField] private CinemachineImpulseSource impulseSource;

        [Header("Default Shake Settings")]
        [SerializeField] private float defaultAmplitude = 1f;
        [SerializeField] private float defaultFrequency = 1f;
        [SerializeField] private float defaultDuration = 0.2f;

        private void Awake()
        {
            CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
        }

        private void Start()
        {
            if (impulseSource != null) return;

            if (!TryGetComponent(out impulseSource))
                Debug.LogError("[CameraShake] No CinemachineImpulseSource found on this GameObject!");
        }

        public void Shake(float amplitude, float frequency, float duration)
        {
            if (impulseSource == null)
            {
                Debug.LogWarning("[CameraShake] Impulse Source not assigned!");
                return;
            }

            impulseSource.ImpulseDefinition.FrequencyGain = frequency;
            impulseSource.ImpulseDefinition.ImpulseDuration = duration;

            impulseSource.GenerateImpulseWithForce(amplitude);
        }

        public void PlayEffect()
        {
            Shake(defaultAmplitude, defaultFrequency, defaultDuration);
        }

        public void StopEffect()
        {
            CinemachineImpulseManager.Instance.Clear();
        }
    }
}