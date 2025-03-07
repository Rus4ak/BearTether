using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Animation _button;

    private Rigidbody2D _playerRb;
    private Animator _playerAnimator;
    private Vector3 _endPos;

    private bool _isPlay = false;

    private void Start()
    {
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _playerAnimator = _player.GetComponent<Animator>();

        float screenHeight = Camera.main.orthographicSize * 2 + .2f;
        float screenWidth = screenHeight * Screen.width / Screen.height + .2f;

        _endPos = new Vector3(screenWidth / 2.2f, 0, 0);
    }

    private void Update()
    {
        if (_isPlay)
        {
            _playerRb.linearVelocity = new Vector3(8, 0, 0);
            
            if (_player.transform.position.x > _endPos.x)
                SceneManager.LoadScene("ChoiceLevel");
        }
    }

    public void Play()
    {
        _isPlay = true;
        _button.Play();
        _playerAnimator.SetBool("Run", true);
    }
}
