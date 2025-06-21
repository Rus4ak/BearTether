using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 12;
    [SerializeField] private float _raycastLength = 1.3f;
    [SerializeField] private LayerMask _jumpLayer;
    [SerializeField] private Transform _spawnPosition;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _runParticleSystem;
    [SerializeField] private ParticleSystem _jumpParticleSystem;
    [SerializeField] private Color[] _particleColors = new Color[2];

    [Header("Sounds")]
    [SerializeField] private AudioSource _stepsSound;
    [SerializeField] private AudioSource _jumpSound;

    private float _move;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private bool _isOnGround;

    private bool _lookRight = true;

    public static int attempt;

    private void Start()
    {
        attempt = 0;
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        ParticleSystem.MainModule[] main = new ParticleSystem.MainModule[2] { _runParticleSystem.main, _jumpParticleSystem.main };
        
        for (int i = 0; i < main.Length; i++)
            main[i].startColor = new ParticleSystem.MinMaxGradient(_particleColors[0], _particleColors[1]);
    }

    private void Update()
    {
        Moving();
        
        if (_move > 0 && _lookRight == false)
            Flip();
        
        else if (_move < 0 && _lookRight == true)
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
        {
            attempt++;
            transform.position = _spawnPosition.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            attempt++;
            transform.position = _spawnPosition.position;
        }
    }

    private void Moving()
    {
        _rigidbody.linearVelocity = new Vector2(_move * _speed, _rigidbody.linearVelocity.y);
        
        if (_move != 0 && _isOnGround)
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
