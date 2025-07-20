using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _stars;

    private LevelsManager _levelsManager;
    private string _sceneName;

    private void Start()
    {
        _levelsManager = GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>();
        _sceneName = SceneManager.GetActiveScene().name;

        if (_sceneName == "Singleplayer")
            InitializeSingleplayerStars();
        else if (_sceneName == "Multiplayer")
            InitializeMultiplayerStars();
        if (_sceneName == "SingleplayerHardcore")
            InitializeSingleplayerHardcoreStars();
        else if (_sceneName == "MultiplayerHardcore")
            InitializeMultiplayerHardcoreStars();
    }

    private void InitializeSingleplayerStars()
    {
        for (int i = 0; i < _levelsManager.levels.Length; i++)
        {
            for (int j = 0; j < _levelsManager.levels[i].countStars; j++)
            {
                _stars[i].transform.GetChild(j).GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void InitializeMultiplayerStars()
    {
        for (int i = 0; i < _levelsManager.multiplayerLevels.Length; i++)
        {
            for (int j = 0; j < _levelsManager.multiplayerLevels[i].countStars; j++)
            {
                _stars[i].transform.GetChild(j).GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void InitializeSingleplayerHardcoreStars()
    {
        for (int i = 0; i < _levelsManager.levelsHardcore.Length; i++)
        {
            for (int j = 0; j < _levelsManager.levelsHardcore[i].countStars; j++)
            {
                _stars[i].transform.GetChild(j).GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void InitializeMultiplayerHardcoreStars()
    {
        for (int i = 0; i < _levelsManager.multiplayerLevelsHardcore.Length; i++)
        {
            for (int j = 0; j < _levelsManager.multiplayerLevelsHardcore[i].countStars; j++)
            {
                _stars[i].transform.GetChild(j).GetComponent<Image>().color = Color.white;
            }
        }
    }
}
