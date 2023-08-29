using System.Collections.Generic;
using UnityEngine;

public class PointerManager
{
    private List<Transform> squarePointers = new();
    private List<Transform> piecePointers = new();
    
    public void ShowPointers(Vector2Int[] squares, BoardState boardState)
    {
        foreach (Vector2Int square in squares)
        {
            Transform pointer;
            if (boardState.HasPiece(square))
            {
                pointer = PointerFactory.I.GetPiecePointer();
                piecePointers.Add(pointer);
            }
            else
            {
                pointer = PointerFactory.I.GetSquarePointer();
                squarePointers.Add(pointer);
            }

            pointer.position = (Vector3Int)square;
        }
    }

    public void HidePointers()
    {
        PointerFactory.I.ReturnSquarePointers(squarePointers);
        PointerFactory.I.ReturnPiecePointers(piecePointers);
        squarePointers.Clear();
        piecePointers.Clear();
    }
}