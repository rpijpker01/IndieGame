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
        Vector2Int _startNode;
        Vector2Int _endNode;
        Vector2Int _currentNode;

        //Random start node
        _startNode = new Vector2Int(Random.Range(0, width - 1), 0);
        //Set current node to start node
        _currentNode = _startNode;
        //Random end node
        //switch (Random.Range(0, 2))
        //{
        //    case 0:
        _endNode = new Vector2Int(Random.Range(0, width - 1), height - 1);
        //        break;
        //    case 1:
        //        _endNode = new Vector2Int(0, Random.Range(0, height - 1));
        //        break;
        //    default:
        //        _endNode = new Vector2Int(width - 1, Random.Range(0, height - 1));
        //        break;

        //}

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
        while (_currentNode.x != _endNode.x || _currentNode.y != _endNode.y)
        {
            nextNode = PathSegmentEnd(_currentNode, _endNode, width, height);
            if (nextNode.x == -1 && nextNode.y == -1) { break; }
            int stepAmount = Mathf.Abs(_currentNode.x - nextNode.x + _currentNode.y - nextNode.y);

            for (int ii = 0; ii < _dungeonPieces.GetLength(0); ii++)
            {
                //Iterate over node height
                for (int jj = 0; jj < _dungeonPieces.GetLength(1); jj++)
                {
                    AddAdjecents(ii, jj, _dungeonPieces);
                }
            }

            for (int j = 0; j < stepAmount; j++)
            {
                Vector3 piecePosition = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _currentNode.x, 0, _extents.z * 2 * _currentNode.y);
                Quaternion pieceRotation = transform.rotation;

                //Set the roadpiece enum
                _dungeonPieces[_currentNode.x, _currentNode.y].roadPiece = RoadPiece.StraightRoad;

                //Instantiate object
                _spawnedPathPieces.Add(Instantiate(_middlePieces[Random.Range(0, _middlePieces.Length)], piecePosition, pieceRotation, transform.parent));

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

            //Get position and rotation
            Vector3 piecePosition1 = transform.position - new Vector3(_extents.x * width - _extents.x, 0, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _currentNode.x, 0, _extents.z * 2 * _currentNode.y);
            Quaternion pieceRotation1 = transform.rotation;

            //Set the roadpiece enum
            _dungeonPieces[_currentNode.x, _currentNode.y].roadPiece = RoadPiece.Corner;

            //Instantiate object
            _spawnedPathPieces.Add(Instantiate(_cornerPieces[Random.Range(0, _cornerPieces.Length)], piecePosition1, pieceRotation1, transform.parent));
        }

        //TEMP
        Vector3 piecePosition2 = transform.position - new Vector3(_extents.x * width - _extents.x, -100, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _startNode.x, 0, _extents.z * 2 * _startNode.y);
        Quaternion pieceRotation2 = transform.rotation;

        _spawnedPathPieces.Add(Instantiate(_cornerPieces[0], piecePosition2, pieceRotation2, transform.parent));
        piecePosition2 = transform.position - new Vector3(_extents.x * width - _extents.x, -100, _extents.z * height - _extents.z) + new Vector3(_extents.x * 2 * _endNode.x, 0, _extents.z * 2 * _endNode.y);
        _spawnedPathPieces.Add(Instantiate(_cornerPieces[1], piecePosition2, pieceRotation2, transform.parent));
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

        if (returnList.Count == 0)
        {
            return null;
        }

        return returnList;
    }

    private Vector2Int PathSegmentEnd(Vector2Int currentNode, Vector2Int endNode, int width, int height)
    {
        List<AvailableDirections> directions = ReturnPathDirections(currentNode.x, currentNode.y, endNode.x, endNode.y, _dungeonPieces);
        if (directions == null)
        {
            return new Vector2Int(-1, -1);
        }
        switch (directions[Random.Range(0, directions.Count - 1)])
        {
            case AvailableDirections.Up:
                return new Vector2Int(currentNode.x, Random.Range(currentNode.y + 1, height - 1));
            case AvailableDirections.Right:
                return new Vector2Int(Random.Range(currentNode.x + 1, width - 1), currentNode.y);
            case AvailableDirections.Down:
                return new Vector2Int(currentNode.x, Random.Range(0, currentNode.y - 1));
            case AvailableDirections.Left:
                return new Vector2Int(Random.Range(0, currentNode.x - 1), currentNode.y);
        }
        return Vector2Int.zero;
    }
}
