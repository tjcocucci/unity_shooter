using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip musicTheme;
    public AudioClip menuTheme;

    void Awake () {
        SceneManager.sceneLoaded += StartLevelMusic;
    }

    void StartLevelMusic (Scene scene, LoadSceneMode mode) {
        if (scene.name == "Menu") {
            AudioManager.instance.PlayMusic(menuTheme, 2);
        } else if (scene.name == "Game") {
            AudioManager.instance.PlayMusic(musicTheme, 2);
        }
    }

}
