using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, SFX, Music};
   public float _masterVolPer { get; private set; }
    public float _sfxVolPer  { get; private set; }
    public float _musicVolPer { get; private set; }
    

    AudioSource[] _musicSources;
    AudioSource _sfx2DSource;
    int activeMusicIndex;


    public static AudioManager instance;
    Transform _audioListener;
    Transform _playerT;

    SoundLibrary library;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
            _musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Muisc sources " + (i + 1));
                _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            GameObject newSFX2DSource = new GameObject("2DMuisc sources ");
            _sfx2DSource = newSFX2DSource.AddComponent<AudioSource>();
            newSFX2DSource.transform.parent = transform;

            _masterVolPer =  PlayerPrefs.GetFloat("master vol", 1);
           _sfxVolPer =  PlayerPrefs.GetFloat("sfx vol", 1);
           _musicVolPer =  PlayerPrefs.GetFloat("music vol", 1);
        }
        

        _audioListener = FindObjectOfType<AudioListener>().transform;
        if(FindObjectOfType<Player>() != null)
        {
            _playerT = FindObjectOfType<Player>().transform;
        }
        
        library = GetComponent<SoundLibrary>();
        
    }

    void Update()
    {
        if(_playerT != null)
        {
            _audioListener.position = _playerT.position;
        }
        
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicIndex = 1 - activeMusicIndex;
        _musicSources[activeMusicIndex].clip = clip;
        _musicSources[activeMusicIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound2d(string soundName)
    {
        _sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), _sfxVolPer * _masterVolPer);
    }
    

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                _masterVolPer = volumePercent;
                break;
            case AudioChannel.SFX:
                _sfxVolPer = volumePercent;
                break;
            case AudioChannel.Music:
                _musicVolPer = volumePercent;
                break;
        }

        _musicSources[0].volume = _musicVolPer * _masterVolPer;
        _musicSources[1].volume = _musicVolPer * _masterVolPer;

        PlayerPrefs.SetFloat("master vol", _masterVolPer); 
        PlayerPrefs.SetFloat("sfx vol",_sfxVolPer);
        PlayerPrefs.SetFloat("music vol", _musicVolPer);
        PlayerPrefs.Save();


    }

    public void PlaySound(string soundName, Vector3 pos) {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound(AudioClip clip,Vector3 pos)
    {  if(clip != null) {
            AudioSource.PlayClipAtPoint(clip, pos, _sfxVolPer * _masterVolPer);

        }
        
    }


    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;
        while(percent < 1)
        {
            percent= Time.deltaTime * 1 / duration;

            _musicSources[activeMusicIndex].volume = Mathf.Lerp(0, _musicVolPer * _masterVolPer,  percent);
            _musicSources[1- activeMusicIndex].volume = Mathf.Lerp(_musicVolPer * _masterVolPer,0, percent);
            yield return null;


        }
    }
}
