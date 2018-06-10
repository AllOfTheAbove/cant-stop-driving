using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour {

    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;

    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private bool fullscreen;

    void Start()
    {
        fullscreen = Screen.fullScreen;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRate + "fps)";
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        GameObject.Find("Fullscreen").GetComponent<Toggle>().isOn = Screen.fullScreen;
        GameObject.Find("MusicVolume").GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");
        GameObject.Find("SoundVolume").GetComponent<Slider>().value = PlayerPrefs.GetFloat("soundVolume");
    }

	public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("soundVolume", volume);
        PlayerPrefs.SetFloat("soundVolume", volume);
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    private void Update()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetFullscreen()
    {
            fullscreen = fullscreenToggle.isOn;
            Screen.fullScreen = fullscreen;
            Resolution resolution = Screen.currentResolution;
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
        Debug.Log(fullscreenToggle.isOn);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }

}
