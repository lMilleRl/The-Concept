using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneHandler : MonoBehaviour
{
    [SerializeField] private TransitionData transitionData;

    
    public void SwitchToScene()
    {
        TransitionHandler.Instance.StartTransition(transitionData);
    }
}
