using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _questItems = new List<GameObject>();

    private Object[] _cornerPieces;
    private Object[] _edgePieces;
    private Object[] _middlePieces;

    private Object[] _straightRoadPieces;
    private Object[] _crossSectionPieces;
    private Object[] _cornerRoadPieces;
    private Object[] _tSectionPieces;
    private Object[] _deadEndPieces;
    private Object[] _noRoadPieces;

    private Object[] _startEndPieces;

    Vector2Int _startNode;
    Vector2Int _endNode;

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

    public Vector3 playerSpawnPosition = Vector3.zero;

    public delegate void DoneWithGenerating();
    public DoneWithGenerating doneWithGenerating;

    private struct MazeNode
    {
        public RoadPiece roadPiece;
        public Vector3 position;
        public List<MazeNode> adjecentNodes;
    }

    private void Awake()
    {
        GameController.levelGenerator = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (_endNode != null && GameController.player != null)
            {
                Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x - 5, -2.75f, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _endNode.x, 0, _extents.z * 2 * _endNode.y);
                GameController.player.transform.position = piecePosition;
            }
        }
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
        _noRoadPieces = Resources.LoadAll("Procedural Level Prefabs/No Road Pieces");

        _startEndPieces = Resources.LoadAll("Procedural Level Prefabs/EndStart Pieces");

        if (_cornerPieces.Length > 0)
        {
            GameObject tempPiece = ((GameObject)Instantiate(_cornerPieces[0]));
            _extents = tempPiece.GetComponentInChildren<BoxCollider>().bounds.extents;
            Destroy(tempPiece);
        }
    }

    public void GenerateFullDungeonLevel(int width, int height)
    {
        //Level Generation
        System.DateTime startGeneratingLevel = System.DateTime.Now;
        StartGeneratingLevel(width, height);
        Debug.Log("Level generation time: " + (System.DateTime.Now - startGeneratingLevel));
        //Main path
        System.DateTime startGeneratingMainPath = System.DateTime.Now;
        GenerateMainPath();
        Debug.Log("Main path generation time: " + (System.DateTime.Now - startGeneratingMainPath));
        //Fill in left over area
        System.DateTime startFillingInLeftOverArea = System.DateTime.Now;
        GenerateLeftOverArea(_startNode, _endNode);
        Debug.Log("Filling in left over area time: " + (System.DateTime.Now - startFillingInLeftOverArea));
        System.DateTime startGeneratingBorder = System.DateTime.Now;
        GenerateBorderAroundLevel();
        Debug.Log("Border around level generation time: " + (System.DateTime.Now - startGeneratingBorder));
        GenerateQuestItems();
        Debug.Log("Total Time: " + (System.DateTime.Now - startGeneratingLevel) + " / " + (System.DateTime.Now - startGeneratingLevel).TotalMilliseconds + "ms");
        doneWithGenerating();
    }

    private void StartGeneratingLevel(int width, int height)
    {
        //for (int i = _spawnedPathPieces.Count - 1; i >= 0; i--)
        //{
        //    Destroy(_spawnedPathPieces[i]);
        //    _spawnedPathPieces.RemoveAt(i);
        //}
        GenerateDungeonLevel(width, width);
    }

    public void DestroyOldLevel()
    {
        for (int i = _spawnedPathPieces.Count - 1; i >= 0; i--)
        {
            Destroy(_spawnedPathPieces[i]);
            _spawnedPathPieces.RemoveAt(i);
        }
    }

    private void GenerateQuestItems()
    {
        if (OlChapBehaviour.GetQuestProgression() == 6)
        {
            Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, -2.75f, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _endNode.x, 0, _extents.z * 2 * _endNode.y);
            Instantiate(_questItems[0], piecePosition + -transform.forward * 2, Quaternion.Euler(0, 0, 0), this.transform);
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

    private void GenerateDungeonLevel(int width, int height)
    {
        _dungeonPieces = new MazeNode[width, height];
        _roadObjects = new int[width, height];
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
        DoubleCheckPieces();
    }

    private void GenerateSidePaths()
    {
        int i = 1;
        int pathCount = 1;

        Vector3 piecePosition;
        Quaternion pieceRotation;

        Vector2Int _currentPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
        Vector2Int _nextPos = new Vector2Int((int)FindValueInArray(2, _roadObjects).x, (int)FindValueInArray(2, _roadObjects).y);
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

                piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 10, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _currentPos.x, 0, _extents.z * 2 * _currentPos.y);

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
                            _roadObjects[currentPosition.x, currentPosition.y] = 1000 + pathCount;
                            _dungeonPieces[currentPosition.x, currentPosition.y].roadPiece = RoadPiece.StraightRoad;
                            AddAdjecents(currentPosition.x, currentPosition.y, _dungeonPieces);
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
                            AddAdjecents(currentPosition.x, currentPosition.y, _dungeonPieces);
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
        int i = 1;

        Vector3 piecePosition;
        Quaternion pieceRotation;

        Vector2Int _currentPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
        Vector2Int _nextPos = new Vector2Int((int)FindValueInArray(1, _roadObjects).x, (int)FindValueInArray(1, _roadObjects).y);
        Vector2Int _previousPos = _currentPos;

        if (_currentPos.y - 1 >= 0 &&
            _roadObjects[_currentPos.x, _currentPos.y - 1] > 0 && _roadObjects[_currentPos.x, _currentPos.y - 1] < 1000)
        {
            _previousPos = new Vector2Int(_currentPos.x, _currentPos.y - 1);
        }
        else if (_currentPos.y + 1 < _roadObjects.GetLength(1) &&
            _roadObjects[_currentPos.x, _currentPos.y + 1] > 0 && _roadObjects[_currentPos.x, _currentPos.y + 1] < 1000)
        {
            _previousPos = new Vector2Int(_currentPos.x, _currentPos.y + 1);
        }
        else if (_currentPos.x - 1 >= 0 &&
            _roadObjects[_currentPos.x - 1, _currentPos.y] > 0 && _roadObjects[_currentPos.x - 1, _currentPos.y] < 1000)
        {
            _previousPos = new Vector2Int(_currentPos.x - 1, _currentPos.y);
        }
        else if (_currentPos.x + 1 < _roadObjects.GetLength(0) &&
            _roadObjects[_currentPos.x + 1, _currentPos.y] > 0 && _roadObjects[_currentPos.x + 1, _currentPos.y] < 1000)
        {
            _previousPos = new Vector2Int(_currentPos.x + 1, _currentPos.y);
        }

        while (i != -1)
        {
            _previousPos = _currentPos;
            _currentPos = _nextPos;
            _nextPos = new Vector2Int((int)FindValueInArray(i, _roadObjects).x, (int)FindValueInArray(i, _roadObjects).y);

            piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _currentPos.x, 0, _extents.z * 2 * _currentPos.y);
            pieceRotation = transform.rotation;

            //Side paths next and previous position might be wrong since there are multiple paths and not jsut one
            if (((_currentPos.x != -1 && _currentPos.y != -1) && ((_currentPos - _previousPos).magnitude > 1)) && i > 1000)
            {
                //Fix nextpos
                if ((_nextPos.x != _currentPos.x && _nextPos.y != _currentPos.y - 1)
                && (_nextPos.x != _currentPos.x && _nextPos.y != _currentPos.y + 1)
                && (_nextPos.x != _currentPos.x - 1 && _nextPos.y != _currentPos.y)
                && (_nextPos.x != _currentPos.x + 1 && _nextPos.y != _currentPos.y)
                && _nextPos.x != -1 && _nextPos.y != -1)
                {
                    _nextPos = _currentPos;
                }
                //Fix previouspos
                if ((_previousPos.x != _currentPos.x && _previousPos.y != _currentPos.y - 1)
                && (_previousPos.x != _currentPos.x && _previousPos.y != _currentPos.y + 1)
                && (_previousPos.x != _currentPos.x - 1 && _previousPos.y != _currentPos.y)
                && (_previousPos.x != _currentPos.x + 1 && _previousPos.y != _currentPos.y)
                && _previousPos.x != -1 && _previousPos.y != -1)
                {
                    if (_dungeonPieces[_currentPos.x, _currentPos.y].adjecentNodes.Exists(node => node.roadPiece != RoadPiece.None))
                    {
                        MazeNode adjecent = _dungeonPieces[_currentPos.x, _currentPos.y].adjecentNodes.Find(node => node.roadPiece != RoadPiece.None);
                        for (int width = 0; width < _dungeonPieces.GetLength(0); width++)
                        {
                            for (int height = 0; height < _dungeonPieces.GetLength(1); height++)
                            {
                                if (_dungeonPieces[width, height].position == adjecent.position)
                                {
                                    _previousPos = new Vector2Int(width, height);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        _previousPos = _currentPos;
                    }
                }
            }



            Object obj1 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(_extents.x * 2, 0, 0));
            Object obj2 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(_extents.x * -2, 0, 0));
            Object obj3 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(0, 0, _extents.z * 2));
            Object obj4 = _spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == piecePosition + new Vector3(0, 0, _extents.z * -2));

            //Tsections
            if ((_currentPos.x != _previousPos.x && _currentPos.x != _nextPos.x && (_roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y - 1, 0, _roadObjects.GetLength(1) - 1)] > 1000 || _roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y + 1, 0, _roadObjects.GetLength(1) - 1)] > 1000))
               && (IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(0, 1)) || IsPieceFacingNode(_currentPos, _currentPos - new Vector2Int(0, 1))))
            {
                if (IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(0, 1)))
                {
                    pieceRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 180, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.TSection;
            }
            else if ((_currentPos.x == _previousPos.x && _currentPos.x == _nextPos.x && (_roadObjects[(int)Mathf.Clamp(_currentPos.x - 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000 || _roadObjects[(int)Mathf.Clamp(_currentPos.x + 1, 0, _roadObjects.GetLength(1) - 1), _currentPos.y] > 1000))
                && (IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(1, 0)) || IsPieceFacingNode(_currentPos, _currentPos - new Vector2Int(1, 0))))
            {
                if (IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(1, 0)))
                {
                    pieceRotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, -90, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.TSection;
            }
            else if ((_currentPos.x == _previousPos.x && _currentPos.x != _nextPos.x && (_roadObjects[(int)Mathf.Clamp(_currentPos.x - 1, 0, _roadObjects.GetLength(0) - 1), _currentPos.y] > 1000 || _roadObjects[(int)Mathf.Clamp(_currentPos.x + 1, 0, _roadObjects.GetLength(0) - 1), _currentPos.y] > 1000))
                && ((obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 90 && _dungeonPieces[_currentPos.x + 1, _currentPos.y].roadPiece != RoadPiece.Corner) || (obj2 != null && Mathf.RoundToInt(((GameObject)obj2).transform.rotation.eulerAngles.y) == 90 && _dungeonPieces[_currentPos.x - 1, _currentPos.y].roadPiece != RoadPiece.Corner)
                || (obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0 && _dungeonPieces[_currentPos.x, _currentPos.y + 1].roadPiece != RoadPiece.Corner)))
            {
                if (obj1 != null && Mathf.RoundToInt(((GameObject)obj1).transform.rotation.eulerAngles.y) == 90 && _dungeonPieces[_currentPos.x + 1, _currentPos.y].roadPiece != RoadPiece.Corner)
                {
                    pieceRotation = Quaternion.Euler(0, 180, 0);
                }
                else if ((obj2 != null && Mathf.RoundToInt(((GameObject)obj2).transform.rotation.eulerAngles.y) == 90 && _dungeonPieces[_currentPos.x - 1, _currentPos.y].roadPiece != RoadPiece.Corner))
                {
                    pieceRotation = Quaternion.Euler(0, 180, 0);
                }
                else if ((obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0 && _dungeonPieces[_currentPos.x, _currentPos.y + 1].roadPiece != RoadPiece.Corner) && _nextPos.x > _currentPos.x)
                {
                    pieceRotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, -90, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.TSection;
            }
            else if ((_currentPos.y == _previousPos.y && _currentPos.y != _nextPos.y && (_roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y - 1, 0, _roadObjects.GetLength(1) - 1)] > 1000 || _roadObjects[_currentPos.x, (int)Mathf.Clamp(_currentPos.y + 1, 0, _roadObjects.GetLength(1) - 1)] > 1000))
                && ((obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0 && _dungeonPieces[_currentPos.x, _currentPos.y + 1].roadPiece != RoadPiece.Corner) || (obj4 != null && Mathf.RoundToInt(((GameObject)obj4).transform.rotation.eulerAngles.y) == 0 && _dungeonPieces[_currentPos.x, _currentPos.y - 1].roadPiece != RoadPiece.Corner)))
            {
                if (obj3 != null && Mathf.RoundToInt(((GameObject)obj3).transform.rotation.eulerAngles.y) == 0 && _dungeonPieces[_currentPos.x, _currentPos.y + 1].roadPiece != RoadPiece.Corner)
                {
                    pieceRotation = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    pieceRotation = Quaternion.Euler(0, 0, 0);
                }
                _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.TSection;
            }
            //Straight path pieces
            else if (_currentPos.x != _previousPos.x && _currentPos.x != _nextPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 0, 0);
                _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.StraightRoad;
            }
            else if (_currentPos.x == _previousPos.x && _currentPos.x == _nextPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 90, 0);
                _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.StraightRoad;
            }
            //Corners
            else if (_previousPos.y < _currentPos.y && _nextPos.x < _currentPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 0, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }
            else if (_previousPos.y < _currentPos.y && _nextPos.x > _currentPos.x)
            {
                pieceRotation = Quaternion.Euler(0, 270, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }
            else if (_previousPos.x < _currentPos.x && _nextPos.y > _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 90, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }
            else if (_previousPos.x < _currentPos.x && _nextPos.y < _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 180, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }
            else if (_previousPos.x > _currentPos.x && _nextPos.y > _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 180, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }
            else if (_previousPos.x > _currentPos.x && _nextPos.y < _currentPos.y)
            {
                pieceRotation = Quaternion.Euler(0, 270, 0);
                _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.Corner;
            }

            _nextPos = new Vector2Int((int)FindValueInArray(i, _roadObjects).x, (int)FindValueInArray(i, _roadObjects).y);

            if (_currentPos.x == -1)
            {
                MakeIntersections();
                return;
            }
            i++;
        }
    }

    private void DoubleCheckPieces()
    {
        for (int i = 0; i < _roadObjects.GetLength(0); i++)
        {
            for (int j = 0; j < _roadObjects.GetLength(1); j++)
            {
                if (_roadObjects[i, j] > 1000)
                {
                    Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * i, 0, _extents.z * 2 * j);
                    Quaternion pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);

                    bool up = false;
                    bool right = false;
                    bool down = false;
                    bool left = false;

                    if (j + 1 < _roadObjects.GetLength(1) && _roadObjects[i, j + 1] > 0) { up = true; }
                    if (i + 1 < _roadObjects.GetLength(0) && _roadObjects[i + 1, j] > 0) { right = true; }
                    if (j - 1 >= 0 && _roadObjects[i, j - 1] > 0) { down = true; }
                    if (i - 1 >= 0 && _roadObjects[i - 1, j] > 0) { left = true; }

                    //Intersection
                    if (up && right && left && down)
                    {
                        _spawnedPathPieces.Add(Instantiate(_crossSectionPieces[Random.Range(0, _crossSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.CrossSection;
                    }
                    //T sections
                    else if (up && down && left)
                    {
                        pieceRotation = Quaternion.Euler(0, -90, 0);
                        _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.TSection;
                    }
                    else if (up && down && right)
                    {
                        pieceRotation = Quaternion.Euler(0, 90, 0);
                        _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.TSection;
                    }
                    else if (right && left && up)
                    {
                        pieceRotation = Quaternion.Euler(0, 0, 0);
                        _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.TSection;
                    }
                    else if (right && left && down)
                    {
                        pieceRotation = Quaternion.Euler(0, 180, 0);
                        _spawnedPathPieces.Add(Instantiate(_tSectionPieces[Random.Range(0, _tSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.TSection;
                    }
                    //Corners
                    else if (down && left)
                    {
                        pieceRotation = Quaternion.Euler(0, 0, 0);
                        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.Corner;
                    }
                    else if (down && right)
                    {
                        pieceRotation = Quaternion.Euler(0, 270, 0);
                        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.Corner;
                    }
                    else if (up && left)
                    {
                        pieceRotation = Quaternion.Euler(0, 90, 0);
                        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.Corner;
                    }
                    else if (up && right)
                    {
                        pieceRotation = Quaternion.Euler(0, 180, 0);
                        _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.Corner;
                    }
                    //Straight path
                    else if (up && down)
                    {
                        pieceRotation = Quaternion.Euler(0, 90, 0);
                        _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.StraightRoad;
                    }
                    else if (right && left)
                    {
                        pieceRotation = Quaternion.Euler(0, 0, 0);
                        _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.StraightRoad;
                    }
                    //Dead ends
                    else if (up || down || right || left)
                    {
                        if (up)
                        {
                            pieceRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (right)
                        {
                            pieceRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (down)
                        {
                            pieceRotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (left)
                        {
                            pieceRotation = Quaternion.Euler(0, 270, 0);
                        }
                        _spawnedPathPieces.Add(Instantiate(_deadEndPieces[Random.Range(0, _deadEndPieces.Length)], piecePosition, pieceRotation, transform.parent));
                        _dungeonPieces[i, j].roadPiece = RoadPiece.DeadEnd;
                    }
                }
            }
        }
    }


    private void GenerateLeftOverArea(Vector2Int startNode, Vector2Int endNode)
    {
        Quaternion pieceRotation = Quaternion.Euler(0, 0, 0);
        Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * startNode.x, 0, _extents.z * 2 * startNode.y);
        //Spawn start node
        Destroy(((GameObject)_spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == _dungeonPieces[startNode.x, startNode.y].position)).gameObject);
        _spawnedPathPieces.Add(Instantiate(_startEndPieces[1], piecePosition, pieceRotation, transform.parent));
        _dungeonPieces[startNode.x, startNode.y].roadPiece = RoadPiece.CrossSection;
        playerSpawnPosition = piecePosition + transform.up * GameController.player.GetComponent<Collider>().bounds.extents.y;
        //Spawn end node
        piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * endNode.x, 0, _extents.z * 2 * endNode.y);
        Destroy(((GameObject)_spawnedPathPieces.Find(obj => ((GameObject)obj).transform.position == _dungeonPieces[endNode.x, endNode.y].position)).gameObject);
        _spawnedPathPieces.Add(Instantiate(_startEndPieces[0], piecePosition, pieceRotation, transform.parent));
        _dungeonPieces[endNode.x, endNode.y].roadPiece = RoadPiece.CrossSection;

        for (int i = 0; i < _roadObjects.GetLength(0); i++)
        {
            for (int j = 0; j < _roadObjects.GetLength(1); j++)
            {
                Vector2Int _currentPos = new Vector2Int(i, j);
                piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _currentPos.x, 0, _extents.z * 2 * _currentPos.y);
                pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);

                if (_spawnedPathPieces.Exists(obj => ((GameObject)obj).transform.position == _dungeonPieces[i, j].position))
                {
                    if (_dungeonPieces[i, j].roadPiece == RoadPiece.None)
                    {
                        _spawnedPathPieces.Add(Instantiate(_noRoadPieces[Random.Range(0, _noRoadPieces.Length)], piecePosition, pieceRotation, transform.parent));
                    }
                }
                else
                {
                    _dungeonPieces[i, j].roadPiece = RoadPiece.None;
                    _spawnedPathPieces.Add(Instantiate(_noRoadPieces[Random.Range(0, _noRoadPieces.Length)], piecePosition, pieceRotation, transform.parent));
                }
            }
        }
    }

    private void GenerateBorderAroundLevel()
    {
        Vector3 piecePosition = transform.position;
        Quaternion pieceRotation = Quaternion.Euler(0, 0, 0);

        int maxWidth = _roadObjects.GetLength(0);
        int maxHeight = _roadObjects.GetLength(1);

        int amountOfLayers = 3;
        for (int i = 0; i < amountOfLayers; i++)
        {
            for (int w = 0; w < maxWidth + amountOfLayers; w++)
            {
                pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);
                piecePosition = transform.position - new Vector3(_extents.x * maxWidth - _extents.x, 0, _extents.z * maxHeight - _extents.z) + new Vector3(_extents.x * 2 * w, 0, _extents.z * 2 * (maxHeight + i));
                _spawnedPathPieces.Add(Instantiate(_edgePieces[Random.Range(0, _edgePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            for (int w = 0; w < maxWidth + amountOfLayers; w++)
            {
                pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);
                piecePosition = transform.position - new Vector3(_extents.x * maxWidth - _extents.x, 0, _extents.z * maxHeight - _extents.z) + new Vector3(_extents.x * 2 * (w - amountOfLayers), 0, _extents.z * 2 * (-1 - i));
                _spawnedPathPieces.Add(Instantiate(_edgePieces[Random.Range(0, _edgePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            for (int h = 0; h < maxHeight + amountOfLayers; h++)
            {
                pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);
                piecePosition = transform.position - new Vector3(_extents.x * maxWidth - _extents.x, 0, _extents.z * maxHeight - _extents.z) + new Vector3(_extents.x * 2 * (maxWidth + i), 0, _extents.z * 2 * (h - amountOfLayers));
                _spawnedPathPieces.Add(Instantiate(_edgePieces[Random.Range(0, _edgePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
            for (int h = 0; h < maxHeight + amountOfLayers; h++)
            {
                pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);
                piecePosition = transform.position - new Vector3(_extents.x * maxWidth - _extents.x, 0, _extents.z * maxHeight - _extents.z) + new Vector3(_extents.x * 2 * (-1 - i), 0, _extents.z * 2 * h);
                _spawnedPathPieces.Add(Instantiate(_edgePieces[Random.Range(0, _edgePieces.Length)], piecePosition, pieceRotation, transform.parent));
            }
        }
    }


    private void MakeIntersections()
    {
        for (int i = 0; i < _roadObjects.GetLength(0); i++)
        {
            for (int j = 0; j < _roadObjects.GetLength(1); j++)
            {
                Vector2Int _currentPos = new Vector2Int(i, j);
                Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * _currentPos.x, 0, _extents.z * 2 * _currentPos.y);

                if (IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(1, 0))
                    && IsPieceFacingNode(_currentPos, _currentPos + new Vector2Int(0, 1))
                    && IsPieceFacingNode(_currentPos, _currentPos - new Vector2Int(1, 0))
                    && IsPieceFacingNode(_currentPos, _currentPos - new Vector2Int(0, 1)))
                {
                    //Delete previous gameobject
                    int index = _spawnedPathPieces.FindIndex(gameObject => ((GameObject)gameObject).transform.position == piecePosition);
                    Destroy(((GameObject)_spawnedPathPieces.Find(gameObject => ((GameObject)gameObject).transform.position == piecePosition)).gameObject);
                    _spawnedPathPieces.RemoveAt(index);

                    //Set random rotation
                    Quaternion pieceRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);

                    //Instiantiate intersection
                    _spawnedPathPieces.Add(Instantiate(_crossSectionPieces[Random.Range(0, _crossSectionPieces.Length)], piecePosition, pieceRotation, transform.parent));
                    _dungeonPieces[_currentPos.x, _currentPos.y].roadPiece = RoadPiece.CrossSection;
                }
            }
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

    private bool IsPieceFacingNode(Vector2Int node, Vector2Int piece)
    {
        Vector3 piecePosition = transform.position - new Vector3(_extents.x * _roadObjects.GetLength(0) - _extents.x, 0, _extents.z * _roadObjects.GetLength(1) - _extents.z) + new Vector3(_extents.x * 2 * piece.x, 0, _extents.z * 2 * piece.y);
        try
        {
            if (node.x > piece.x)
            {
                switch (_dungeonPieces[piece.x, piece.y].roadPiece)
                {
                    case RoadPiece.Corner:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == -90
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 180
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 270)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.StraightRoad:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 0)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.TSection:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) != -90
                            && Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) != 270)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.DeadEnd:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 270 ||
                            Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == -90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.CrossSection:
                        return true;
                }
            }
            else if (node.x < piece.x)
            {
                switch (_dungeonPieces[piece.x, piece.y].roadPiece)
                {
                    case RoadPiece.Corner:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 0
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.StraightRoad:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 0)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.TSection:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) != 90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.DeadEnd:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.CrossSection:
                        return true;
                }
            }
            else if (node.y > piece.y)
            {
                switch (_dungeonPieces[piece.x, piece.y].roadPiece)
                {
                    case RoadPiece.Corner:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 90
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 180)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.StraightRoad:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.TSection:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) != 180)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.DeadEnd:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 180)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.CrossSection:
                        return true;
                }
            }
            else if (node.y < piece.y)
            {
                switch (_dungeonPieces[piece.x, piece.y].roadPiece)
                {
                    case RoadPiece.Corner:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 270
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 0
                            || Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == -90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.StraightRoad:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 90)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.TSection:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) != 0)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.DeadEnd:
                        if (Mathf.RoundToInt(((GameObject)_spawnedPathPieces.Find(spawnedPiece => ((GameObject)spawnedPiece).transform.position == piecePosition)).transform.rotation.eulerAngles.y) == 0)
                        {
                            return true;
                        }
                        break;
                    case RoadPiece.CrossSection:
                        return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }
}
