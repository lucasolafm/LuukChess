using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public abstract class Piece
{
    protected bool whiteOrBlack;
    private PieceType pieceType;
    private int pieceValue;
    protected bool hasMoved;

    protected Piece(bool whiteOrBlack, bool hasMoved = false)
    {
        this.whiteOrBlack = whiteOrBlack;
        this.hasMoved = hasMoved;
    }

    public abstract Piece Copy();
    
    public bool IsWhiteOrBlack() => whiteOrBlack;
    
    public abstract PieceType GetPieceType();

    public abstract int GetPieceValue();

    public abstract Sprite GetSprite();

    public virtual Vector2Int[] GetSquares(BoardState boardState, Vector2Int currentSquare) =>         
        GetSquaresAttacking(boardState, currentSquare)
            .Where(t => !boardState.HasPiece(t, whiteOrBlack)).ToArray();

    public abstract Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare);
    
    public virtual void OnMove(Vector2Int square, BoardState boardState) => hasMoved = true;

    public bool HasMoved() => hasMoved;
    
    protected Vector2Int[] ContinuousLines(BoardState boardState, Vector2Int origin, 
        Vector2Int[] directions, int range = int.MaxValue)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int square;
            int count = 0;
            do
            {
                count++;
                square = origin + direction * count;

                if (!boardState.IsInsideBoard(square)) break;
                
                squares.Add(square);
            } 
            while (count < range && boardState.GetPiece(square) == null);
        }
        
        return squares.ToArray();
    }
}