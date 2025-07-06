using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] private GameObject[] _lights;

    private float _timeToNext;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _timeToNext = Time.time + Random.Range(5, 30);
    }

    private void Update()
    {
        if (Time.time >= _timeToNext)
        {
            StartLightning();

            _timeToNext = Time.time + Random.Range(5, 30);
        }
    }

    private void StartLightning()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();

        _audioSource.Play();

        int direction = Random.Range(0, _lights.Length);

        _lights[direction].SetActive(true);
        Invoke(nameof(EndLightning), .1f);
    }

    private void EndLightning()
    {
        foreach (GameObject light in _lights)
        {
            light.SetActive(false);
        }
    }
}
