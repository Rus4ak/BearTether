using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [NonSerialized] public float soundVolume = .7f;
    [SerializeField] private SoundManager _soundManager;

    private void Start()
    {
        soundVolume = Options.Instance.optionsData.soundVolume;
        _soundSlider.value = soundVolume;
    }

    public void ChangeSoundVolume()
    {
        soundVolume = _soundSlider.value;
        SoundVolume.Instance.volume = soundVolume;
        Options.Instance.optionsData.soundVolume = soundVolume;
        Options.Instance.Save();
        _soundManager.ChangeVolume();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
