using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _raycastLength = 1.3f;
    [SerializeField] private LayerMask _jumpLayer;
    [SerializeField] private Transform _spawnPosition;

    private NetworkVariable<float> _move = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;

    private bool _lookRight = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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
            _rigidbody.linearVelocity = new Vector2(_move.Value * _speed, _rigidbody.linearVelocity.y);
        
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
