using UnityEngine;

public class ChoiceLevel : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _ground;

    private Rigidbody2D _playerRb;
    private Animator _playerAnimator;
    private float _screenWidth;

    public Transform Ground
    {
        get { return _ground; }
        set { _ground = value; }
    }

    private void Start()
    {
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _playerAnimator = _player.GetComponent<Animator>();
        _playerAnimator.SetBool("Run", true);

        float screenHeight = Camera.main.orthographicSize * 2;
        _screenWidth = screenHeight * Screen.width / Screen.height;
    }

    private void Update()
    {
        if (_player.transform.position.x < -1)
        {
            _playerRb.linearVelocity = new Vector3(5, 0, 0);
        }
        else
        {
            _playerRb.linearVelocity = Vector3.zero;

            Ground.transform.position -= new Vector3(Time.deltaTime * 2, 0, 0);

            if (Ground.transform.position.x < -_screenWidth)
                Ground.transform.position = new Vector3(_screenWidth, 0, 0);
        }
    }
}
