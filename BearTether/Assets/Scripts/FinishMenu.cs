using UnityEngine;

public class FinishMenu : MonoBehaviour
{
    [SerializeField] private int _levelID;
    [SerializeField] private GameObject[] _stars = new GameObject[3];
    [SerializeField] private Animation _nextLevelButtonAnimation;
    [SerializeField] private Animation _mainMenuButtonAnimation;

    private Animation _currentStarAnimation;
    private string _nextLevel;
    private LoadScene _loadScene;
    
    private int _countStars = 0;
    private bool _isNextLevel = false;
    private bool _isMainMenu = false;

    private void Start()
    {
        _loadScene = GetComponent<LoadScene>();
        _stars[0].SetActive(true);
        _currentStarAnimation = _stars[0].GetComponent<Animation>();
        _countStars++;

        if (PlayerMovement.attempt < 5 && _countStars == 1)
            _countStars++;

        if (PlayerMovement.attempt < 3 && _countStars == 2)
            _countStars++;

        GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>().levels[_levelID] = new Level(_levelID, _countStars);
    }

    private void Update()
    {
        if (!_currentStarAnimation.isPlaying)
        {
            if (_countStars > 1 && !_stars[1].activeInHierarchy)
            {
                _stars[1].SetActive(true);
                _currentStarAnimation = _stars[1].GetComponent<Animation>();
                return;
            }

            if (_countStars > 2 && !_stars[2].activeInHierarchy)
            {
                _stars[2].SetActive(true);
                _currentStarAnimation = _stars[2].GetComponent<Animation>();
            }
        }

        if (_isNextLevel && !_nextLevelButtonAnimation.isPlaying)
            _loadScene.Load(_nextLevel);

        if (_isMainMenu && !_mainMenuButtonAnimation.isPlaying)
            _loadScene.Load("MainMenu");

    }

    public void NextLevelButton(string nextLevel)
    {
        _nextLevelButtonAnimation.Play();
        _isNextLevel = true;
        _nextLevel = nextLevel;
    }

    public void MainMenuButton()
    {
        _mainMenuButtonAnimation.Play();
        _isMainMenu = true;
    }
}
