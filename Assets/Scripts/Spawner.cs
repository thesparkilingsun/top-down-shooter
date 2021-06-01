using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
   
    public Wave[] _waves;
    public Enemy _enemy;
    LivingEntity _playerEntity;
    Transform _player;
    Wave _currentWave;
    int _wavesLeft;

    int _enemiesLeftToSpawn;
    int _enemiesRemainingAlive;
    float _spawnTimeRemaining;
    MapGenerator _map;

    //Check timing
    float _timeBetweenCampingChecks = 2;
    float _campThresholdDistance = 1.5f;
    float _nextCampCheckTime;
    Vector3 _campPositionOld;
    bool _isCamping;
    bool _isDisabled;

    //Event
    public event System.Action<int> OnNewWave;
    void Start()
    {
       
        
        _map = FindObjectOfType<MapGenerator>();
        _playerEntity = FindObjectOfType<Player>();
        _player = _playerEntity.transform;
        _playerEntity.OnDeath += OnPlayerDeath;
        _nextCampCheckTime = _timeBetweenCampingChecks + Time.time;
        _campPositionOld = _player.position;
        NextWave();
    }

    void Update()
    {
        if (!_isDisabled) {
            if (Time.time > _nextCampCheckTime)
            {
                _nextCampCheckTime = Time.time + _timeBetweenCampingChecks;
                _isCamping = (Vector3.Distance(_player.position, _campPositionOld) < _campThresholdDistance);
                _campPositionOld = _player.position;
            }

            if (_enemiesLeftToSpawn > 0  || _currentWave.infinite && Time.time > _spawnTimeRemaining)
            {
                _enemiesLeftToSpawn--;
                _spawnTimeRemaining = Time.time + _currentWave.timeBetweenSpawn;
                StartCoroutine(SpawnEnemy());
            }
        }
        
        //else
        //{
        //    print("All enemies dead");
        //}
    }


    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        float spawnTimer = 0;
        //Spawn Enemy at player position if Staganant for n seconds
        Transform spawnTile = _map.GetRadomOpenTile();
        if (_isCamping)
        {
            spawnTile = _map.GetTileFromPosition(_player.position);
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        
        Enemy spawnedEnemy = Instantiate(_enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(_currentWave.moveSpeed,_currentWave.hitsToKillPeople,_currentWave.enemyHealth,_currentWave.skinColor);
    }
    void ResetPlayerPosition()
    {
        _player.position = _map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    void OnPlayerDeath()
    {
        _isDisabled = true;
    }
    void OnEnemyDeath()
    {
        _enemiesRemainingAlive--;
        if(_enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        if(_wavesLeft > 0)
        {
            AudioManager.instance.PlaySound2d("Level Complete");
        }
        _wavesLeft++;
        if(_wavesLeft - 1 < _waves.Length) {
            _currentWave = _waves[_wavesLeft - 1];
            _enemiesLeftToSpawn = _currentWave.enemycount;
            _enemiesRemainingAlive = _enemiesLeftToSpawn;
        }
        if(OnNewWave != null)
        {
            OnNewWave(_wavesLeft);
        }
        ResetPlayerPosition();
    }



    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemycount;
        public float timeBetweenSpawn;

        public float moveSpeed;
        public int hitsToKillPeople;
        public float enemyHealth;
        public Color skinColor;

    }
}
