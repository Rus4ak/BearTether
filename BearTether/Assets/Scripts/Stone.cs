using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private AudioSource _hitSound;
    [SerializeField] private AudioSource _rollSound;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            if (!_hitSound.isPlaying)
                _hitSound.Play();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Mathf.Abs(_rb.linearVelocity.x) > 2)
        {
            if (!_rollSound.isPlaying)
                _rollSound.Play();

            _rollSound.volume = Mathf.Clamp(Mathf.Abs(_rb.linearVelocity.x) - 2, .1f, SoundVolume.Instance.volume);
        }
        else
        {
            if (_rollSound.isPlaying)
                _rollSound.Stop();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_rollSound.isPlaying)
            _rollSound.Stop();
    }
}
