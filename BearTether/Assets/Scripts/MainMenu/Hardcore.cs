using UnityEngine;
using UnityEngine.SceneManagement;

public class Hardcore : MonoBehaviour
{
    [SerializeField] private GameObject _buyMenu;

    [Header("Multiplayer")]
    [SerializeField] private GameObject _normalLevelsMenu;
    [SerializeField] private GameObject _hardcoreLevelsMenu;

    public static bool isBoughtHardcore;

    public void SwitchHardcore(string SceneName)
    {
        if (isBoughtHardcore)
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            _buyMenu.SetActive(true);
        }
    }

    public void SwitchHardcoreMultiplayer()
    {
        if (isBoughtHardcore)
        {
            if (_normalLevelsMenu.activeInHierarchy)
            {
                _hardcoreLevelsMenu.SetActive(true);
                _normalLevelsMenu.SetActive(false);
            }
            else
            {
                _normalLevelsMenu.SetActive(true);
                _hardcoreLevelsMenu.SetActive(false);
            }
        }
        else
        {
            _buyMenu.SetActive(true);
        }

    }

    public void BuyHardcore()
    {
        isBoughtHardcore = true;
        Progress.Instance.progressData.isBoughtHardcore = true;
        Progress.Instance.Save();
        _buyMenu.SetActive(false);
    }
}
