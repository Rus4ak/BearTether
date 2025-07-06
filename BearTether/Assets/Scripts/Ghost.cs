using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Transform _moveTo;
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;

    private Transform _player;
    private Vector3 _startPosition;
    private GameObject _child;

    private bool _isMove = false;

    private void Start()
    {
        _child = transform.GetChild(0).gameObject;
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _startPosition = transform.position;

        if (_moveTo == null)
            _moveTo = _player;
    }

    private void Update()
    {
        if (transform.position.x - _player.position.x <= _distance)
        {
            _isMove = true;
            _child.SetActive(true);
        }

        if (_player.position.x < 0)
        {
            _child.SetActive(false);
            _isMove = false;
            transform.position = _startPosition;
        }

        if (Vector3.Distance(transform.position, _moveTo.position) < 1)
            _child.SetActive(false);

        if (!_isMove)
            return;

        transform.position = Vector3.Lerp(transform.position, _moveTo.position, _speed * Time.deltaTime);
    }
}
