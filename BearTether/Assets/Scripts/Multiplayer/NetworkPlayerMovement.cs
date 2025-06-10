using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private GameObject _playerSprite;

    [Header("Movement")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _raycastLength = 1.3f;
    [SerializeField] private LayerMask _jumpLayer;

    [Header("Rope")]
    [SerializeField] private float _maxRopeDistance = 2.5f;
    [SerializeField] private float _ropePullStrength = 35f;
    [SerializeField] private Transform _anchor;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _runParticleSystem;
    [SerializeField] private ParticleSystem _jumpParticleSystem;
    [SerializeField] private Color[] _particleColors = new Color[2];

    [Header("Sounds")]
    [SerializeField] private AudioSource _stepsSound;
    [SerializeField] private AudioSource _jumpSound;

    private NetworkVariable<float> _move = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;
    private float _currentSpeed;
    private HingeJoint2D _hingeJoint;
    private NetworkPlayer _networkPlayer;

    private bool _isMove = true;
    private bool _lookRight = true;

    [NonSerialized] public Vector3 spawnPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = _playerSprite.GetComponent<Animator>();
        _hingeJoint = GetComponent<HingeJoint2D>();
        _networkPlayer = GetComponent<NetworkPlayer>();
        _currentSpeed = _speed;

        AudioListener audioListener;

        if (Camera.main.TryGetComponent<AudioListener>(out audioListener))
            Destroy(audioListener);

        if (IsOwner)
            gameObject.AddComponent<AudioListener>();

        ParticleSystem.MainModule[] main = new ParticleSystem.MainModule[2] { _runParticleSystem.main, _jumpParticleSystem.main };

        for (int i = 0; i < main.Length; i++)
            main[i].startColor = new ParticleSystem.MinMaxGradient(_particleColors[0], _particleColors[1]);
    }

    private void Update()
    {
        if (_networkPlayer.sceneName == "Multiplayer")
            return;

        Moving();

        _anchor.localPosition = Vector3.zero;

        if (_hingeJoint.enabled)
            if (_isOnGround)
            {
                _hingeJoint.enabled = false;
                _isMove = true;
            }
        
        if (_move.Value > 0 && _lookRight == false)
            Flip();
        
        else if (_move.Value < 0 && _lookRight == true)
            Flip();

        if (_isOnGround && _animator.GetBool("Jump"))
        {
            _animator.SetBool("Jump", false);
            _jumpParticleSystem.Play();
            _jumpSound.Play();
        }

        if (!_isOnGround)
            _animator.SetBool("Jump", true);
        
        if (transform.position.y < -5f)
        {
            if (IsOwner)
                DeadServerRpc();
        }
    }

    private void FixedUpdate()
    {
        _isOnGround = Physics2D.Raycast(transform.position, Vector2.down, _raycastLength, _jumpLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            if (IsOwner)
                DeadServerRpc();
        }
    }

    [ServerRpc]
    private void DeadServerRpc()
    {
        NetworkFinish.Instance.AddAttemptServerRpc();

        float xPos = 0;

        foreach (var player in NetworkPlayersManager.Instance.players)
        {
            player.player.GetComponent<NetworkPlayerMovement>().DeadClientRpc(xPos);
            xPos -= 2f;
        }
        
    }

    [ClientRpc]
    private void DeadClientRpc(float xPos)
    {
        if (IsOwner)
        {
            Vector3 position = spawnPosition;
            position.x = xPos;

            transform.position = position;
        }
    }

    private void Moving()
    {
        if (IsOwner)
        {
            List<PlayerMultiplayer> players = NetworkPlayersManager.Instance.players;

            for (int i = 0; i < players.Count - 1; i++)
            {
                var playerA = players[i];
                var playerB = players[i + 1];

                Vector3 posA = playerA.player.transform.position;
                Vector3 posB = playerB.player.transform.position;

                float distance = Vector3.Distance(posA, posB);

                if (distance > _maxRopeDistance)
                {
                    float overshoot = distance - _maxRopeDistance;
                    Vector3 direction = (posA - posB).normalized;

                    playerA.playerRb.AddForce(-direction * overshoot * _ropePullStrength);
                    playerB.playerRb.AddForce(direction * overshoot * _ropePullStrength);
                }
            }

            int myIndex = players.FindIndex(p => p.player == gameObject);

            if (myIndex != -1)
            {
                Vector2 myVelocity = _rigidbody.linearVelocity;
                _currentSpeed = _speed;

                if (myIndex > 0)
                {
                    Transform otherPlayer = players[myIndex - 1].player.transform;

                    float speed = CalculateDistance(transform, otherPlayer);
                    RopeHanging(transform, otherPlayer);

                    _currentSpeed = speed;
                }

                if (myIndex < players.Count - 1)
                {
                    Transform otherPlayer = players[myIndex + 1].player.transform;

                    float speed = CalculateDistance(transform, otherPlayer);
                    RopeHanging(transform, otherPlayer);

                    _currentSpeed = speed;
                }

                if (myIndex > 0 && myIndex < players.Count - 1)
                {
                    Transform otherPlayerLeft = players[myIndex - 1].player.transform;
                    Transform otherPlayerRight = players[myIndex + 1].player.transform;

                    float speedLeft = CalculateDistance(transform, otherPlayerLeft);
                    float speedRight = CalculateDistance(transform, otherPlayerRight);

                    _currentSpeed = Mathf.Min(speedLeft, speedRight);
                }
            }

            if (_isMove)
            {
                _rigidbody.linearVelocity = new Vector2(_move.Value * _currentSpeed, _rigidbody.linearVelocity.y);

                if (_rigidbody.linearVelocity.y == 0 && !_isOnGround)
                    _rigidbody.linearVelocity += new Vector2(0, -10);
            }
        }

        if (_move.Value != 0 && _isOnGround)
        {
            _animator.SetBool("Run", true);

            if (!_stepsSound.isPlaying)
                _stepsSound.Play();

            if (!_runParticleSystem.isPlaying)
                _runParticleSystem.Play();
        }
        else
        {
            _animator.SetBool("Run", false);

            if (_stepsSound.isPlaying)
                _stepsSound.Stop();

            if (_runParticleSystem.isPlaying)
                _runParticleSystem.Stop();
        }
    }

    private float CalculateDistance(Transform playerOwner, Transform otherPlayer)
    {
        float distanceX = playerOwner.position.x - otherPlayer.position.x;
        float distance = Vector3.Distance(playerOwner.position, otherPlayer.position);

        if (distanceX > 2.5f)
        {
            if (_lookRight)
                return _speed / (Mathf.Clamp(Mathf.Abs(distance), 1f, 10f));
            else
                return _speed;
        }
        else if (distanceX < -2.5f)
        {
            if (_lookRight)
                return _speed;
            else
                return _speed / (Mathf.Clamp(Mathf.Abs(distance), 1f, 10f));
        }
        else
            return _speed;
    }

    private void RopeHanging(Transform playerOwner, Transform otherPlayer)
    {
        float distance = Vector3.Distance(playerOwner.position, otherPlayer.position);

        if (distance > _maxRopeDistance * 1.5f)
        {
            if (!_isOnGround)
            {
                if (playerOwner.position.y < otherPlayer.position.y)
                {
                    if (!_hingeJoint.enabled)
                    {
                        _isMove = false;
                        _hingeJoint.enabled = true;
                        _hingeJoint.connectedBody = otherPlayer.Find("Anchor").GetComponent<Rigidbody2D>();
                    }
                    
                    _rigidbody.AddForce(new Vector2(_move.Value, 0) * 20);
                }
                else
                {
                    if (_hingeJoint.enabled)
                    {
                        _isMove = true;
                        _hingeJoint.enabled = false;
                    }
                }
            }
        }
    }

    public void Jump()
    {
        if (IsOwner)
            if (_isOnGround)
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
    }

    public void SetMoveDirection(int direction)
    {
        direction = Mathf.Clamp(direction, -1, 1);
        _move.Value = direction;
    }

    private void Flip()
    {
        _lookRight = !_lookRight;
        _playerSprite.transform.Rotate(0, 180, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _raycastLength);
    }
}
