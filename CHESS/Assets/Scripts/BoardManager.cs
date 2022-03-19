using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardCube;
    public GameObject chessFigurePawn;
    public GameObject chessFigureRook;
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
    private bool figureSelected;
    private ChessPiece selectedPiece;

    private int x;
    private int y;

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
        InstantiateRooks();
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
        RaycastHit hitFigure;
        Ray rayFigure = mainCamera.ScreenPointToRay(Input.mousePosition);
        int indexPawn;
        if (!figureSelected)
        {
            if (Physics.Raycast(rayFigure, out hitFigure))
            {
                if (hitFigure.transform.GetComponent<ChessPiece>())
                {
                    selectedPiece = hitFigure.transform.GetComponent<ChessPiece>();
                    figureSelected = true;
                    selectedPiece.GetComponent<Renderer>().material.color = Color.green;
                    MoveFigure(selectedPiece);
                }
            }
        }
        else if (figureSelected)
        {
            if (Physics.Raycast(rayFigure, out hitFigure))
            {
                if (hitFigure.transform.GetComponent<ChessPiece>())
                {
                    selectedPiece.GetComponent<Renderer>().material.color = Color.red;
                    selectedPiece = hitFigure.transform.GetComponent<ChessPiece>();
                    figureSelected = true;
                    selectedPiece.GetComponent<Renderer>().material.color = Color.green;
                    MoveFigure(selectedPiece);
                }
                else
                {
                    FindIndicesOfObjectToMove(selectedPiece.transform.parent.gameObject, out x, out y);
                    selectedPiece.GetComponent<Renderer>().material.color = Color.green;
                    MoveFigure(selectedPiece);
                }
            }
        }
    }

    private void MoveFigure(ChessPiece pieceToMove)
    {
        int indexFigure;
        indexFigure = listOfPieces.IndexOf(pieceToMove);
        if (listOfPieces[indexFigure].name == "rook" &&
            pieceToMove.transform.gameObject == listOfPieces[indexFigure].piece)
        {
            RaycastHit hitMove;
            Ray rayMove = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (figureSelected)
            {
                if (Physics.Raycast(rayMove, out hitMove))
                {
                    int jFound;
                    int kFound;
                    if (FindIndicesOfObjectToMove(hitMove.transform.gameObject, out jFound, out kFound))
                    {
                        y = kFound;
                        listOfPieces[indexFigure].piece.transform.DOMove(
                            cubesArray[x, y].transform.position +
                            Vector3.up, 1f);
                        listOfPieces[indexFigure].piece.transform.parent =
                            cubesArray[x, y].transform;
                        figureSelected = false;
                        selectedPiece.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }

        if (pieceToMove.transform.GetComponent<ChessPiece>())
        {
            int jFound;
            int kFound;
            if (listOfPieces[indexFigure].name == "pawn" &&
                pieceToMove.transform.gameObject == listOfPieces[indexFigure].piece)
            {
                if (FindIndicesOfObjectToMove(pieceToMove.transform.parent.gameObject, out jFound, out kFound))
                {
                    if (listOfPieces[indexFigure].pawnMove == true)
                    {
                        listOfPieces[indexFigure].piece.transform.DOMove(
                            cubesArray[jFound, kFound + 2].transform.position + Vector3.up, 1f);
                        listOfPieces[indexFigure].pawnMove = false;
                        listOfPieces[indexFigure].piece.transform.parent = cubesArray[jFound, kFound + 2].transform;
                        figureSelected = false;
                        selectedPiece.GetComponent<Renderer>().material.color = Color.red;
                    }
                    else
                    {
                        listOfPieces[indexFigure].piece.transform.DOMove(
                            cubesArray[jFound, kFound + listOfPieces[indexFigure].horizontalNumber].transform
                                .position +
                            Vector3.up, 1f);
                        listOfPieces[indexFigure].piece.transform.parent =
                            cubesArray[jFound, kFound + listOfPieces[indexFigure].horizontalNumber].transform;
                        figureSelected = false;
                        selectedPiece.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }
    }

    bool FindIndicesOfObjectToMove(GameObject objectToLookFor, out int j, out int k)
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

        if (chessPiece.name == "rook")
        {
            chessPiece.diagonalMove = false;
            chessPiece.horizontalMove = true;
            chessPiece.horizontalNumber = 0;
        }
    }

    private void InstantiatePawns()
    {
        for (int i = 0; i < widthCount; i++)
        {
            GameObject pawnPiece = Instantiate(chessFigurePawn, cubesArray[i, 0].transform.position + Vector3.up,
                Quaternion.identity);
            chessPiece = pawnPiece.GetComponent<ChessPiece>();
            chessPiece.piece = pawnPiece;
            chessPiece.piece.transform.SetParent(cubesArray[i, 0].transform, true);
            chessPiece.name = "pawn";
            SetValues(chessPiece);
            listOfPieces.Add(chessPiece);
        }
    }

    private void InstantiateRooks()
    {
        for (int i = 0; i < widthCount; i += widthCount - 1)
        {
            GameObject rookPiece = Instantiate(chessFigureRook, cubesArray[i, 1].transform.position + Vector3.up,
                Quaternion.identity);
            chessPiece = rookPiece.GetComponent<ChessPiece>();
            chessPiece.piece = rookPiece;
            chessPiece.piece.transform.SetParent(cubesArray[i, 1].transform, true);
            chessPiece.name = "rook";
            SetValues(chessPiece);
            listOfPieces.Add(chessPiece);
        }
    }
}