using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject OptionsMenu;
    public GameObject MainMenu;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullScreenToggle;
    public int[] widths = {960, 1280, 1920};

    int activeResolutionIndex;

    public void Start () {
        volumeSliders[0].value = AudioManager.instance.masterVolume;
        volumeSliders[1].value = AudioManager.instance.musicVolume;
        volumeSliders[2].value = AudioManager.instance.sfxVolume;

        activeResolutionIndex = PlayerPrefs.GetInt("Resolution index");
        for (int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].isOn = (i == activeResolutionIndex);
        }

        bool isFullscreen = (PlayerPrefs.GetInt("Fullscreen") == 1) ? true : false;

        fullScreenToggle.isOn = isFullscreen;

    }

    public void PlayGame () {
        SceneManager.LoadScene("Game");
    }

    public void Quit () {
        Application.Quit();
    }

    public void Options () {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void BackToMainMenu () {
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void SetFullscreen (bool isFullscreen) {
        for (int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen) {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution resolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(resolution.width, resolution.height, true);
        } else {
            SetResolution(activeResolutionIndex);
        }
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetResolution (int i) {

        if (resolutionToggles[i].isOn) {
            float aspectRatio = 19 / 6f;
            activeResolutionIndex = i;
            int height = (int) aspectRatio * widths[i];
            Screen.SetResolution(widths[i], height, false);
            PlayerPrefs.SetInt("Resolution index", activeResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume (float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume (float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume (float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

}
