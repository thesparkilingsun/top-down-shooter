using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int _score { get; private set; }
    float _lastEnemyKilled;
    int _streakCount;
    float _streakExpiryTime = 1;
     void Start()
    {
        Enemy._OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if(Time.time < _lastEnemyKilled + _streakExpiryTime)
        {
            _streakCount++;
        }
        else
        {
            _streakCount = 0;
        }

        _lastEnemyKilled = Time.time;
        _score += 5 + (int)Mathf.Pow(2, _streakCount);
    }

    void OnPlayerDeath()
    {
        Enemy._OnDeathStatic -= OnEnemyKilled;
    }
}
