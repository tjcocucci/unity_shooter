using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip musicTheme;
    public AudioClip menuTheme;

    void Start () {
        AudioManager.instance.PlayMusic(musicTheme, 2);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
                AudioManager.instance.PlayMusic(menuTheme, 1);
        }
    }

}
