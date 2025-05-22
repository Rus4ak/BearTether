using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Canvas _isReadyCanvas;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private GameObject _notReadyText;
    [SerializeField] private GameObject _readyCanvas;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _readyButton;
    [SerializeField] private GameObject _cancelReadyButton;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _movementButtons;
    [SerializeField] private GameObject _rope;

    private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private MultiplayerLobby _multiplayerManager;
    private string _sceneName;

    private static int countReady = 0;

    private void Start()
    {
        InitializeMultiplayerScene();
    }

    private void Update()
    {
        if (_sceneName == "Multiplayer")
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
        transform.position = transform.position + Vector3.left * NetworkPlayersManager.Instance.players.Count * 3;

        PlayerMultiplayer player = new PlayerMultiplayer(gameObject, GetComponent<Rigidbody2D>());
        player.player.GetComponent<SpriteRenderer>().sortingOrder += NetworkPlayersManager.Instance.players.Count;

        bool isSimilar = false;
        
        foreach (PlayerMultiplayer plr in NetworkPlayersManager.Instance.players)
        {
            if (plr.player == gameObject)
                isSimilar = true;
        }

        if (!isSimilar) 
            NetworkPlayersManager.Instance.players.Add(player);
        
        _multiplayerManager.InitializePlayer();
    }

    private void ChangeScene(Scene oldScene, Scene newScene)
    {
        _sceneName = newScene.name;
        
        if (_sceneName != "Multiplayer")
        {
            _isReadyCanvas.gameObject.SetActive(false);
            _readyCanvas.gameObject.SetActive(false);

            if (IsOwner)
            {
                _camera.SetActive(true);
                _movementButtons.SetActive(true);

                GetComponent<NetworkPlayerMovement>().spawnPosition = transform.position;

                for (int i = 0; i < NetworkPlayersManager.Instance.players.Count - 1; i++)
                {
                    Rope rope = Instantiate(_rope).GetComponent<Rope>();
                    rope.InstantiateRope(NetworkPlayersManager.Instance.players[i].player.transform, NetworkPlayersManager.Instance.players[i + 1].player.transform);
                }
            }
        }
        else
        {
            InitializeMultiplayerScene();
            _multiplayerManager.ShowRoomMenu();
            transform.rotation = Quaternion.identity;

            AudioListener audioListener;

            if (Camera.main.TryGetComponent<AudioListener>(out audioListener))
                Destroy(audioListener);

            if (IsServer && IsOwner)
                countReady = 1;
        }
    }

    public void InitializeMultiplayerScene()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += ChangeScene;

        if (_sceneName == "Multiplayer")
        {
            if (IsOwner)
                _readyCanvas.SetActive(true);

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
    }
}
