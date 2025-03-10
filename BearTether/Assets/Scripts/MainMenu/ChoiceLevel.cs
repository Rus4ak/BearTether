using UnityEngine;

public class ChoiceLevel : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform[] _grounds;

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
        print(_screenWidth);
    }

    private void Update()
    {
        if (_player.transform.position.x < -2)
        {
            _playerRb.linearVelocity = new Vector3(5, 0, 0);
        }
        else
        {
            foreach (Transform ground in _grounds)
            {
                ground.transform.position -= new Vector3(Time.deltaTime, 0, 0);

                if (ground.transform.position.x < -_screenWidth)
                    ground.transform.position = new Vector3(_screenWidth, 0, 0);
            }
        }
    }
}
