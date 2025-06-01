using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionSpeedChecker : MonoBehaviour
{
    [SerializeField] private GameObject _lowWiFiIcon;
    [SerializeField] private GameObject _failedConnectionMenu;

    private string _pingAddress = "8.8.8.8";
    private float _timeout = 200f;

    private void Awake()
    {
        ConnectionSpeedChecker[] objs = FindObjectsByType<ConnectionSpeedChecker>(FindObjectsSortMode.InstanceID);

        if (objs.Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(CheckPing());
    }

    IEnumerator CheckPing()
    {
        while (true)
        {
            Ping ping = new Ping(_pingAddress);

            float startTime = Time.time;

            while (!ping.isDone && Time.time - startTime < 1f)
            {
                yield return null;
            }

            if (ping.isDone)
            {
                if (ping.time > _timeout)
                    _lowWiFiIcon.SetActive(true);
                else if (ping.time < 0)
                    _failedConnectionMenu.SetActive(true);
                else
                    _lowWiFiIcon.SetActive(false);
            }
            else
                _failedConnectionMenu.SetActive(true);

            yield return new WaitForSeconds(5);
        }
    }

    public void Restart()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
