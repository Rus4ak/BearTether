using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Transform _moveTo;
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;

    private Vector3 _startPosition;
    private GameObject _child;

    private GameObject[] _players;
    private bool _isMove = false;

    private void Start()
    {
        _child = transform.GetChild(0).gameObject;
        _startPosition = transform.position;
        _players = GameObject.FindGameObjectsWithTag("Player");

        if (_moveTo == null)
        {
            
            _moveTo = _players[Random.Range(0, _players.Length - 1)].transform;
        }
    }

    private void Update()
    {
        foreach (GameObject player in _players)
        {
            if (transform.position.x - player.transform.position.x <= _distance)
            {
                _isMove = true;
                _child.SetActive(true);
            }

            if (player.transform.position.x < 0)
            {
                _child.SetActive(false);
                _isMove = false;
                transform.position = _startPosition;
            }
        }

        if (Vector3.Distance(transform.position, _moveTo.position) < 1)
            _child.SetActive(false);

        if (!_isMove)
            return;

        transform.position = Vector3.Lerp(transform.position, _moveTo.position, _speed * Time.deltaTime);
    }
}
