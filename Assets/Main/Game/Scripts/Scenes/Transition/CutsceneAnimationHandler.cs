using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneAnimationHandler : MonoBehaviour
{
    [SerializeField] private VideoPlayer _clipsPlayer;

    public IEnumerator PlayCutscene(CutsceneData cutscene)
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