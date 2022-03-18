using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChessPiece : MonoBehaviour
{
    public bool diagonalMove;
    public bool horizontalMove;
    public int horizontalNumber;
    public int diagonalNumber;
    public bool pawnMove;
    public GameObject piece;
    public string name;
}
