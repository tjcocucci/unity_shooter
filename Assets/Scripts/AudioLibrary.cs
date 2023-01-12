using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
    public AudioGroup[] soundGroups;

    void Start () {
        foreach(AudioGroup audioGroup in soundGroups) {
            groupDictionary.Add(audioGroup.groupName, audioGroup.clips);
        }
    }

    public AudioClip GetClipFromName(string name) {
        if (groupDictionary.ContainsKey(name)) {
            AudioClip[] clips = groupDictionary[name];
            if(clips.Length > 0) {
                return clips[Random.Range(0, clips.Length)];
            }
        }
        return null;
    }

    [System.Serializable]
    public class AudioGroup {
        public string groupName;
        public AudioClip[] clips;
    }
}
