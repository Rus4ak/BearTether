using TMPro;
using UnityEngine;

enum GameMode 
{
    Singleton,
    Multiplayer
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

    private Animation _currentStarAnimation;
    private string _nextLevel;
    private LoadScene _loadScene;
    private int _rewardedCoinsCount;
    private int _additionalCoinsCount;
    private TextMeshProUGUI _rewardedCoinsText;
    private string _quitScene;

    private int _countStars = 0;
    private bool _isNextLevel = false;
    private bool _isQuit = false;
    private bool _isFinishMapCoins = false;
    private bool _isRewardedFinishMapCoins = false;

    private void Start()
    {
        _loadScene = GetComponent<LoadScene>();
        _stars[0].SetActive(true);
        _starFlySound.Play();
        _currentStarAnimation = _stars[0].GetComponent<Animation>();
        _countStars++;
        _rewardedCoinsCount += 10;
        _rewardedCoinsText = _rewardedCoins.GetComponentInChildren<TextMeshProUGUI>();

        if (_gameMode == GameMode.Singleton)
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

        _additionalCoinsCount = _countStars - levelsManager.levels[_levelID].countStars;
        _rewardedCoinsCount += _additionalCoinsCount * 5;
        
        levelsManager.InitializeLevels();

        if (levelsManager.countCompletedLevels == 9)
            if (_levelID == 9)
                if (!levelsManager.isRewardedFinishMap)
                {
                    _rewardedCoinsCount += 100;
                    levelsManager.isRewardedFinishMap = true;
                    _isFinishMapCoins = true;
                }

        Bank.Instance.Coins += _rewardedCoinsCount;
        Progress.Instance.progressData.bank = Bank.Instance.Coins;

        if (_gameMode == GameMode.Singleton)
        {
            if (levelsManager.levels[_levelID].countStars < _countStars)
            {
                levelsManager.levels[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.levels[_levelID] = new Level(_levelID, _countStars, true);
            }
        } 
        else
        {
            if (levelsManager.multiplayerLevels[_levelID].countStars < _countStars)
            {
                levelsManager.multiplayerLevels[_levelID] = new Level(_levelID, _countStars, true);
                Progress.Instance.progressData.multiplayerLevels[_levelID] = new Level(_levelID, _countStars, true);
            }
        }

        Progress.Instance.Save();

        if (_additionalCoinsCount == 3)
            InstantiateAdditionalCoins("+5");
    }

    private void Update()
    {
        if (!_currentStarAnimation.isPlaying)
        {
            if (_isFinishMapCoins && _stars[2].activeInHierarchy && !_isRewardedFinishMapCoins)
            {
                InstantiateAdditionalCoins("+100");
                _isRewardedFinishMapCoins = true;
            }

            if (_countStars > 1 && !_stars[1].activeInHierarchy)
            {
                _stars[1].SetActive(true);
                _starFlySound.Play();
                _currentStarAnimation = _stars[1].GetComponent<Animation>();
                if (_additionalCoinsCount >= 2)
                    InstantiateAdditionalCoins("+5");
                return;
            }

            if (_countStars > 2 && !_stars[2].activeInHierarchy)
            {
                _stars[2].SetActive(true);
                _starFlySound.Play();
                _currentStarAnimation = _stars[2].GetComponent<Animation>();
                if (_additionalCoinsCount >= 1)
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
