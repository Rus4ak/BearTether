using UnityEngine;

public class ChoiceLevel : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _ground;

    private Rigidbody2D _playerRb;
    private Animator _playerAnimator;
    private float _screenWidth;

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
            _ground.transform.position -= new Vector3(Time.deltaTime * 2, 0, 0);

            if (_ground.transform.position.x < -_screenWidth)
                _ground.transform.position = new Vector3(_screenWidth, 0, 0);
        }
    }
}
