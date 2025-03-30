using UnityEngine;
using UnityEngine.SceneManagement;

public class Cancel : MonoBehaviour
{
    [SerializeField] private string _nameScene;

    private Animation _animation;

    private bool _isPressed = false;

    private void Start()
    {
        _animation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (_isPressed && !_animation.isPlaying)
        {
            SceneManager.LoadScene(_nameScene);
        }
    }

    public void PressButton()
    {
        _animation.Play();
        _isPressed = true;
    }
}
