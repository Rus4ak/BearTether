using TMPro;
using UnityEngine;

enum GameMode 
{
    Singleplayer,
    Multiplayer,
    SingleplayerHardcore,
    MultiplayerHardcore
}

public class FinishMenu : MonoBehaviour
{
    [SerializeField] private int _levelID;
    [SerializeField] private GameObject[] _stars = new GameObject[3];
    [SerializeField] private Animation _nextLevelButtonAnimation;
    [SerializeField] private Animation _quitButtonAnimation;
    [SerializeField] private GameObject _additionalCoinsPrefab;
    [SerializeField] private GameObject _rewardedCoins;
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private AudioSource _starFlySound;
    [SerializeField] private GameObject _IARManager;

    private Animation _currentStarAnimation;
    private string _nextLevel;
    private LoadScene _loadScene;
    private int _rewardedCoinsCount;
    private int _additionalCoinsCount;
    private TextMeshProUGUI _rewardedCoinsText;
    private string _quitScene;

    private bool[] _isAdditionalCoins = new bool[3] { false, false, false };
    private int _countStars = 0;
    private bool _isNextLevel = false;
    private bool _isQuit = false;

    private void Start()
    {
        _loadScene = GetComponent<LoadScene>();
        _stars[0].SetActive(true);
        _starFlySound.Play();
        _currentStarAnimation = _stars[0].GetComponent<Animation>();
        _countStars++;
        _rewardedCoinsText = _rewardedCoins.GetComponentInChildren<TextMeshProUGUI>();

        if (_levelID == 4 && !IARManager.isShownReview)
        {
            Instantiate(_IARManager);
        }

        if (_gameMode == GameMode.Singleplayer || _gameMode == GameMode.SingleplayerHardcore)
        {
            if (PlayerMovement.attempt < 5 && _countStars == 1)
                _countStars++;

            if (PlayerMovement.attempt < 3 && _countStars == 2)
                _countStars++;
        } 
        else
        {
            if (NetworkFinish.Instance.attempts.Value < 5 && _countStars == 1)
                _countStars++;

            if (NetworkFinish.Instance.attempts.Value < 3 && _countStars == 2)
                _countStars++;
        }

        LevelsManager levelsManager = GameObject.FindWithTag("LevelsManager").GetComponent<LevelsManager>();

        if (_gameMode == GameMode.Singleplayer)
        {
            _additionalCoinsCount = _countStars - levelsManager.levels[_levelID].countStars;
            for (int i = 1; i <= _countStars; i++)
            {
                if (i > levelsManager.levels[_levelID].countStars)
                {
                    _isAdditionalCoins[i - 1] = true;
                }
            }
        }
        
        else if (_gameMode == GameMode.SingleplayerHardcore)
        {
            _additionalCoinsCount = _countStars - levelsManager.levelsHardcore[_levelID].countStars;
            for (int i = 1; i <= _countStars; i++)
            {
                if (i > levelsManager.levelsHardcore[_levelID].countStars)
                {
                    _isAdditionalCoins[i - 1] = true;
                }
            }
        }
        
        else if (_gameMode == GameMode.Multiplayer)
        {
            _additionalCoinsCount = _countStars - levelsManager.multiplayerLevels[_levelID].countStars;
            for (int i = 1; i <= _countStars; i++)
            {
                if (i > levelsManager.multiplayerLevels[_levelID].countStars)
                {
                    _isAdditionalCoins[i - 1] = true;
                }
            }
        }

        else if (_gameMode == GameMode.MultiplayerHardcore)
        {
            _additionalCoinsCount = _countStars - levelsManager.multiplayerLevelsHardcore[_levelID].countStars;
            for (int i = 1; i <= _countStars; i++)
            {
                if (i > levelsManager.multiplayerLevelsHardcore[_levelID].countStars)
                {
                    _isAdditionalCoins[i - 1] = true;
                }
            }
        }

        _rewardedCoinsCount += _additionalCoinsCount * 5;
        
        levelsManager.InitializeLevels();

        Bank.Instance.Coins += _rewardedCoinsCount;
        Progress.Instance.progressData.bank = Bank.Instance.Coins;

        if (_gameMode == GameMode.Singleplayer)
        {
            if (levelsManager.levels[_levelID].countStars < _countStars)
            {
                levelsManager.levels[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.levels[_levelID] = new Level(_levelID, _countStars, true);
            }
        } 
        else if (_gameMode == GameMode.SingleplayerHardcore)
        {
            if (levelsManager.levelsHardcore[_levelID].countStars < _countStars)
            {
                levelsManager.levelsHardcore[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.levelsHardcore[_levelID] = new Level(_levelID, _countStars, true);
            }
        }
        else if (_gameMode == GameMode.Multiplayer)
        {
            if (levelsManager.multiplayerLevels[_levelID].countStars < _countStars)
            {
                levelsManager.multiplayerLevels[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.multiplayerLevels[_levelID] = new Level(_levelID, _countStars, true);
            }
        }
        else if (_gameMode == GameMode.MultiplayerHardcore)
        {
            if (levelsManager.multiplayerLevelsHardcore[_levelID].countStars < _countStars)
            {
                levelsManager.multiplayerLevelsHardcore[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.multiplayerLevelsHardcore[_levelID] = new Level(_levelID, _countStars, true);
            }
        }

        Progress.Instance.Save();

        if (_isAdditionalCoins[0])
            InstantiateAdditionalCoins("+5");

        if (_gameMode == GameMode.Singleplayer || _gameMode == GameMode.SingleplayerHardcore)
            AnalyticsManager.Instance.CompleteLevel(_gameMode.ToString(), _levelID, _countStars, PlayerMovement.attempt);
        else
            AnalyticsManager.Instance.CompleteLevel(_gameMode.ToString(), _levelID, _countStars, NetworkFinish.Instance.attempts.Value);
    }

    private void Update()
    {
        if (!_currentStarAnimation.isPlaying)
        {
            if (_countStars > 1 && !_stars[1].activeInHierarchy)
            {
                _stars[1].SetActive(true);
                _starFlySound.Play();
                _currentStarAnimation = _stars[1].GetComponent<Animation>();
                if (_isAdditionalCoins[1])
                    InstantiateAdditionalCoins("+5");
                return;
            }

            if (_countStars > 2 && !_stars[2].activeInHierarchy)
            {
                _stars[2].SetActive(true);
                _starFlySound.Play();
                _currentStarAnimation = _stars[2].GetComponent<Animation>();
                if (_isAdditionalCoins[2])
                    InstantiateAdditionalCoins("+5");
            }
        }

        if (_isNextLevel && !_nextLevelButtonAnimation.isPlaying)
            _loadScene.Load(_nextLevel);

        if (_isQuit && !_quitButtonAnimation.isPlaying)
            _loadScene.Load(_quitScene);

    }

    private void InstantiateAdditionalCoins(string text)
    {
        GameObject additionalCoins = Instantiate(_additionalCoinsPrefab);

        AdditionalCoins additionalCoinsScript = additionalCoins.GetComponentInChildren<AdditionalCoins>();
        additionalCoinsScript.ChangeText(text);
        _rewardedCoinsText.text = "+" + (int.Parse(_rewardedCoinsText.text) + int.Parse(text)).ToString();
    }

    public void NextLevelButton(string nextLevel)
    {
        _nextLevelButtonAnimation.Play();
        _isNextLevel = true;
        _nextLevel = nextLevel;
    }

    public void QuitButton(string quitScene)
    {
        _quitButtonAnimation.Play();
        _isQuit = true;
        _quitScene = quitScene;
    }
}
