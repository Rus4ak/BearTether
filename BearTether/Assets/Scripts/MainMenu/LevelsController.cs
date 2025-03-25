using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _stars;

    private LevelsManager _levelsManager;

    private void Start()
    {
        _levelsManager = GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>();
       
        for (int i =  0; i < _levelsManager.levels.Length; i++)
        {
            for (int j = 0; j < _levelsManager.levels[i].countStars; j++)
            {
                _stars[i].transform.GetChild(j).GetComponent<Image>().color = Color.white;
            }
        }
    }
}
