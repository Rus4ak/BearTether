using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private bool _isLoadingScreen = true;

    public void Load(string scene)
    {
        if (_isLoadingScreen)
            LoadingScreen.Instance.SetOn();

        SceneManager.LoadScene(scene);
    }
}
