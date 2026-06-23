using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneHandler : MonoBehaviour
{
    [SerializeField] private TransitionHandler transitionHandler;
    [SerializeField] private TransitionData transitionData;

    
    public void SwitchToScene()
    {
        transitionHandler.StartTransition(transitionData);
    }
}
