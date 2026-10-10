using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slafurry.Systems.Audio;
using Slafurry.Systems.Scene;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Labels")]
    [SerializeField] private TMP_Text masterLabel;
    [SerializeField] private TMP_Text musicLabel;
    [SerializeField] private TMP_Text sfxLabel;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button resetButton;

    [Header("Scenes")]
    [SerializeField] private SceneReference mainMenuScene;

    private const string MasterPref = "MasterVolume_dB";
    private const string MusicPref  = "MusicVolume_dB";
    private const string SFXPref    = "SFXVolume_dB";

    private void Awake()
    {
        ConfigureSlider(masterSlider);
        ConfigureSlider(musicSlider);
        ConfigureSlider(sfxSlider);
    }

    private void OnEnable()
    {
        SetSlider(masterSlider, masterLabel, PlayerPrefs.GetFloat(MasterPref, AudioSystem.DefaultDb));
        SetSlider(musicSlider,  musicLabel,  PlayerPrefs.GetFloat(MusicPref,  AudioSystem.DefaultDb));
        SetSlider(sfxSlider,    sfxLabel,    PlayerPrefs.GetFloat(SFXPref,    AudioSystem.DefaultDb));

        if (masterSlider) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider)  musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider)    sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        if (backButton)  backButton.onClick.AddListener(BackToMain);
        if (resetButton) resetButton.onClick.AddListener(ResetVolumes);
    }

    private void OnDisable()
    {
        if (masterSlider) masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        if (musicSlider)  musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider)    sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);

        if (backButton)  backButton.onClick.RemoveListener(BackToMain);
        if (resetButton) resetButton.onClick.RemoveListener(ResetVolumes);
    }

    private void OnMasterChanged(float db)
    {
        AudioSystem.Instance.UpdateMasterVolume(db);
        UpdateLabel(masterLabel, db);
    }

    private void OnMusicChanged(float db)
    {
        AudioSystem.Instance.UpdateMusicVolume(db);
        UpdateLabel(musicLabel, db);
    }

    private void OnSFXChanged(float db)
    {
        AudioSystem.Instance.UpdateSFXVolume(db);
        UpdateLabel(sfxLabel, db);
    }

    public void ResetVolumes()
    {
        SetSlider(masterSlider, masterLabel, AudioSystem.DefaultDb);
        SetSlider(musicSlider,  musicLabel,  AudioSystem.DefaultDb);
        SetSlider(sfxSlider,    sfxLabel,    AudioSystem.DefaultDb);

        AudioSystem.Instance.UpdateMasterVolume(AudioSystem.DefaultDb);
        AudioSystem.Instance.UpdateMusicVolume(AudioSystem.DefaultDb);
        AudioSystem.Instance.UpdateSFXVolume(AudioSystem.DefaultDb);
    }

    public void BackToMain()
    {
        PlayerPrefs.Save();
        SceneSystem.Load(mainMenuScene);
    }

    private static void ConfigureSlider(Slider slider)
    {
        if (!slider) return;
        slider.minValue = AudioSystem.MinDb;
        slider.maxValue = AudioSystem.MaxDb;
        slider.wholeNumbers = true;
    }

    private void SetSlider(Slider slider, TMP_Text label, float db)
    {
        db = Mathf.Clamp(db, AudioSystem.MinDb, AudioSystem.MaxDb);
        if (slider) slider.SetValueWithoutNotify(db);
        UpdateLabel(label, db);
    }

    private static void UpdateLabel(TMP_Text label, float db)
    {
        if (!label) return;
        label.text = db <= AudioSystem.MinDb ? "Mute" : $"{db:+0;-0;0} dB";
    }
}