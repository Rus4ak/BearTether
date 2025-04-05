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

    private NetworkVariable<bool> _isReady = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private MultiplayerLobby _multiplayerManager;
    private string _sceneName;
    private int _playerID;

    private static int countReady = 0;

    private void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;

        if (_sceneName == "Multiplayer")
        {
            if (IsOwner)
                _readyCanvas.SetActive(true);
            
            _isReadyCanvas.worldCamera = Camera.main;
            GameObject.Find("MovementButtons").SetActive(false);

            int playersCount;

            _multiplayerManager = GameObject.FindWithTag("LevelsManager").GetComponent<MultiplayerLobby>();
            _multiplayerManager.playersCount += 1;
            playersCount = _multiplayerManager.playersCount;

            transform.position = transform.position + Vector3.left * playersCount * 3;
            _playerID = playersCount - 1;

            _multiplayerManager.players[_playerID] = this.gameObject;
            _multiplayerManager.InitializePlayer();

            _playButton.GetComponent<Button>().interactable = false;

            _isReady.OnValueChanged += UpdateReady;
            UpdateReady(false, _isReady.Value);

            if (IsServer)
            {
                ToggleReadyServerRpc();
                _playButton.SetActive(true);
            }
            else
            {
                _readyButton.SetActive(true);
            }
        }
        else
        {
            _isReadyCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsServer
            &&_playButton.GetComponent<Button>().interactable == false 
            && _multiplayerManager.playersCount > 1 
            && countReady == _multiplayerManager.playersCount)
        {
            _playButton.GetComponent<Button>().interactable = true;
        }
        if (IsServer
            && _playButton.GetComponent<Button>().interactable == true
            && countReady != _multiplayerManager.playersCount)
        {
            _playButton.GetComponent<Button>().interactable = false;
        }
    }

    public override void OnDestroy()
    {
        _multiplayerManager.playersCount--;
        _multiplayerManager.players[_playerID] = null;
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
}
