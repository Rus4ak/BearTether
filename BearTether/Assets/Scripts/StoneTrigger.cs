using UnityEngine;

public class StoneTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _stone;
    [SerializeField] private float _force;
    [SerializeField] private int _direction;

    private Vector3 _startPosition;
    private Transform _player;

    private void Start()
    {
        _startPosition = _stone.transform.position;
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (_player.position.x < 0)
        {
            _stone.linearVelocity = Vector3.zero;
            _stone.angularVelocity = 0;
            _stone.transform.position = _startPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _stone.AddForce(new Vector2(_direction * _force, 0));
        }
    }
}
