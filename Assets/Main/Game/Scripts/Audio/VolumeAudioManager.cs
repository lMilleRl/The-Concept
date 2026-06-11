using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeAudioManager : MonoBehaviour, IVolumeAudioManager
{
    const float MinDB = -80f;
    
    public static IVolumeAudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private string _gameplayVolumeParam = "GameplayVolume";
    [SerializeField] private string _cutsceneVolumeParam = "CutsceneVolume";
    
    [SerializeField] private bool _isFadeInGameplaySoundOnStart;
    [SerializeField] private Ease _easeFadeInType;
    [SerializeField] [Range(0f, float.MaxValue)] private float _fadeInGameplaySoundDuration = 2f;

    private void Awake()
    {
        if (Instance != null && (Instance as MonoBehaviour)?.gameObject != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (_isFadeInGameplaySoundOnStart)
            InitializeSceneAudio(_fadeInGameplaySoundDuration, _easeFadeInType);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Инициализация аудио при старте сцены: начинаем с тишины и плавно включаем
    public void InitializeSceneAudio(float fadeInDuration, Ease easeType)
    {
        MuteGameplay();
        FadeInGameplay(fadeInDuration, easeType);
    }

    // Конвертация линейного значения (0-1) в децибелы
    // 0 → -80dB (тишина), 1 → 0dB (исходная громкость)
    private float LinearToDecibels(float normalizedVolume)
    {
        const float MinNormalizeValue = 0.0001f;
        if (normalizedVolume <= MinNormalizeValue)
            return MinDB;
        return 20f * Mathf.Log10(normalizedVolume);
    }

    // Конвертация децибелов в линейное значение (0-1)
    private float DecibelsToLinear(float db)
    {
        if (db <= MinDB)
            return 0f;
        return Mathf.Pow(10f, db / 20f);
    }

    // Получить громкость как линейное значение (0-1)
    public float GetVolume(string paramName)
    {
        _mixer.GetFloat(paramName, out var db);
        return DecibelsToLinear(db);
    }

    // Установить громкость линейным значением (0-1)
    public void SetVolume(string paramName, float normalizedVolume)
    {
        var db = LinearToDecibels(normalizedVolume);
        _mixer.SetFloat(paramName, db);
    }

    // Fade с линейной интерполяцией для корректного восприятия
    // Анимируем значение 0-1, конвертируя в децибелы на каждом шаге
    private void FadeVolume(string paramName, float targetLinear, float duration, Ease ease)
    {
        var currentLinear = GetVolume(paramName);
        DOVirtual.Float(currentLinear, targetLinear, duration, 
            value => SetVolume(paramName, value))
            .SetEase(ease);
    }

    public void FadeInGameplay(float durationInSec, Ease easeType)
    {
        FadeVolume(_gameplayVolumeParam, 1f, durationInSec, easeType);
    }

    public void FadeOutGameplay(float durationInSec, Ease easeType)
    {
        FadeVolume(_gameplayVolumeParam, 0f, durationInSec, easeType);
    }

    public void MuteGameplay()
    {
        SetVolume(_gameplayVolumeParam, 0f);
    }

    public void ResumeGameplay()
    {
        SetVolume(_gameplayVolumeParam, 1f);
    }

    public void FadeInCutscene(float durationInSec, Ease easeType)
    {
        FadeVolume(_cutsceneVolumeParam, 1f, durationInSec, easeType);
    }

    public void FadeOutCutscene(float durationInSec, Ease easeType)
    {
        FadeVolume(_cutsceneVolumeParam, 0f, durationInSec, easeType);
    }

    public void MuteCutscene()
    {
        SetVolume(_cutsceneVolumeParam, 0f);
    }

    public void ResumeCutscene()
    {
        SetVolume(_cutsceneVolumeParam, 1f);
    }
}