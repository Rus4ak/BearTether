using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        ChangeVolume();
    }

    public void ChangeVolume()
    {
        _audioSource.volume = SoundVolume.Instance.volume;
    }

    public void PlaySound()
    {
        _audioSource.Play();
    }
}
