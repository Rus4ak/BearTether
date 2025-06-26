using UnityEngine;

public class StoneTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _stone;
    [SerializeField] private float _force;
    [SerializeField] private int _direction;

    private Vector3 _startPosition;
    private Transform _player;
    private bool _isActive = false;

    private void Start()
    {
        _startPosition = _stone.transform.position;
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (_player.position.x < 5)
        {
            _stone.linearVelocity = Vector3.zero;
            _stone.angularVelocity = 0;
            _stone.transform.position = _startPosition;
            _isActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!_isActive)
            {
                _isActive = true;
                _stone.AddForce(new Vector2(_direction * _force, 0));
            }
        }
    }
}
