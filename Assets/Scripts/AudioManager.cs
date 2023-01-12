using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {Master, Music, Sfx};
    public int numberOfMusicSources = 2;
    AudioSource[] musicSources;

    [Header("Volume")]
    public float masterVolume = 1;
    public float musicVolume = 1;
    public float sfxVolume = 1;
    
    public static AudioManager instance;

    AudioLibrary library;
    Transform playerTransform;
    Transform audioListenerTrasnform;
    int activeMusicSourceIndex;

    void Awake () {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[numberOfMusicSources];
            for (int i = 0; i < numberOfMusicSources; i++) {
                GameObject newMusicSource = new GameObject("Music source (" + (i+1) + ")");
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;

            }

            library = FindObjectOfType<AudioLibrary>();
            playerTransform = FindObjectOfType<Player> ().transform;
            audioListenerTrasnform = FindObjectOfType<AudioListener> ().transform;

            masterVolume = PlayerPrefs.GetFloat("Master volume", masterVolume);
            musicVolume = PlayerPrefs.GetFloat("Music volume", musicVolume);
            sfxVolume = PlayerPrefs.GetFloat("sfx volume", sfxVolume);

        }
    }

    void Update () {
        if (playerTransform != null) {
            audioListenerTrasnform.position = playerTransform.position;
            audioListenerTrasnform.rotation = playerTransform.rotation;
        }
    }

    public void PlayMusic (AudioClip clip, float fadeDuration = 1) {
        activeMusicSourceIndex = (activeMusicSourceIndex + 1) % numberOfMusicSources;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound (AudioClip clip, Vector3 position) {
        float volume = masterVolume * sfxVolume;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlaySound (string audioName, Vector3 position) {
        AudioClip clip = library.GetClipFromName(audioName);
        PlaySound(clip, position);
    }

    IEnumerator AnimateMusicCrossfade (float fadeDuration) {
        float  percent = 0;
        int nextMusicSourceIndex = (activeMusicSourceIndex + 1) % numberOfMusicSources;

        while (percent <= 1) {
            percent += Time.deltaTime * (1 / fadeDuration);
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(masterVolume * musicVolume, 0, percent);
            musicSources[nextMusicSourceIndex].volume = Mathf.Lerp(0, masterVolume * musicVolume, percent);
            yield return null;
        }
    }

    void SetVolume (float volume, AudioChannel channel) {
        switch (channel) {
            case AudioChannel.Master:
                masterVolume = volume;
                break;
            case AudioChannel.Music:
                musicVolume = volume;
                break;
            case AudioChannel.Sfx:
                sfxVolume = volume;
                break;
        }
        for (int i = 0; i < numberOfMusicSources; i++) {
            musicSources[i].volume = musicVolume * masterVolume;
        }
            PlayerPrefs.SetFloat("Master volume", masterVolume);
            PlayerPrefs.SetFloat("Music volume", musicVolume);
            PlayerPrefs.SetFloat("sfx volume", sfxVolume);
    }



}
