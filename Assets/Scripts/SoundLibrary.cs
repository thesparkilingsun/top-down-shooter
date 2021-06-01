using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] _soundGroup;

    Dictionary<string, AudioClip[]> _groupDictionary = new Dictionary<string, AudioClip[]>();

    void Awake()
    {
        foreach(SoundGroup soundgroup in _soundGroup)
        {
            _groupDictionary.Add(soundgroup.groupID, soundgroup.group);
        }
    }
    public AudioClip GetClipFromName(string soundName)
    {
        if (_groupDictionary.ContainsKey(soundName))
        {
            AudioClip[] sounds = _groupDictionary[soundName];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }
    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}
