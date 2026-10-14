using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Slafurry.Systems.Audio;

public class DialogHUD : MonoBehaviour
{
    public bool IsBusy => isOpen || closedFrame == Time.frameCount;

    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject nextNotifier;
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private Key advanceKey = Key.E;

    [Header("Audio")]
    [Tooltip("Typewriter sound. Loops while text is typing, stops when the line is done.")]
    [SerializeField] private SFXReference typeSFX;

    [Tooltip("Sound played when advancing to the next dialog line.")]
    [SerializeField] private SFXReference continueSFX;

    [Tooltip("Sound played when the dialog ends and closes.")]
    [SerializeField] private SFXReference endSFX;

    private DialogBucket bucket;
    private Action onFinished;
    private Coroutine typingRoutine;
    private int index;
    private bool isOpen;
    private bool isTyping;
    private bool isTypeSFXPlaying;
    private int startedFrame = -1;
    private int closedFrame = -1;

    private void Awake()
    {
        container.SetActive(false);
        nextNotifier.SetActive(false);
    }

    private void OnDisable()
    {
        StopTypeSFX();
    }

    private void Update()
    {
        if (!isOpen || Time.frameCount == startedFrame)
            return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null || !keyboard[advanceKey].wasPressedThisFrame)
            return;

        if (isTyping)
            CompleteLine();
        else
            NextLine();
    }

    public void Play(DialogBucket newBucket, Action finished = null)
    {
        if (isOpen || newBucket == null || newBucket.Lines.Count == 0)
            return;

        bucket = newBucket;
        onFinished = finished;
        index = 0;
        isOpen = true;
        startedFrame = Time.frameCount;

        container.SetActive(true);
        ShowLine();
    }

    private void ShowLine()
    {
        DialogLine line = bucket.Lines[index];

        nameText.text = line.speakerName;

        dialogText.text = line.text;
        dialogText.maxVisibleCharacters = 0;

        nextNotifier.SetActive(false);

        typingRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;

        dialogText.ForceMeshUpdate();

        int total = dialogText.textInfo.characterCount;

        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(
            1f / Mathf.Max(1f, charactersPerSecond)
        );

        if (total > 0)
            StartTypeSFX();

        for (int i = 1; i <= total; i++)
        {
            dialogText.maxVisibleCharacters = i;
            yield return delay;
        }

        FinishTyping();
    }

    private void CompleteLine()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        FinishTyping();
    }

    private void FinishTyping()
    {
        typingRoutine = null;
        isTyping = false;

        dialogText.maxVisibleCharacters = int.MaxValue;
        nextNotifier.SetActive(true);

        StopTypeSFX();
    }

    private void NextLine()
    {
        index++;

        if (index >= bucket.Lines.Count)
        {
            Close();
        }
        else
        {
            PlaySFX(continueSFX);
            ShowLine();
        }
    }

    private void Close()
    {
        Action callback = onFinished;

        isOpen = false;
        isTyping = false;
        closedFrame = Time.frameCount;

        bucket = null;
        onFinished = null;

        container.SetActive(false);
        nextNotifier.SetActive(false);

        StopTypeSFX();
        PlaySFX(endSFX);

        callback?.Invoke();
    }

    // ======================== AUDIO ========================

    private void StartTypeSFX()
    {
        if (typeSFX == null || isTypeSFXPlaying)
            return;

        if (AudioSystem.Instance == null)
            return;
        Audio.PlaySFX2D(typeSFX, loop: true);
        isTypeSFXPlaying = true;
    }

    private void StopTypeSFX()
    {
        if (!isTypeSFXPlaying)
            return;

        if (AudioSystem.Instance == null)
            return;
        Audio.StopSFX(typeSFX);
        isTypeSFXPlaying = false;
    }

    private static void PlaySFX(SFXReference sfx)
    {
        if (sfx == null)
            return;
        if (AudioSystem.Instance == null)
            return;
        Audio.PlaySFX2D(sfx);
    }
}