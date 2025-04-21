using Unity.Netcode;
using UnityEngine;

public class NetworkFinish : NetworkBehaviour
{
    [SerializeField] private GameObject _finishMenu;

    private int _countFinishedPlayers;
    private int _playersCount;
    
    public NetworkVariable<int> attempts = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static NetworkFinish Instance;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _playersCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void Update()
    {
        if (_countFinishedPlayers == _playersCount)
            _finishMenu.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _countFinishedPlayers++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _countFinishedPlayers--;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddAttemptServerRpc()
    {
        attempts.Value++;
    }
}
