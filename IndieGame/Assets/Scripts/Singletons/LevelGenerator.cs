using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private Object[] _cornerPieces;
    private Object[] _edgePieces;
    private Object[] _middlePieces;

    private Object[] _straightRoadPieces;
    private Object[] _crossSectionPieces;
    private Object[] _cornerRoadPieces;
    private Object[] _tSectionPieces;
    private Object[] _deadEndPieces;

    private enum RoadPiece
    {
        StraightRoad,
        CrossSection,
        Corner,
        TSection,
        DeadEnd,
        None
    }

    private enum AvailableDirections
    {
        Left,
        Right,
        Up,
        Down
    }

    private Vector3 _extents;
    private MazeNode[,] _dungeonPieces;
    private int[,] _roadObjects;
    private List<Object> _spawnedPathPieces = new List<Object>();

    private struct MazeNode
    {
        public RoadPiece roadPiece;
        public Vector3 position;
        public List<MazeNode> adjecentNodes;
    }

    // Use this for initialization
    void Start()
    {
        _cornerPieces = Resources.LoadAll("Procedural Level Prefabs/Corner Pieces");
        _edgePieces = Resources.LoadAll("Procedural Level Prefabs/Edge Pieces");
        _middlePieces = Resources.LoadAll("Procedural Level Prefabs/Middle Pieces");

        _straightRoadPieces = Resources.LoadAll("Procedural Level Prefabs/Straight Road Pieces");
        _crossSectionPieces = Resources.LoadAll("Procedural Level Prefabs/Cross Section Pieces");
        _cornerRoadPieces = Resources.LoadAll("Procedural Level Prefabs/Corner Road Pieces");
        _tSectionPieces = Resources.LoadAll("Procedural Level Prefabs/T Section Pieces");
        _deadEndPieces = Resources.LoadAll("Procedural Level Prefabs/Dead End Pieces");

        if (_cornerPieces.Length > 0)
        {
            GameObject tempPiece = ((GameObject)Instantiate(_middlePieces[0]));
            _extents = tempPiece.GetComponentInChildren<BoxCollider>().bounds.extents;
            Destroy(tempPiece);
            Debug.Log(_extents);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateSquareLevel(8, 8);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = _spawnedPathPieces.Count - 1; i >= 0; i--)
            {
                Destroy(_spawnedPathPieces[i]);
                _spawnedPathPieces.RemoveAt(i);
            }
            GenerateDungeonLevel(16, 16);
        }
    }

    public void GenerateSquareLevel(int width = 3, int height = 3)
    {
        //Iterate over width
        for (int i = 0; i < width; i++)
        {
            //Iterate over height
            for (int j = 0; j < height; j++)
            {
                Vector3 piecePosition = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * i, 0, _extents.z * 2 * j);
                Quaternion pieceRotation = transform.rotation;

                //Set rotations
                if (j != 0 && i == 0)
                {
                    pieceRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                }
                else if (j == 0 && i != width - 1)
                {
                    pieceRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                }
                else if (i != 0 && j == height - 1)
                {
                    pieceRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else if (j != height - 1 && i == width - 1)
                {
                    pieceRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else
                {
                    pieceRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 3) * 90, 0));
                }


                //Check if corner piece
                if ((i == 0 || i == width - 1) && (j == 0 || j == height - 1))
                {
                    //Initialize Corner piece
                    Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent);
                }
                //Check if edge piece
                else if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    //Initialize Edge piece
                    Instantiate(_edgePieces[Random.Range(0, _edgePieces.Length)], piecePosition, pieceRotation, transform.parent);
                }
                //Else is middle
                else
                {
                    //Initialize Middle piece
                    Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent);
                }
            }
        }
    }

    public void GenerateDungeonLevel(int width, int height)
    {
        _dungeonPieces = new MazeNode[width, height];
        _roadObjects = new int[width, height];
        Vector2Int _startNode;
        Vector2Int _endNode;
        Vector2Int _currentNode;

        //Random start node
        _startNode = new Vector2Int(Random.Range(0, width - 1), 0);
        //Set current node to start node
        _currentNode = _startNode;
        //Set end node
        _endNode = new Vector2Int(Random.Range(0, width - 1), height - 1);

        //Iterate over width
        for (int i = 0; i < width; i++)
        {
            //Iterate over height
            for (int j = 0; j < height; j++)
            {
                Vector3 piecePosition = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * i, 0, _extents.z * 2 * j);
                Quaternion pieceRotation = transform.rotation;

                //Create all the nodes
                _dungeonPieces[i, j] = new MazeNode { roadPiece = RoadPiece.None, adjecentNodes = new List<MazeNode>(), position = piecePosition };
            }
        }

        //Iterate over node width
        for (int i = 0; i < _dungeonPieces.GetLength(0); i++)
        {
            //Iterate over node height
            for (int j = 0; j < _dungeonPieces.GetLength(1); j++)
            {
                AddAdjecents(i, j, _dungeonPieces);
            }
        }

        //Generate main path
        Vector2Int nextNode = new Vector2Int(0, 0);
        int pathCount = 1;
        while (_currentNode.x != _endNode.x || _currentNode.y != _endNode.y)
        {
            nextNode = PathSegmentEnd(_currentNode, _endNode, width, height, height / 3, false);
            if (nextNode.x == -1 && nextNode.y == -1) { break; }
            int stepAmount = Mathf.Abs(_currentNode.x - nextNode.x + _currentNode.y - nextNode.y);

            for (int j = 0; j < stepAmount; j++)
            {
                //Set the roadpiece enum
                _dungeonPieces[_currentNode.x, _currentNode.y].roadPiece = RoadPiece.StraightRoad;

                //Instantiate object
                _roadObjects[_currentNode.x, _currentNode.y] = pathCount;
                pathCount++;

                if (_currentNode.x > nextNode.x)
                {
                    _currentNode.x--;
                }
                if (_currentNode.x < nextNode.x)
                {
                    _currentNode.x++;
                }
                if (_currentNode.y > nextNode.y)
                {
                    _currentNode.y--;
                }
                if (_currentNode.y < nextNode.y)
                {
                    _currentNode.y++;
                }
            }

            //Set the roadpiece enum
            _dungeonPieces[_currentNode.x, _currentNode.y].roadPiece = RoadPiece.Corner;

            //Instantiate object
            _roadObjects[_currentNode.x, _currentNode.y] = pathCount;
        }

        GenerateSidePaths();
        GenerateMainPath();

        //TEMP
        Vector3 piecePosition1 = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _startNode.x, 0, _extents.z * 2 * _startNode.y);
        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition1, Quaternion.Euler(0, 45, 0), transform.parent));
        piecePosition1 = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _endNode.x, 0, _extents.z * 2 * _endNode.y);
        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition1, Quaternion.Euler(0, 45, 0), transform.parent));
    }

    private void GenerateSidePaths()
    {
        int i = 2;
        int pathCount = 1;

        Vector3 piecePosition;
        Quaternion pieceRotation;

        Vector2Int _currentPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
        Vector2Int _nextPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
        Vector2Int _previousPos = _currentPos;

        while (i != -1)
        {
            _previousPos = _currentPos;
            _currentPos = _nextPos;
            _nextPos = new Vector2Int((int)FindValueInArray(i, _roadObjects).x, (int)FindValueInArray(i, _roadObjects).y);

            if (i % 3 == 0)
            {
                int rand = Random.Range(0, 1);
                Vector2Int endOfPath = _currentPos;
                Vector2Int currentPosition = _currentPos;
                bool endOfPathReached = false;

                if (_previousPos.x == _nextPos.x)
                {
                    switch (rand)
                    {
                        case 0:
                            while (!endOfPathReached)
                            {
                                Vector2Int endOfPathCopy = endOfPath;
                                switch (Random.Range(1, 3))
                                {
                                    case 1:
                                        endOfPath.x++;
                                        break;
                                    case 2:
                                        endOfPath.y++;
                                        break;
                                    case 3:
                                        endOfPath.y--;
                                        break;
                                }
                                try
                                {
                                    if (endOfPath.x >= _roadObjects.GetLength(0) || _roadObjects[endOfPath.x, endOfPath.y] > 0)
                                    {
                                        endOfPathReached = true;
                                        endOfPath = endOfPathCopy;
                                    }
                                }
                                catch
                                {
                                    endOfPathReached = true;
                                    endOfPath = endOfPathCopy;
                                }
                            }
                            break;
                        case 1:
                            while (!endOfPathReached)
                            {
                                Vector2Int endOfPathCopy = endOfPath;
                                switch (Random.Range(1, 3))
                                {
                                    case 1:
                                        endOfPath.x--;
                                        break;
                                    case 2:
                                        endOfPath.y++;
                                        break;
                                    case 3:
                                        endOfPath.y--;
                                        break;
                                }
                                try
                                {
                                    if (endOfPath.x <= 0 || _roadObjects[endOfPath.x, endOfPath.y] > 0)
                                    {
                                        endOfPathReached = true;
                                        endOfPath = endOfPathCopy;
                                    }
                                }
                                catch
                                {
                                    endOfPathReached = true;
                                    endOfPath = endOfPathCopy;
                                }
                            }
                            break;
                    }
                }
                if (_previousPos.y == _nextPos.y)
                {
                    switch (rand)
                    {
                        case 0:
                            while (!endOfPathReached)
                            {
                                Vector2Int endOfPathCopy = endOfPath;
                                switch (Random.Range(1, 3))
                                {
                                    case 1:
                                        endOfPath.y++;
                                        break;
                                    case 2:
                                        endOfPath.x++;
                                        break;
                                    case 3:
                                        endOfPath.x--;
                                        break;
                                }
                                try
                                {
                                    if (endOfPath.y >= _roadObjects.GetLength(1) || _roadObjects[endOfPath.x, endOfPath.y] > 0)
                                    {
                                        endOfPathReached = true;
                                        endOfPath = endOfPathCopy;
                                    }
                                }
                                catch
                                {
                                    endOfPathReached = true;
                                    endOfPath = endOfPathCopy;
                                }
                            }
                            break;
                        case 1:
                            while (!endOfPathReached)
                            {
                                Vector2Int endOfPathCopy = endOfPath;
                                switch (Random.Range(1, 3))
                                {
                                    case 1:
                                        endOfPath.y--;
                                        break;
                                    case 2:
                                        endOfPath.x++;
                                        break;
                                    case 3:
                                        endOfPath.x--;
                                        break;
                                }
                                try
                                {
                                    if (endOfPath.y <= 0 || _roadObjects[endOfPath.x, endOfPath.y] > 0)
                                    {
                                        endOfPathReached = true;
                                        endOfPath = endOfPathCopy;
                                    }
                                }
                                catch
                                {
                                    endOfPathReached = true;
                                    endOfPath = endOfPathCopy;
                                }
                            }
                            break;
                    }
                }

                while (currentPosition.x != endOfPath.x || currentPosition.y != endOfPath.y)
                {
                    Vector2Int nextNode = PathSegmentEnd(currentPosition, endOfPath, _roadObjects.GetLength(0), _roadObjects.GetLength(1), 3, true);
                    if (nextNode.x == -1 && nextNode.y == -1) { break; }
                    int stepAmount = Mathf.Abs(currentPosition.x - nextNode.x + currentPosition.y - nextNode.y);

                    piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * currentPosition.x, 0, _extents.z * 2 * currentPosition.y);
                    pieceRotation = transform.rotation;

                    for (int j = 0; j < stepAmount; j++)
                    {
                        //Set the roadpiece enum
                        if (_dungeonPieces[currentPosition.x, currentPosition.y].roadPiece == RoadPiece.None)
                        {
                            //pieceRotation = Quaternion.Euler(0, 270, 0);
                            //_spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                            //_dungeonPieces[currentPosition.x, currentPosition.y].roadPiece = RoadPiece.StraightRoad;
                            _roadObjects[currentPosition.x, currentPosition.y] = 1000 + pathCount;
                            _dungeonPieces[currentPosition.x, currentPosition.y].roadPiece = RoadPiece.StraightRoad;
                            pathCount++;
                        }

                        if (currentPosition.x > nextNode.x)
                        {
                            currentPosition.x--;
                        }
                        if (currentPosition.x < nextNode.x)
                        {
                            currentPosition.x++;
                        }
                        if (currentPosition.y > nextNode.y)
                        {
                            currentPosition.y--;
                        }
                        if (currentPosition.y < nextNode.y)
                        {
                            currentPosition.y++;
                        }

                        piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * currentPosition.x, 0, _extents.z * 2 * currentPosition.y);
                        pieceRotation = transform.rotation;
                    }

                    if (currentPosition.x < _dungeonPieces.GetLength(0) && currentPosition.y < _dungeonPieces.GetLength(1))
                    {
                        if (_dungeonPieces[currentPosition.x, currentPosition.y].roadPiece == RoadPiece.None)
                        {
                            //Set the roadpiece enum
                            _roadObjects[currentPosition.x, currentPosition.y] = 1000 + pathCount;
                            _dungeonPieces[currentPosition.x, currentPosition.y].roadPiece = RoadPiece.StraightRoad;
                            pathCount++;
                        }
                    }
                }
            }

            i++;
            if (_currentPos.x == -1)
            {
                i = -1;
                return;
            }
        }
    }

    private void GenerateMainPath()
    {
        int i = 1001;
        //int i = 2;

        Vector3 piecePosition;
        Quaternion pieceRotation;

        Vector2Int _currentPos = new Vector2Int((int)FindValueInArray(1001, _roadObjects).x, (int)FindValueInArray(1001, _roadObjects).y);
        Vector2Int _nextPos = new Vector2Int((int)FindValueInArray(1001, _roadObjects).x, (int)FindValueInArray(1001, _roadObjects).y);
        Vector2Int _previousPos = _currentPos;

        Debug.Log(Mathf.RoundToInt(90.0000000001f) == 90);

        bool checkedSidePaths = false;

        while (i != -1)
        {
            _previousPos = _currentPos;
            _currentPos = _nextPos;
            _nextPos = new Vector2Int((int)FindValueInArray(i, _roadObjects).x, (int)FindValueInArray(i, _roadObjects).y);

            piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _currentPos.x, 0, _extents.z * 2 * _currentPos.y);
            pieceRotation = transform.rotation;

            Object obj1 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(_extents.x * 2, 0, 0));
            Object obj2 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(_extents.x * -2, 0, 0));
            Object obj3 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(0, 0, _extents.z * 2));
            Object obj4 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(0, 0, _extents.z * -2));


            if ((_currentPos.x != _previousPos.x && _currentPos.x != _nextPos.x && (_roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y - 1, 0, _roadObjects.GetLength(1) - 1)] > 1000 || _roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y + 1, 0, _roadObjects.GetLength(1) - 1)] > 1000) && _roadObjects[_currentPos.x, _currentPos.y] < 1000)
               && (obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0 || obj4 != null && Mathf.RoundToInt(((GameObject)obj4).transform.rotation.eulerAngles.y) == 0))
            {
                if (obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0)
                {
                    pieceRotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 270, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if ((_currentPos.x == _previousPos.x && _currentPos.x == _nextPos.x && (_roadObjects[(int)Mathf.Clamp(_currentPos.x - 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000 || _roadObjects[(int)Mathf.Clamp(_currentPos.x + 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000) && _roadObjects[_currentPos.x, _currentPos.y] < 1000)
                && (obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 90 || obj2 != null && Mathf.RoundToInt(((GameObject)obj2).transform.rotation.eulerAngles.y) == 90))
            {
                if (obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 90)
                {
                    pieceRotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 0, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if ((_currentPos.x == _previousPos.x && _currentPos.x != _nextPos.x && (_roadObjects[(int)Mathf.Clamp(_currentPos.x - 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000 || _roadObjects[(int)Mathf.Clamp(_currentPos.x + 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000) && _roadObjects[_currentPos.x, _currentPos.y] < 1000)
                && (((obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 90) || (obj4 != null && Mathf.RoundToInt(((GameObject)obj4).transform.rotation.eulerAngles.y) == 90))))
            {
                if (obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 90)
                {
                    pieceRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 270, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if ((_currentPos.y == _previousPos.y && _currentPos.y != _nextPos.y && (_roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y - 1, 0, _roadObjects.GetLength(1) - 1)] > 1000 || _roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y + 1, 0, _roadObjects.GetLength(1) - 1)] > 1000) && _roadObjects[_currentPos.x, _currentPos.y] < 1000)
                && ((obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 0) || (obj2 != null && Mathf.RoundToInt(((GameObject)obj2).transform.rotation.eulerAngles.y) == 0)))
            {
                if (obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 0)
                {
                    pieceRotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 90, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_currentPos.x != _previousPos.x && _currentPos.x != _nextPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 90, 0);
                _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_currentPos.x == _previousPos.x && _currentPos.x == _nextPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 0, 0);
                _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.y < _currentPos.y && _nextPos.x < _currentPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 0, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.y < _currentPos.y && _nextPos.x > _currentPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 270, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.x < _currentPos.x && _nextPos.y > _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 90, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.x < _currentPos.x && _nextPos.y < _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 180, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.x > _currentPos.x && _nextPos.y > _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 180, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            else if (_previousPos.x > _currentPos.x && _nextPos.y < _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 270, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
            }

            if (_currentPos.x == -1)
            {
                if (checkedSidePaths)
                {
                    return;
                }
                else
                {
                    checkedSidePaths = true;
                    _currentPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
                    _nextPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
                    _previousPos = _currentPos;
                    i = 1;
                    //i = 1001;
                }
            }
            i++;
        }
    }


    private void AddAdjecents(int x, int y, MazeNode[,] nodes)
    {
        nodes[x, y].adjecentNodes = new List<MazeNode>();
        if (x - 1 >= 0)
        {
            nodes[x, y].adjecentNodes.Add(nodes[x - 1, y]);
        }
        if (x + 1 < nodes.GetLength(0))
        {
            nodes[x, y].adjecentNodes.Add(nodes[x + 1, y]);
        }
        if (y - 1 > 0)
        {
            nodes[x, y].adjecentNodes.Add(nodes[x, y - 1]);
        }
        if (y + 1 < nodes.GetLength(1))
        {
            nodes[x, y].adjecentNodes.Add(nodes[x, y + 1]);
        }
    }

    private List<AvailableDirections> ReturnPathDirections(int startX, int startY, int endX, int endY, MazeNode[,] nodes)
    {
        List<AvailableDirections> returnList = new List<AvailableDirections>();

        try
        {
            //Check horizontal directinos
            if (startX <= endX)
            {
                if (nodes[startX, startY].adjecentNodes.Exists(node => node.position.x > nodes[startX, startY].position.x && nodes[startX + 1, startY].roadPiece == RoadPiece.None))
                    returnList.Add(AvailableDirections.Right);
            }
            if (startX >= endX)
            {
                if (nodes[startX, startY].adjecentNodes.Exists(node => node.position.x < nodes[startX, startY].position.x && nodes[startX - 1, startY].roadPiece == RoadPiece.None))
                    returnList.Add(AvailableDirections.Left);
            }

            //Check vertical directinos
            if (startY <= endY)
            {
                if (nodes[startX, startY].adjecentNodes.Exists(node => node.position.z > nodes[startX, startY].position.z && nodes[startX, startY + 1].roadPiece == RoadPiece.None))
                    returnList.Add(AvailableDirections.Up);
            }
            if (startY > endY)
            {
                if (nodes[startX, startY].adjecentNodes.Exists(node => node.position.z < nodes[startX, startY].position.z && nodes[startX, startY - 1].roadPiece == RoadPiece.None))
                    returnList.Add(AvailableDirections.Down);
            }
        }
        catch
        {
            return null;
        }

        if (returnList.Count == 0)
        {
            return null;
        }

        return returnList;
    }

    private Vector2Int PathSegmentEnd(Vector2Int currentNode, Vector2Int endNode, int width, int height, int segmentLengthCap, bool moreStraightLines)
    {
        List<AvailableDirections> directions = ReturnPathDirections(currentNode.x, currentNode.y, endNode.x, endNode.y, _dungeonPieces);
        if (directions == null)
        {
            return new Vector2Int(-1, -1);
        }
        int i = 1;
        if (moreStraightLines)
        {
            i = 2;
        }
        int rand = 0;
        Vector2Int returnValue = Vector2Int.zero;
        switch (directions[Random.Range(0, directions.Count - 1)])
        {
            case AvailableDirections.Up:
                rand = Random.Range(currentNode.y + i, height - 1);
                rand = (int)Mathf.Clamp(rand, 0, currentNode.y + segmentLengthCap);
                returnValue = new Vector2Int(currentNode.x, rand);
                break;
            case AvailableDirections.Right:
                rand = Random.Range(currentNode.x + i, width - 1);
                rand = (int)Mathf.Clamp(rand, 0, currentNode.x + segmentLengthCap);
                returnValue = new Vector2Int(rand, currentNode.y);
                break;
            case AvailableDirections.Down:
                rand = Random.Range(0, currentNode.y - i);
                rand = (int)Mathf.Clamp(rand, 0, currentNode.y + segmentLengthCap);
                returnValue = new Vector2Int(currentNode.x, rand);
                break;
            case AvailableDirections.Left:
                rand = Random.Range(0, currentNode.x - i);
                rand = (int)Mathf.Clamp(rand, 0, currentNode.x + segmentLengthCap);
                returnValue = new Vector2Int(rand, currentNode.y);
                break;
        }
        if (moreStraightLines)
        {
            if (currentNode.x == endNode.x && (currentNode.y - endNode.y < segmentLengthCap || endNode.y - currentNode.y < segmentLengthCap))
            {
                return endNode;
            }
            else if (currentNode.y == endNode.y && (currentNode.x - endNode.x < segmentLengthCap || endNode.x - currentNode.x < segmentLengthCap))
            {
                return endNode;
            }
        }
        return returnValue;
    }

    private Vector2 FindValueInArray(int value, int[,] array)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (array[i, j] == value)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return new Vector2(-1, -1);
    }
}
