using UnityEngine;
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

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Moving();
        Jump();
        
        if (_move > 0 && _lookRight == false)
            Flip();
        
        else if (_move < 0 && _lookRight == true)
            Flip();
    }

    private void FixedUpdate()
    {
        _isOnGround = Physics2D.Raycast(transform.position, Vector2.down, _raycastLength, _jumpLayer);
    }

    private void Moving()
    {
        _move = Input.GetAxis("Horizontal");
        _rigidbody.linearVelocity = new Vector2(_move * _speed, _rigidbody.linearVelocity.y);
        
        if (_move != 0)
            _animator.SetBool("Run", true);
        else
            _animator.SetBool("Run", false);
    }

    private void Jump()
    {
        if (_isOnGround == true && Input.GetKeyDown(KeyCode.Space))
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
        
        if (_isOnGround)
            _animator.SetBool("Jump", false);

        else
            _animator.SetBool("Jump", true);
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
