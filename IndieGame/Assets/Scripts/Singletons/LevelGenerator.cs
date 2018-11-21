using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private Object[] _cornerPieces;
    private Object[] _edgePieces;
    private Object[] _middlePieces;

    private Vector3 _extents;

    // Use this for initialization
    void Start()
    {
        _cornerPieces = Resources.LoadAll("Procedural Level Prefabs/Corner Pieces");
        _edgePieces = Resources.LoadAll("Procedural Level Prefabs/Edge Pieces");
        _middlePieces = Resources.LoadAll("Procedural Level Prefabs/Middle Pieces");

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
}
