using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneAnimationHandler : CutscenePlayer
{
    [SerializeField] private VideoPlayer _clipsPlayer;

    public override IEnumerator PlayCutscene(CutsceneData cutscene)
    {
        if (cutscene.Clip != null)
        {
            _clipsPlayer.clip = cutscene.Clip;
            _clipsPlayer.Play();
            yield return new WaitUntil(() => _clipsPlayer.isPlaying);
            yield return new WaitUntil(() => _clipsPlayer.isPlaying == false);
        }
    }
}