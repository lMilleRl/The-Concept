using DG.Tweening;

public interface IVolumeAudioManager
{
    void InitializeSceneAudio(float fadeInDuration, Ease easeType);

    void FadeInGameplay(float durationInSec, Ease easeType);
    void FadeOutGameplay(float durationInSec, Ease easeType);
    void MuteGameplay();
    void ResumeGameplay();

    void FadeInCutscene(float durationInSec, Ease easeType);
    void FadeOutCutscene(float durationInSec, Ease easeType);
    void MuteCutscene();
    void ResumeCutscene();
}
