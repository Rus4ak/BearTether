using UnityEngine;

public class ErrorText : MonoBehaviour
{
    private Animation _animation;

    private void Start()
    {
        _animation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (!_animation.isPlaying)
            Destroy(gameObject);
    }
}
