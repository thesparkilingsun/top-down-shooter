using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    public Image _fadePlane;
    public GameObject _gameOverUI;
    public RectTransform _newWaveBanner;
    public Text _waveBannerTitle;
    public Text _enemyCountText;
    public Text _gameOverScoreUI;

    public Text _scoreUI;
    public RectTransform _healthBar;
    Spawner _spawner;

    Player _player;
    void Start()
    {
        _player = FindObjectOfType<Player>();
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }
    void Awake()
    {
        _spawner = FindObjectOfType<Spawner>();
        _spawner.OnNewWave += OnNewWave;
    }

     void Update()
    {
        _scoreUI.text = ScoreKeeper._score.ToString("D6");
        float healtPercent = 0;
        if (_player != null)
        {
             healtPercent = _player._health / _player._startHealth;
            
        }
        _healthBar.localScale = new Vector3(healtPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One","Two","Three","Four","Five" };
        _waveBannerTitle.text = "-Wave "+ numbers[waveNumber-1] + " -";
        _enemyCountText.text = "Enemies: " + _spawner._waves[waveNumber - 1].enemycount;

        StartCoroutine(AnimateWaveBanner());
    }

    IEnumerator AnimateWaveBanner()
    {
        float animatePercent = 0;
        float speed = 2.5f;
        float delayTime = 1f;
        int dir = 1;

        float endDelay = Time.time + 1 / speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;
            
            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelay)
                {
                    dir = -1;
                }
            }

            _newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-71, 102, animatePercent);
            yield return null;
        }
    }
    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear,new Color(0,0,0,0.8f),1));
        _gameOverScoreUI.text = _scoreUI.text;
        _scoreUI.gameObject.SetActive(false);
        _healthBar.transform.parent.gameObject.SetActive(false);
        _gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            _fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }


    //UI Input
    //Start New Game
    
    public void StartGame()
    {
        SceneManager.LoadScene("ShooterGame");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
