using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public void SetPause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        LoadingScreen.Instance.SetOn();
        SceneManager.LoadScene("MainMenu");
    }
}
