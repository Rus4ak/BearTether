using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject _light;
    [SerializeField] private AudioSource _lampSound;

    private bool _isStart = false;

    private void Update()
    { 
        if (Daytime.timeRadius < .25f)
        {
            if (!_isStart)
            {
                _light.SetActive(true);
                _lampSound.Play();
                _isStart = true;
            }
        }
        else
        {
            if (_isStart)
            {
                if (_lampSound.isPlaying) 
                    _lampSound.Stop();

                _light.SetActive(false);
                _isStart=false;
            }
        }
    }
}
