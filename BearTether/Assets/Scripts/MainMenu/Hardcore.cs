using UnityEngine;
using UnityEngine.SceneManagement;

public class Hardcore : MonoBehaviour
{
    [SerializeField] private GameObject _buyMenu;

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

    public void BuyHardcore()
    {
        isBoughtHardcore = true;
        Progress.Instance.progressData.isBoughtHardcore = true;
        Progress.Instance.Save();
        _buyMenu?.SetActive(false);
    }
}
