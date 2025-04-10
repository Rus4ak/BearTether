using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMultiplayer
{
    public GameObject player;
    public Rigidbody2D playerRb;

    public PlayerMultiplayer(GameObject player, Rigidbody2D playerRb)
    {
        this.player = player;
        this.playerRb = playerRb;
    }
}

public class MultiplayerLobby : NetworkBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private Transform _room;
    [SerializeField] private Button _leaveButton;
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _errorText;

    private float _screenWidth;

    private NetworkVariable<FixedString64Bytes> _sessionName = new NetworkVariable<FixedString64Bytes>("Room");
    
    [NonSerialized] public List<PlayerMultiplayer> players = new List<PlayerMultiplayer>();

    private void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        _screenWidth = screenHeight * Screen.width / Screen.height;
    }

    private void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].player.transform.position.x < -1 * i * 3)
            {
                players[i].playerRb.linearVelocity = new Vector3(5, 0, 0);
            }
        }

        _ground.transform.position -= new Vector3(Time.deltaTime * 2, 0, 0);

        if (_ground.transform.position.x < -_screenWidth)
            _ground.transform.position = new Vector3(_screenWidth, 0, 0);

        if (_roomNameText.text != _sessionName.Value.ToString())
        {
            _roomNameText.text = _sessionName.Value.ToString();
        }
    }

    public void InitializePlayer()
    {
        for (int i = 0; i < players.Count; i++)
            players[i].player.GetComponent<Animator>().SetBool("Run", true);

        if (IsServer)
            _leaveButton.onClick.AddListener(LeaveClientRpc);
    }

    public void SetSessionName(TextMeshProUGUI name)
    {
        _sessionName.Value = name.text;
    }

    public void ShowRoomMenu()
    {
        Vector3 position = _room.localPosition;
        position.x = 0;
        _room.localPosition = position;
    }

    public void HideRoomMenu()
    {
        Vector3 position = _room.localPosition;
        position.x = 2000;
        _room.localPosition = position;
    }

    [ClientRpc]
    private void LeaveClientRpc()
    {
        if (!IsServer)
            _leaveButton.onClick.Invoke();
    }

    public void InvokeErrorText(string text)
    {
        TextMeshProUGUI errorText = Instantiate(_errorText, _canvas).GetComponent<TextMeshProUGUI>();
        errorText.text = text;
    }
}
