using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string scene)
    {
        LoadingScreen.Instance.SetOn();
        SceneManager.LoadScene(scene);
    }
}
