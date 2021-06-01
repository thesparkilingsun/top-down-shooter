using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip _mainTheme;
    public AudioClip _menuTheme;

    string _sceneName;
    void Start()
    {
        AudioManager.instance.PlayMusic(_mainTheme, 1);
        OnLevelWasLoaded(0);
        
    }

     void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != _sceneName)
        {
            _sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        if(_sceneName == "Menu")
        {
            clipToPlay = _menuTheme;
        }
        else if (_sceneName == "Shooting Game")
        {
            clipToPlay = _mainTheme;
        }

        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
