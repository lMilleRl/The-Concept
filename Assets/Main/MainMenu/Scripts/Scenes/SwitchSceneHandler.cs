using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneHandler : MonoBehaviour
{
    [SerializeField] private SceneTransitionHandler _sceneTransitionHandler;
    [SerializeField] private SceneTransitionData _sceneTransitionData;

    
    public void SwitchToScene()
    {
        _sceneTransitionHandler.StartAnimatedTransitionToScene(_sceneTransitionData);
    }
}
