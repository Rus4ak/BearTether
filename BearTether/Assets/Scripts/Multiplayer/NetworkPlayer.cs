using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Canvas _isReadyCanvas;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private GameObject _notReadyText;
    [SerializeField] private GameObject _readyCanvas;
    [SerializeField] private GameObject _meMark;
    [SerializeField] private GameObject _pauseMark;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _readyButton;
    [SerializeField] private GameObject _cancelReadyButton;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _movementButtons;
    [SerializeField] private GameObject _rope;
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private GameObject[] _mapBackgrounds;

    private NetworkPlayerMovement _networkPlayerMovement;
    private MultiplayerLobby _multiplayerManager;
    private int _playerID;

    private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> _isPause = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int _currentMapId = 0;
    
    [NonSerialized] public string sceneName;

    private static int countReady = 0;

    private void Start()
    {
        _networkPlayerMovement = GetComponent<NetworkPlayerMovement>();
        sceneName = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += ChangeScene;
        InitializeMultiplayerScene();
    }

    private void Update()
    {
        //print(LoadingScreen.Instance.countInitialized.Value);
        if (IsOwner)
        {
            if (LoadingScreen.Instance.countInitialized.Value == NetworkPlayersManager.Instance.players.Count)
            {
                LoadingScreen.Instance.RemoveInitializeServerRpc();
            }
        }

        if (sceneName == "Multiplayer")
        {
            if (IsServer
                &&_playButton.GetComponent<Button>().interactable == false 
                && NetworkPlayersManager.Instance.players.Count > 1 
                && countReady == NetworkPlayersManager.Instance.players.Count)
            {
                _playButton.GetComponent<Button>().interactable = true;
            }
            if (IsServer
                && _playButton.GetComponent<Button>().interactable == true
                && countReady != NetworkPlayersManager.Instance.players.Count)
            {
                _playButton.GetComponent<Button>().interactable = false;
            }
            if (IsServer
                && countReady != NetworkPlayersManager.Instance.players.Count
                && _multiplayerManager.choiceLevelMenu.activeInHierarchy)
            {
                _multiplayerManager.HideChoiceLevelMenu();
            }
            if (IsServer
                && NetworkPlayersManager.Instance.players.Count <= 1
                && _multiplayerManager.choiceLevelMenu.activeInHierarchy)
            {
                _multiplayerManager.HideChoiceLevelMenu();
            }

            if (_pauseMark.activeInHierarchy)
            {
                _pauseMark.SetActive(false);
                _playerSprite.color = Color.white;
            }

            if (IsOwner)
            {
                if (_isPause.Value)
                    _isPause.Value = false;

                transform.localPosition = new Vector3(transform.localPosition.x, -0.8f, transform.localPosition.z);
            }

            if (_currentMapId != _multiplayerManager.currentMapId.Value)
            {
                if (_multiplayerManager != null)
                {
                    _mapBackgrounds[_currentMapId].SetActive(false);
                    _currentMapId = _multiplayerManager.currentMapId.Value;
                    _mapBackgrounds[_currentMapId].SetActive(true);
                }
            }
        }
        else
        {
            if (!IsOwner)
            {
                if (_isPause.Value)
                {
                    if (!_pauseMark.activeInHierarchy)
                    {
                        _pauseMark.SetActive(true);
                        _playerSprite.color = new Color(.7f, .7f, .7f, 1);
                    }
                }
                else
                {
                    if (_pauseMark.activeInHierarchy)
                    {
                        _pauseMark.SetActive(false);
                        _playerSprite.color = Color.white;
                    }
                }
            }
        }
    }

    public override void OnDestroy()
    {
        for (int i = 0;  i < NetworkPlayersManager.Instance.players.Count; i++)
        {
            if (NetworkPlayersManager.Instance.players[i].player == gameObject)
            {
                NetworkPlayersManager.Instance.players.RemoveAt(i);
            }
        }

        if (IsOwner)
            if (!Camera.main.TryGetComponent<AudioListener>(out _))
                Camera.main.AddComponent<AudioListener>();

        SceneManager.activeSceneChanged -= ChangeScene;

        if (IsServer)
            countReady = 0;
    }

    public void Ready()
    {
        ToggleReadyServerRpc();
        _readyButton.SetActive(false);
        _cancelReadyButton.SetActive(true);
    }

    public void NotReady()
    {
        ToggleReadyServerRpc();
        _readyButton.SetActive(true);
        _cancelReadyButton.SetActive(false);
    }

    [ServerRpc]
    private void ToggleReadyServerRpc()
    {
        _isReady.Value = !_isReady.Value;

        if (_readyText.activeInHierarchy)
            countReady++;
        else
            countReady--;
    }

    private void UpdateReady(bool oldValue, bool newValue)
    {
        _notReadyText.SetActive(newValue);
        _readyText.SetActive(!newValue);
    }

    private void CreatePlayer()
    {
        Vector3 pos = new Vector3(-10, -2, 0);

        if (NetworkPlayersManager.Instance.players.Count > 0)
        {
            pos.x -= NetworkPlayersManager.Instance.players.Count * 3;
        }
        
        transform.position = pos;

        PlayerMultiplayer player = new PlayerMultiplayer(gameObject, GetComponent<Rigidbody2D>());
        player.player.transform.Find("Sprite").GetComponent<SpriteRenderer>().sortingOrder += NetworkPlayersManager.Instance.players.Count;

        bool isSimilar = false;
        
        foreach (PlayerMultiplayer plr in NetworkPlayersManager.Instance.players)
        {
            if (plr.player == gameObject)
                isSimilar = true;
        }

        if (!isSimilar)
        {
            _playerID = NetworkPlayersManager.Instance.players.Count + 1;
            NetworkPlayersManager.Instance.players.Add(player);
        }
        
        _multiplayerManager.InitializePlayer();
    }

    private void ChangeScene(Scene oldScene, Scene newScene)
    {
        sceneName = newScene.name;
        NetworkPlayerMovement networkPlayerMovement = GetComponent<NetworkPlayerMovement>();

        if (sceneName != "Multiplayer")
        {
            _isReadyCanvas.gameObject.SetActive(false);
            _readyCanvas.gameObject.SetActive(false);

            if (IsOwner)
            {
                _meMark.gameObject.SetActive(true);
                _camera.SetActive(true);
                _movementButtons.SetActive(true);

                if (networkPlayerMovement.spawnPosition == default)
                {
                    Vector3 pos = Vector3.zero;
                    pos.x -= _playerID * 2;
                    
                    networkPlayerMovement.spawnPosition = pos;
                }
                
                transform.position = networkPlayerMovement.spawnPosition;

                for (int i = 0; i < NetworkPlayersManager.Instance.players.Count - 1; i++)
                {
                    Rope rope = Instantiate(_rope).GetComponent<Rope>();
                    rope.InstantiateRope(NetworkPlayersManager.Instance.players[i].player.transform, NetworkPlayersManager.Instance.players[i + 1].player.transform);
                }
            }

            switch (_currentMapId)
            {
                case 0:
                    _networkPlayerMovement.ChangeParticleColors(new Color(0, .4f, .15f), new Color(.37f, .25f, .19f));
                    break;
                case 1:
                    _networkPlayerMovement.ChangeParticleColors(new Color(.4f, .4f, .4f), new Color(.35f, .35f, .35f));
                    break;
            }
        }
        else
        {
            InitializeMultiplayerScene();
            _multiplayerManager.ShowRoomMenu();
            _playerSprite.flipX = false;

            AudioListener audioListener;

            if (Camera.main.TryGetComponent<AudioListener>(out audioListener))
                Destroy(audioListener);

            if (networkPlayerMovement.spawnPosition != null)
                transform.position = networkPlayerMovement.spawnPosition;
        }

        if (IsServer && IsOwner)
            countReady = 1;

        if (IsOwner)
            LoadingScreen.Instance.AddInitializeServerRpc();
    }

    public void InitializeMultiplayerScene()
    {
        if (IsOwner)
        {
            _readyCanvas.SetActive(true);
            _meMark.gameObject.SetActive(false);
        }

        if (IsServer && !_readyButton.activeInHierarchy)
            _isReady.Value = true;

        if (!_isReadyCanvas.gameObject.activeInHierarchy)
            _isReadyCanvas.gameObject.SetActive(true);

        _camera.SetActive(false);
        _isReadyCanvas.worldCamera = Camera.main;
        _movementButtons.SetActive(false);

        _multiplayerManager = GameObject.FindWithTag("MultiplayerManager").GetComponent<MultiplayerLobby>();

        if (IsOwner)
            Invoke(nameof(CreatePlayer), .1f);
        else
            CreatePlayer();

        _playButton.GetComponent<Button>().interactable = false;

        _isReady.OnValueChanged += UpdateReady;
        UpdateReady(false, _isReady.Value);

        if (IsServer)
        {
            if (IsOwner)
            {
                ToggleReadyServerRpc();
                _playButton.SetActive(true);
                _playButton.GetComponent<Button>().onClick.AddListener(_multiplayerManager.ShowChoiceLevelMenu);
            }
        }
        else
        {
            _readyButton.SetActive(true);

            if (_cancelReadyButton.activeInHierarchy)
                _cancelReadyButton.SetActive(false);
        }
    }

    public void ActivatePauseMark()
    {
        if (IsOwner)
        {
            _isPause.Value = true;
        }
    }

    public void DisablePauseMark()
    {
        if (IsOwner)
        {
            _isPause.Value = false;
        }
    }
}
