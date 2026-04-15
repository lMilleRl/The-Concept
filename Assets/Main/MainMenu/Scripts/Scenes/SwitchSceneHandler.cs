using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneHandler : MonoBehaviour
{
    [SerializeField] private int _sceneToSwith;

    public void SwitchToScene()
    {
        SceneManager.LoadScene(_sceneToSwith);
    }
}
