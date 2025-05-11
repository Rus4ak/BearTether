using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;

    [NonSerialized] public float musicVolume = .7f;
    [NonSerialized] public float soundVolume = .7f;

    private void Start()
    {
        musicVolume = Options.Instance.optionsData.musicVolume;
        _musicSlider.value = musicVolume;

        soundVolume = Options.Instance.optionsData.soundVolume;
        _soundSlider.value = soundVolume;
    }

    public void ChangeMusicVolume()
    {
        musicVolume = _musicSlider.value;
        Options.Instance.optionsData.musicVolume = musicVolume;
        Options.Instance.Save();
    }

    public void ChangeSoundVolume()
    {
        soundVolume = _soundSlider.value;
        Options.Instance.optionsData.soundVolume = soundVolume;
        Options.Instance.Save();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
