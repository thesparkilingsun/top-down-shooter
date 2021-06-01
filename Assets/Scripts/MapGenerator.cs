using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapGenerator : MonoBehaviour
{
    public Map[] _maps;
    public int _mapIndex;
    public Transform _tilePrefab;
    public Transform _obstaclePrefab;
    public Transform _navMeshMaskPrefab;
    public Transform _navMeshFloor;
    public Transform _mapFloor;
    public Vector2 _maxMapSize;

   
    [Range(0,1)]
    public float _outlinePercent;
   

    public float _tileSize;
    //List to store all tile coordinates
    List<Coordinate> _allTileCoordinate;
    Queue<Coordinate> _shuffeledTileCoordinate;
    Queue<Coordinate> _allOpenTileCoordinate;
    
    Map _currentMap;
    Transform[,] _tileMap;
    void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        _mapIndex = waveNumber - 1;
        GenerateMap();
    }
    public void GenerateMap()
    {
        _currentMap = _maps[_mapIndex];
        _tileMap = new Transform[_currentMap.mapSize.x, _currentMap.mapSize.y];
        System.Random prng = new System.Random(_currentMap.seed);
       

        // Generating Coordinates
        _allTileCoordinate = new List<Coordinate>();
        for (int x = 0; x < _currentMap.mapSize.x; x++)
        { for (int y = 0; y < _currentMap.mapSize.y; y++)
            {
                _allTileCoordinate.Add(new Coordinate(x, y));
            }
        }

        _shuffeledTileCoordinate = new Queue<Coordinate>(Utility.ShuffleArray(_allTileCoordinate.ToArray(),_currentMap.seed));
      
        //Creating map holder objects
         string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        //Spawning Tiles
        for(int x=0; x < _currentMap.mapSize.x; x++)
        {
            for(int y=0; y< _currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoorToPosition(x, y);
                Transform tile = Instantiate(_tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                tile.localScale = Vector3.one * (1 - _outlinePercent) * _tileSize;
                tile.parent = mapHolder;
                _tileMap[x, y] = tile;
            }
        }

        //Spawning Obstacles
        bool[,] obstacleMap = new bool[(int)_currentMap.mapSize.x, (int)_currentMap.mapSize.y];
        int obstacleCount = (int)(_currentMap.mapSize.x * _currentMap.mapSize.y * _currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coordinate> allOpenCoordinate = new List<Coordinate>(_allTileCoordinate);
        for (int i = 0; i <obstacleCount; i++)
        {
            Coordinate randCoordinte = GetRandCoordinate();
            obstacleMap[randCoordinte.x, randCoordinte.y] = true;
            currentObstacleCount++;
            if (randCoordinte != _currentMap.mapCenter && MapFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(_currentMap.minObstacleHeight, _currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoorToPosition(randCoordinte.x, randCoordinte.y);
                Transform obstacle = Instantiate(_obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                obstacle.parent = mapHolder;
                obstacle.localScale = new Vector3((1 - _outlinePercent) * _tileSize, obstacleHeight, (1 - _outlinePercent) * _tileSize);

                Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randCoordinte.y / (float)_currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(_currentMap.foregroundColor, _currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoordinate.Remove(randCoordinte);
            }
            else
            {
                obstacleMap[randCoordinte.x, randCoordinte.y] = false;
                currentObstacleCount--;
            }
        }
        _allOpenTileCoordinate = new Queue<Coordinate>(Utility.ShuffleArray(allOpenCoordinate.ToArray(), _currentMap.seed));
        // Creating Navmesh Mask
        Transform maskLeft = Instantiate(_navMeshMaskPrefab, Vector3.left * (_currentMap.mapSize.x+ _maxMapSize.x) / 4f * _tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((_maxMapSize.x - _currentMap.mapSize.x)/2f,1, _currentMap.mapSize.y) * _tileSize;

        Transform maskRight = Instantiate(_navMeshMaskPrefab, Vector3.right * (_currentMap.mapSize.x + _maxMapSize.x) / 4f * _tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((_maxMapSize.x - _currentMap.mapSize.x) / 2f, 1, _currentMap.mapSize.y) * _tileSize;

        Transform maskTop = Instantiate(_navMeshMaskPrefab, Vector3.forward * (_currentMap.mapSize.y + _maxMapSize.y) / 4f * _tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(_maxMapSize.x, 1, (_maxMapSize.y - _currentMap.mapSize.y) / 2f) * _tileSize;

        Transform maskBottom = Instantiate(_navMeshMaskPrefab, Vector3.back * (_currentMap.mapSize.y + _maxMapSize.y) / 4f * _tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(_maxMapSize.x, 1, (_maxMapSize.y - _currentMap.mapSize.y) / 2f) * _tileSize;

        _navMeshFloor.localScale = new Vector3(_maxMapSize.x, _maxMapSize.y) * _tileSize;
        _mapFloor.localScale = new Vector3(_currentMap.mapSize.x * _tileSize, _currentMap.mapSize.y * _tileSize);
    }
    bool MapFullyAccessible(bool[,] obstacleMap,int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coordinate> queue = new Queue<Coordinate>();
        queue.Enqueue(_currentMap.mapCenter);

        mapFlags[_currentMap.mapCenter.x, _currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {
            Coordinate tile = queue.Dequeue();
            for(int x = -1; x <= 1; x++)
            {
                for(int y=-1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if(x==0 || y == 0)
                    {
                        if(neighbourX >=0 && neighbourX <obstacleMap.GetLength(0) && neighbourY >=0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coordinate(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(_currentMap.mapSize.x * _currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }
    Vector3 CoorToPosition(int x, int y)
    {
        return new Vector3(-_currentMap.mapSize.x / 2f + 0.5f + x, 0, -_currentMap.mapSize.y / 2f + 0.5f + y) * _tileSize;
    }

    public Coordinate GetRandCoordinate()
    {
        Coordinate randomCoord = _shuffeledTileCoordinate.Dequeue();
        _shuffeledTileCoordinate.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / _tileSize + (_currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / _tileSize + (_currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);
        return _tileMap[x, y];
    }

    public Transform GetRadomOpenTile()
    {
        Coordinate randomCoordinate = _allOpenTileCoordinate.Dequeue();
        _allOpenTileCoordinate.Enqueue(randomCoordinate);
        return _tileMap[randomCoordinate.x, randomCoordinate.y];
    }

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    [System.Serializable]
    public struct Coordinate
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public int x;
        public int y;
        public Coordinate(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public static bool operator ==(Coordinate c1,Coordinate c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coordinate mapSize;
        [Range(0,1)] // Clamp the float range
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coordinate mapCenter
        {
            get
            {
                return new Coordinate(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }
}
   
