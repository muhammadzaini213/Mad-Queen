using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Slafurry.Systems.Audio;

public class DialogHUD : MonoBehaviour
{
    public static DialogHUD Instance { get; private set; }

    public static bool IsBusy =>
        Instance != null && (Instance.isOpen || Instance.closedFrame == Time.frameCount);

    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject nextNotifier;
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private Key advanceKey = Key.E;

    [Header("Audio")]
    [Tooltip("Suara typewriter. Diputar loop selama teks mengetik, berhenti saat baris selesai.")]
    [SerializeField] private SFXReference typeSFX;
    [Tooltip("Suara saat lanjut ke baris dialog berikutnya.")]
    [SerializeField] private SFXReference continueSFX;
    [Tooltip("Suara saat dialog selesai dan ditutup.")]
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
        Instance = this;
        container.SetActive(false);
        nextNotifier.SetActive(false);
    }

    private void OnDisable()
    {
        StopTypeSFX();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (!isOpen || Time.frameCount == startedFrame) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard[advanceKey].wasPressedThisFrame) return;

        if (isTyping)
            CompleteLine();
        else
            NextLine();
    }

    public void Play(DialogBucket newBucket, Action finished = null)
    {
        if (isOpen || newBucket == null || newBucket.Lines.Count == 0) return;

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
        portraitImage.sprite = line.portrait;
        portraitImage.enabled = line.portrait != null;
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
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1f / Mathf.Max(1f, charactersPerSecond));

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
        if (typeSFX == null || isTypeSFXPlaying) return;

        Audio.PlaySFX2D(typeSFX, loop: true);
        isTypeSFXPlaying = true;
    }

    private void StopTypeSFX()
    {
        if (!isTypeSFXPlaying) return;

        Audio.StopSFX(typeSFX);
        isTypeSFXPlaying = false;
    }

    // Referensi opsional: kosong = tidak ada suara, tanpa error
    private static void PlaySFX(SFXReference sfx)
    {
        if (sfx == null) return;

        Audio.PlaySFX2D(sfx);
    }
}