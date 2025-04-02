using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _raycastLength = 1.3f;
    [SerializeField] private LayerMask _jumpLayer;
    [SerializeField] private Transform _spawnPosition;

    private float _move;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;
    private string _sceneName;
    private Multiplayer _multiplayerManager;

    private bool _lookRight = true;

    public static int attempt = 0;

    private void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;

        if (_sceneName != "Multiplayer")
            transform.position = _spawnPosition.position;
        else
        {
            GameObject.Find("MovementButtons").SetActive(false);
            
            int playersCount;

            _multiplayerManager = GameObject.FindWithTag("LevelsManager").GetComponent<Multiplayer>();
            _multiplayerManager.playersCount += 1;
            playersCount = _multiplayerManager.playersCount;

            transform.position = transform.position + Vector3.left * playersCount * 2;

            _multiplayerManager.players[playersCount-1] = this.gameObject;
            _multiplayerManager.InitializePlayer();
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_sceneName == "Multiplayer")
            return;

        Moving();

        if (_move > 0 && _lookRight == false)
            Flip();
        
        else if (_move < 0 && _lookRight == true)
            Flip();
        
        if (_isOnGround)
            _animator.SetBool("Jump", false);

        else
            _animator.SetBool("Jump", true);

        if (transform.position.y < -5f)
        {
            attempt++;
            transform.position = _spawnPosition.position;
        }
    }

    private void FixedUpdate()
    {
        _isOnGround = Physics2D.Raycast(transform.position, Vector2.down, _raycastLength, _jumpLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
            transform.position = _spawnPosition.position;
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
