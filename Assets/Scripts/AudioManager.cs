using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioSource running;
    public AudioSource jumping;
    public AudioSource landing;

    [Header("Background Music")]
    public AudioSource musicAudioSource;

    [Header("UI Sliders")]
    public Slider soundSlider;
    public Slider musicSlider;

    void Start()
    {
       
        float defaultVolume = 0.5f;

        if (soundSlider != null)
            soundSlider.value = defaultVolume;

        if (musicSlider != null)
            musicSlider.value = defaultVolume;

        UpdateSoundVolumes(defaultVolume);
        UpdateMusicVolume(defaultVolume);
    }

    void Update()
    {
        // Update volumes in real-time
        if (soundSlider != null)
            UpdateSoundVolumes(soundSlider.value);

        if (musicSlider != null)
            UpdateMusicVolume(musicSlider.value);
    }

    void UpdateSoundVolumes(float volume)
    {
        if (running != null) running.volume = volume;
        if (jumping != null) jumping.volume = volume;
        if (landing != null) landing.volume = volume;
    }

    void UpdateMusicVolume(float volume)
    {
        if (musicAudioSource != null)
            musicAudioSource.volume = volume;
    }
}