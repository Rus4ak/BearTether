using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] private GameObject _pullRopeButton;
    [SerializeField] private float _maxRopeDistance = 3f;
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
    private NetworkVariable<bool> _isPullRope = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;
    private float _currentSpeed;
    private HingeJoint2D _hingeJoint;
    private NetworkPlayer _networkPlayer;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    
    private bool _isMove = true;
    private bool _lookRight = true;

    [NonSerialized] public float minPosY;
    [NonSerialized] public Vector3 spawnPosition;
    [NonSerialized] public PullRope pullRopeLeft;
    [NonSerialized] public PullRope pullRopeRight;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = _playerSprite.GetComponent<Animator>();
        _hingeJoint = GetComponent<HingeJoint2D>();
        _networkPlayer = GetComponent<NetworkPlayer>();
        _spriteRenderer = _playerSprite.GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _currentSpeed = _speed;

        AudioListener audioListener;

        if (Camera.main.TryGetComponent<AudioListener>(out audioListener))
            Destroy(audioListener);

        if (IsOwner)
            gameObject.AddComponent<AudioListener>();
        
        ChangeParticleColors(_particleColors[0], _particleColors[1]);
    }

    private void Update()
    {
        if (_networkPlayer.sceneName == "Multiplayer")
            return;

        if (minPosY == default)
        {
            Transform borders = GameObject.FindWithTag("CameraBorders").transform;
            minPosY = borders.Find("MinPos").position.y;
        }

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
        
        if (transform.position.y < minPosY)
        {
            if (IsOwner)
                DeadServerRpc();
        }

        if (_isPullRope.Value && !_animator.GetBool("Pull"))
            _animator.SetBool("Pull", true);

        if (!_isPullRope.Value && _animator.GetBool("Pull"))
            _animator.SetBool("Pull", false);
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

    private void OnCollisionEnter2D(Collision2D collision)
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
                    PullRope(transform, otherPlayer, pullRopeRight);

                    _currentSpeed = speed;
                }

                if (myIndex < players.Count - 1)
                {
                    Transform otherPlayer = players[myIndex + 1].player.transform;

                    float speed = CalculateDistance(transform, otherPlayer);
                    RopeHanging(transform, otherPlayer);
                    PullRope(transform, otherPlayer, pullRopeLeft);

                    _currentSpeed = speed;
                }

                if (myIndex > 0 && myIndex < players.Count - 1)
                {
                    Transform otherPlayerLeft = players[myIndex - 1].player.transform;
                    Transform otherPlayerRight = players[myIndex + 1].player.transform;

                    float speedLeft = CalculateDistance(transform, otherPlayerLeft);
                    float speedRight = CalculateDistance(transform, otherPlayerRight);

                    if (otherPlayerLeft.position.y < otherPlayerRight.position.y)
                        PullRope(transform, otherPlayerLeft, pullRopeLeft);
                    else
                        PullRope(transform, otherPlayerRight, pullRopeRight);

                    _currentSpeed = Mathf.Min(speedLeft, speedRight);
                }
            }

            if (_isMove)
            {
                _rigidbody.linearVelocity = new Vector2(_move.Value * _currentSpeed, _rigidbody.linearVelocity.y);
            }

            if (pullRopeLeft != null)
                PullRopeMoving(pullRopeLeft);
            
            if (pullRopeRight != null)
                PullRopeMoving(pullRopeRight);
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
                if (playerOwner.position.y < otherPlayer.position.y - 2)
                {
                    if (!_hingeJoint.enabled)
                    {
                        _isMove = false;

                        if (!_isPullRope.Value)
                        {
                            _hingeJoint.enabled = true;
                            _hingeJoint.connectedBody = otherPlayer.Find("Anchor").GetComponent<Rigidbody2D>();
                        }
                    }
                    
                    _rigidbody.AddForce(new Vector2(_move.Value, 0) * 30);
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
            else
            {
                if (_hingeJoint.enabled)
                {
                    _isMove = true;
                    _hingeJoint.enabled = false;
                }
            }
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

    private void PullRope(Transform playerOwner, Transform otherPlayer, PullRope pullRope)
    {
        if (otherPlayer.position.y < playerOwner.position.y - 2)
        {
            if (!_pullRopeButton.activeInHierarchy)
                _pullRopeButton.SetActive(true);
        }
        else
        {
            if (_pullRopeButton.activeInHierarchy)
                if (!_isPullRope.Value)
                    _pullRopeButton.SetActive(false);
        }

        if (_isPullRope.Value)
        {
            if (_hingeJoint.enabled)
                _hingeJoint.enabled = false;

            NetworkObject pulledObj = otherPlayer.parent.GetComponent<NetworkObject>();
            NetworkObjectReference pulledRef = new NetworkObjectReference(pulledObj);

            pullRope.SetPulledPlayerServerRpc(pulledRef);
        }
    }

    private void PullRopeMoving(PullRope pullRope)
    {
        if (pullRope.PulledPlayer.Value.TryGet(out NetworkObject netObj))
        {
            if (transform.parent == netObj.transform)
            {
                if (pullRope.PullTo.Value.TryGet(out NetworkObject netObj2))
                {
                    if (_rigidbody.gravityScale == 3)
                    {
                        _rigidbody.gravityScale = 0;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, netObj2.transform.GetChild(1).position, 10 * Time.deltaTime);

                    bool isUnderCollider = Physics2D.Raycast(netObj.transform.GetChild(1).position, Vector2.up, _raycastLength, _jumpLayer);

                    if (isUnderCollider)
                    {
                        _collider.isTrigger = true;
                    }
                    else
                    {
                        _collider.isTrigger = false;
                    }

                    if (transform.position == netObj2.transform.GetChild(1).position)
                    {
                        _collider.isTrigger = false;
                    }
                }
            }
        }
        else
        {
            if (_collider.isTrigger)
            {
                _collider.isTrigger = false;
            }

            if (_rigidbody.gravityScale == 0)
            {
                _rigidbody.gravityScale = 3;
            }    
        }
    }

    public void StartPullRope()
    {
        _isPullRope.Value = true;
        _isMove = false;
        _rigidbody.bodyType = RigidbodyType2D.Static;

        NetworkObject pullToObj = transform.parent.GetComponent<NetworkObject>();
        NetworkObjectReference pullToRef = new NetworkObjectReference(pullToObj);
        
        if (pullRopeLeft != null)
            pullRopeLeft.SetPullToServerRpc(pullToRef);

        if (pullRopeRight != null)
            pullRopeRight.SetPullToServerRpc(pullToRef);
    }

    public void StopPullRope()
    {
        _isPullRope.Value = false;
        _isMove = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        if (pullRopeLeft != null)
            pullRopeLeft.DeactivatePullServerRpc();

        if (pullRopeRight != null)
            pullRopeRight.DeactivatePullServerRpc();
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
        _spriteRenderer.flipX = !_lookRight;
    }

    public void ChangeParticleColors(Color color1, Color color2)
    {
        ParticleSystem.MainModule[] main = new ParticleSystem.MainModule[2] { _runParticleSystem.main, _jumpParticleSystem.main };

        for (int i = 0; i < main.Length; i++)
            main[i].startColor = new ParticleSystem.MinMaxGradient(color1, color2);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _raycastLength);
    }
}
