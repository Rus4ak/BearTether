using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _raycastLength = 1.1f;
    [SerializeField] private LayerMask _jumpLayer;

    private float _move;
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
        Moving();

        if (_move > 0 && _lookRight == false)
            Flip();
        
        else if (_move < 0 && _lookRight == true)
            Flip();
        
        if (_isOnGround)
            _animator.SetBool("Jump", false);

        else
            _animator.SetBool("Jump", true);
    }

    private void FixedUpdate()
    {
        _isOnGround = Physics2D.Raycast(transform.position, Vector2.down, _raycastLength, _jumpLayer);
    }

    private void Moving()
    {
        _rigidbody.linearVelocity = new Vector2(_move * _speed, _rigidbody.linearVelocity.y);
        
        if (_move != 0)
            _animator.SetBool("Run", true);
        else
            _animator.SetBool("Run", false);
        
        if (_rigidbody.linearVelocity.y == 0 && !_isOnGround)
            _rigidbody.linearVelocity += new Vector2(0, -10);
    }

    public void Jump()
    {
        if (_isOnGround)
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
    }

    public void SetMoveDirection(int direction)
    {
        direction = Mathf.Clamp(direction, -1, 1);
        _move = direction;
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
