using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardCube;
    public GameObject chessFigure;
    public List<GameObject> cubesList = new List<GameObject>();
    public List<String> letterList = new List<string>();
    private int positionNumber = 1;
    private int positionLetter = 0;
    public Camera mainCamera;
    private static int heightCount = 8;
    private static int widthCount = 8;
    private GameObject[,] cubesArray = new GameObject[heightCount, widthCount];
    private ChessPiece chessPiece;
    public List<ChessPiece> listOfPieces = new List<ChessPiece>();

    private enum Letters
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
    }

    private void Start()
    {
        string[] letters = System.Enum.GetNames(typeof(Letters));
        foreach (var letter in letters)
        {
            letterList.Add(letter);
        }

        for (int i = 0, a = 0; i < 8; i++, a += 2)
        {
            for (int j = 0, b = 0; j < 8; j++, b += 2)
            {
                GameObject cube = Instantiate(boardCube, new Vector3(a, 0, b), Quaternion.identity);
                cube.name = letterList[positionLetter] + positionNumber.ToString();
                cubesArray[i, j] = cube;
                cubesList.Add(cube);

                if (positionNumber < 8)
                {
                    positionNumber++;
                }
                else if (positionNumber >= 8)
                {
                    positionNumber = 1;
                }
            }

            positionLetter++;
        }

        InstantiatePawns();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            MoveToPosition();
        }
    }

    private void MoveToPosition()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int index;
        if (Physics.Raycast(ray, out hit))
        {
            index = listOfPieces.IndexOf(hit.transform.GetComponent<ChessPiece>());
            if (hit.transform.gameObject == listOfPieces[index].piece)
            {
                int jFound;
                int kFound;
                if (FindIndicesOfObject(hit.transform.parent.gameObject, out jFound, out kFound))
                {
                    if (listOfPieces[index].name == "pawn")
                    {
                        if (listOfPieces[index].pawnMove == true)
                        {
                            listOfPieces[index].piece.transform.DOMove(
                                cubesArray[jFound, kFound + 2].transform.position + Vector3.up, 1f);
                            listOfPieces[index].pawnMove = false;
                            listOfPieces[index].piece.transform.parent = cubesArray[jFound, kFound + 2].transform;
                        }
                        else
                        {
                            listOfPieces[index].piece.transform.DOMove(
                                cubesArray[jFound, kFound + listOfPieces[index].horizontalNumber].transform.position +
                                Vector3.up, 1f);
                            listOfPieces[index].piece.transform.parent =
                                cubesArray[jFound, kFound + listOfPieces[index].horizontalNumber].transform;
                        }
                    }
                }
            }
        }
    }

    bool FindIndicesOfObject(GameObject objectToLookFor, out int j, out int k)
    {
        for (j = 0; j < heightCount; j++)
        {
            for (k = 0; k < widthCount; k++)
            {
                // Is this the one?
                if (cubesArray[j, k] == objectToLookFor)
                {
                    return true;
                }
            }
        }

        j = k = -1;
        return false;
    }

    private void SetValues(ChessPiece chessPiece)
    {
        if (chessPiece.name == "pawn")
        {
            chessPiece.diagonalMove = false;
            chessPiece.horizontalMove = true;
            chessPiece.horizontalNumber = 1;
            chessPiece.pawnMove = true;
        }
    }

    private void InstantiatePawns()
    {
        for (int i = 0; i < widthCount; i++)
        {
            GameObject pawnPiece = Instantiate(chessFigure, cubesArray[i, 0].transform.position + Vector3.up, Quaternion.identity);
            chessPiece = pawnPiece.GetComponent<ChessPiece>();
            chessPiece.piece = pawnPiece;
            chessPiece.piece.transform.SetParent(cubesArray[i, 0].transform, true);
            chessPiece.name = "pawn";
            SetValues(chessPiece);
            listOfPieces.Add(chessPiece);
        }
    }
}