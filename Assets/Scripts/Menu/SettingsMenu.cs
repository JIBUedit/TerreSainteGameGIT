using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Dropdown resolutionDropdown;

    Resolution[] resolutions;

    public Slider musicSlider;
    public Slider soundSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SoundVolumeKey = "SoundVolume";

    public void Start()
    {
        // Charger les valeurs sauvegardées ou définir les valeurs par défaut
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f); // Par défaut : 1
        float savedSoundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, 1f); // Par défaut : 1

        // Appliquer les valeurs aux sliders et au mixer
        musicSlider.value = savedMusicVolume;
        soundSlider.value = savedSoundVolume;

        SetVolume(savedMusicVolume);
        SetSoundVolume(savedSoundVolume);

        // Gestion des résolutions
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Screen.fullScreen = true;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Music", 20 * Mathf.Log10(volume));
        // Sauvegarder la valeur
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("Sound", 20 * Mathf.Log10(volume));
        // Sauvegarder la valeur
        PlayerPrefs.SetFloat(SoundVolumeKey, volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
