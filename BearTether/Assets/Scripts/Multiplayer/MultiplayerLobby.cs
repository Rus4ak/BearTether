using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobby : NetworkBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private TextMeshProUGUI _roomNameText;

    private NetworkVariable<FixedString64Bytes> _sessionName = new NetworkVariable<FixedString64Bytes>(
        "Room", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private Rigidbody2D[] _playersRb = new Rigidbody2D[4];
    private Animator[] _playerAnimators = new Animator[4];
    private float _screenWidth;

    [NonSerialized] public int playersCount = 0;
    [NonSerialized] public GameObject[] players = new GameObject[4];

    private void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        _screenWidth = screenHeight * Screen.width / Screen.height;
    }

    private void Update()
    {
        for (int i = 0; i < playersCount; i++)
        {
            if (players[i].transform.position.x < -1 * i * 3)
            {
                _playersRb[i].linearVelocity = new Vector3(5, 0, 0);
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
        for (int i = 0; i < playersCount; i++)
        {
            _playersRb[i] = players[i].GetComponent<Rigidbody2D>();
            _playerAnimators[i] = players[i].GetComponent<Animator>();
            _playerAnimators[i].SetBool("Run", true);
        }
    }

    public void SetSessionName(TextMeshProUGUI name)
    {
        _sessionName.Value = name.text;
    }
}
