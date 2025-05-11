using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkCheck : MonoBehaviour
{
    [SerializeField] private GameObject _failedConnectionMenu;

    private void Awake()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _failedConnectionMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
