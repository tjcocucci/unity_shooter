using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {Master, Music, Sfx};
    public int numberOfMusicSources = 2;
    AudioSource[] musicSources;
    AudioSource sfx2DSource;

    public float masterVolume {get; private set;}
    public float musicVolume {get; private set;}
    public float sfxVolume {get; private set;}
    
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
            GameObject newsfx2DSource = new GameObject("2D sfx");
            sfx2DSource = newsfx2DSource.AddComponent<AudioSource>();
            newsfx2DSource.transform.parent = transform;

            library = FindObjectOfType<AudioLibrary>();

            audioListenerTrasnform = FindObjectOfType<AudioListener> ().transform;

            masterVolume = PlayerPrefs.GetFloat("Master volume", 1);
            musicVolume = PlayerPrefs.GetFloat("Music volume", 1);
            sfxVolume = PlayerPrefs.GetFloat("sfx volume", 1);

            SceneManager.sceneLoaded += FindPlayer;
        }
    }

    void FindPlayer (Scene scene, LoadSceneMode mode) {
        if (scene.name == "Game") {
            if (FindObjectOfType<Player> () != null) {
                playerTransform = FindObjectOfType<Player> ().transform;
            }
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

    public void PlaySound2D (string audioName) {
        sfx2DSource.PlayOneShot(library.GetClipFromName(audioName), sfxVolume * masterVolume);
    }

    IEnumerator AnimateMusicCrossfade (float fadeDuration) {
        float  percent = 0;
        int perviousMusicSourceIndex = (activeMusicSourceIndex - 1 + numberOfMusicSources) % numberOfMusicSources;
        print("perviousMusicSourceIndex" + perviousMusicSourceIndex);

        while (percent <= 1) {
            percent += Time.deltaTime * (1 / fadeDuration);
            musicSources[perviousMusicSourceIndex].volume = Mathf.Lerp(masterVolume * musicVolume, 0, percent);
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, masterVolume * musicVolume, percent);
            yield return null;
        }
    }

    public void SetVolume (float volume, AudioChannel channel) {
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
            PlayerPrefs.Save();
    }



}
