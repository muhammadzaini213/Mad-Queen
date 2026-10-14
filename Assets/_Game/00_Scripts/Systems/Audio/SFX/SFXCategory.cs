using UnityEngine;

[System.Serializable]
public class SFXEffect
{
    public string groupID;
    public AudioClip[] clips;
    [Range(0f, 1f)]
    public float volume = 1f;
    public int maxSimultaneous = 3;

    [Header("Clip Selection")]
    [Tooltip("Checked = clips picked randomly. Unchecked = sequential. Also applies in loop mode (clip swaps each pass).")]
    public bool isRandom = true;

    [Header("Fade")]
    [Tooltip("Default fade-in duration (seconds). 0 = instant full volume.")]
    [Range(0f, 5f)]
    public float fadeIn = 0f;
    [Tooltip("Default fade-out duration (seconds). 0 = stop immediately.")]
    [Range(0f, 5f)]
    public float fadeOut = 0f;

    [Header("Distance (used in PlaySFX3D)")]
    [Tooltip("Sound only changes VOLUME with distance, no left/right panning. Suited for 2D/top-down games.")]
    public bool distanceOnlyNoPanning = true;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
    [Tooltip("Within this distance volume is full 100%.")]
    public float minDistance = 1f;
    [Tooltip("Beyond this distance the sound is no longer audible at all.")]
    public float maxDistance = 15f;

    // Runtime only
    [System.NonSerialized] private int sequentialIndex;
    [System.NonSerialized] private int lastIndex = -1;

    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0) return null;
        if (clips.Length == 1) return clips[0];

        int index;

        if (isRandom)
        {
            // Avoid the same clip twice in a row
            index = Random.Range(0, clips.Length);
            if (index == lastIndex)
                index = (index + 1 + Random.Range(0, clips.Length - 1)) % clips.Length;
        }
        else
        {
            index = sequentialIndex % clips.Length;
            sequentialIndex = (index + 1) % clips.Length;
        }

        lastIndex = index;
        return clips[index];
    }
}

namespace Slafurry.Systems.Audio
{
    [CreateAssetMenu(fileName = "SFX Category", menuName = "Audio/SFX Category")]
    public class SFXCategory : ScriptableObject
    {
        public string categoryName;
        public SFXEffect[] effects;
        public int poolSize = 8;
    }
}