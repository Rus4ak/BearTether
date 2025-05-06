using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _raycastLength = 1.3f;
    [SerializeField] private float _maxRopeDistance = 2f;
    [SerializeField] private float _ropePullStrength = 35f;
    [SerializeField] private LayerMask _jumpLayer;
    [SerializeField] private Transform _spawnPosition;

    private NetworkVariable<float> _move = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;
    private float _currentSpeed;

    private bool _lookRight = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _currentSpeed = _speed;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Multiplayer")
            return;

        Moving();

        if (_move.Value > 0 && _lookRight == false)
            Flip();
        
        else if (_move.Value < 0 && _lookRight == true)
            Flip();
        
        if (_isOnGround)
            _animator.SetBool("Jump", false);

        else
            _animator.SetBool("Jump", true);
        
        if (transform.position.y < -5f)
        {
            if (IsOwner)
            {
                NetworkFinish.Instance.AddAttemptServerRpc();
                transform.position = _spawnPosition.position;
            }
        }
    }

    private void FixedUpdate()
    {
        _isOnGround = Physics2D.Raycast(transform.position, Vector2.down, _raycastLength, _jumpLayer);

        if (!IsOwner || SceneManager.GetActiveScene().name == "Multiplayer")
            return;

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
            float maxDot = 0f;
            float maxOvershoot = 0f;
            _currentSpeed = _speed; 

            if (myIndex > 0)
            {
                Vector3 otherPos = players[myIndex - 1].player.transform.position;
                float dist = Vector3.Distance(transform.position, otherPos);

                if (dist > _maxRopeDistance)
                {
                    float overshoot = dist - _maxRopeDistance;
                    Vector2 dir = (transform.position - otherPos).normalized;
                    float dot = Vector2.Dot(myVelocity, dir);

                    if (dot > maxDot)
                    {
                        maxDot = dot;
                        maxOvershoot = overshoot;
                    }
                }
            }

            if (myIndex < players.Count - 1)
            {
                Vector3 otherPos = players[myIndex + 1].player.transform.position;
                float dist = Vector3.Distance(transform.position, otherPos);

                if (dist > _maxRopeDistance)
                {
                    float overshoot = dist - _maxRopeDistance;
                    Vector2 dir = (transform.position - otherPos).normalized;
                    float dot = Vector2.Dot(myVelocity, dir);

                    if (dot > maxDot)
                    {
                        maxDot = dot;
                        maxOvershoot = overshoot;
                    }
                }
            }

            if (maxDot > 0.1f)
            {
                if (maxOvershoot > 1f)
                    _currentSpeed = _speed / (1.5f * maxOvershoot);
                else
                    _currentSpeed = _speed / 2f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            if (IsOwner)
            {
                NetworkFinish.Instance.AddAttemptServerRpc();
                transform.position = _spawnPosition.position;
            }
        }
    }

    private void Moving()
    {
        if (IsOwner)
            _rigidbody.linearVelocity = new Vector2(_move.Value * _currentSpeed, _rigidbody.linearVelocity.y);
        
        if (_move.Value != 0)
            _animator.SetBool("Run", true);
        else
            _animator.SetBool("Run", false);
        
        if (IsOwner)
            if (_rigidbody.linearVelocity.y == 0 && !_isOnGround)
                _rigidbody.linearVelocity += new Vector2(0, -10);
    }

    public void Jump()
    {
        if (_isOnGround)
            if (IsOwner)
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
        transform.Rotate(0, 180, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _raycastLength);
    }
}
