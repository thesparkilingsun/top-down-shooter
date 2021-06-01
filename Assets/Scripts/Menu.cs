using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject _mainMenuHolder;
    public GameObject _optionsMenuHolder;
    public Slider[] _volSliders;
    public Toggle[] _resolutionToggle;
    public int[] _screenWidths;
    int _activeScreenResIndex;
    public Toggle _fullScreen;
    void Start()
    {
        _activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        _volSliders[0].value = AudioManager.instance._masterVolPer;
        _volSliders[1].value = AudioManager.instance._musicVolPer;
        _volSliders[2].value = AudioManager.instance._sfxVolPer;

        for(int i=0; i < _resolutionToggle.Length; i++)
        {
            _resolutionToggle[i].isOn = i == _activeScreenResIndex;
        }
        _fullScreen.isOn = isFullscreen;
    }
    public void Play()
    {
        SceneManager.LoadScene("ShooterGame");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        _mainMenuHolder.SetActive(false);
        _optionsMenuHolder.SetActive(true);

    }

    public void MainMenu()
    {
        _mainMenuHolder.SetActive(true);
        _optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i)
    {
        if (_resolutionToggle[i].isOn)
        {
            _activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(_screenWidths[i], (int)(_screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index",_activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetSFXrolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.SFX);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        for (int i = 0; i < _resolutionToggle.Length; i++)
        {
            _resolutionToggle[i].interactable = !isFullScreen;
            
        }
        if (isFullScreen)
        {
            Resolution[] allRes = Screen.resolutions;
            Resolution maxRes = allRes[allRes.Length - 1];
            Screen.SetResolution(maxRes.width, maxRes.height, true);
        }
        else
        {
            SetScreenResolution(_activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreeen", ((isFullScreen) ? 1 :0));
        PlayerPrefs.Save();
    }



}
