using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Piece
{
    private Vector2Int castleSquareLeft;
    private Vector2Int castleSquareRight;
    private Vector2Int castleRookSquareLeft;
    private Vector2Int castleRookSquareRight;
    private bool canCastle;
    
    public King(bool whiteOrBlack, bool hasMoved = false) : base(whiteOrBlack, hasMoved) { }
    
    public override Piece Copy() => new King(whiteOrBlack, hasMoved);
    public override PieceType GetPieceType() => PieceType.King;
    public override int GetPieceValue() => int.MaxValue;

    public override Sprite GetSprite() => Chess.I.king[whiteOrBlack ? 0 : 1];

    public override Vector2Int[] GetSquares(BoardState boardState, Vector2Int currentSquare)
    {
        Vector2Int[] defaultSquares = 
            GetSquaresAttacking(boardState, currentSquare)
                .Where(t => !boardState.HasPiece(t, whiteOrBlack)).ToArray();

            List<Vector2Int> castleSquares = new();
        bool canCastleLeft = CanCastle(-1, boardState, currentSquare, 
            out castleSquareLeft, out castleRookSquareLeft);
        if (canCastleLeft)
        {
            castleSquares.Add(castleSquareLeft);
        }

        bool canCastleRight = CanCastle(1, boardState, currentSquare, 
            out castleSquareRight, out castleRookSquareRight);
        if (canCastleRight)
        {
            castleSquares.Add(castleSquareRight);
        }

        canCastle = canCastleLeft || canCastleRight;

        return defaultSquares.Concat(castleSquares.ToArray()).ToArray();
    }

    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        new []
        {
            currentSquare + Vector2Int.up,
            currentSquare + Vector2Int.right,
            currentSquare + Vector2Int.down,
            currentSquare + Vector2Int.left,
            currentSquare + Vector2Int.up + Vector2Int.right,
            currentSquare + Vector2Int.right + Vector2Int.down,
            currentSquare + Vector2Int.down + Vector2Int.left,
            currentSquare + Vector2Int.left + Vector2Int.up
        }.Where(boardState.IsInsideBoard).ToArray();

    public override void OnMove(Vector2Int square, BoardState boardState)
    {
        base.OnMove(square, boardState);

        if (!canCastle) return;

        if (square == castleSquareLeft)
        {
            boardState.MovePiece(castleRookSquareLeft, square + Vector2Int.right);
            AudioManager.I.OnCastled();
        }
        else if (square == castleSquareRight)
        {
            boardState.MovePiece(castleRookSquareRight, square - Vector2Int.right);
            AudioManager.I.OnCastled();
        }
    }

    private bool CanCastle(int side, BoardState boardState, Vector2Int currentSquare, 
        out Vector2Int castleSquare, out Vector2Int castleRookSquare)
    {
        castleSquare = new Vector2Int();
        castleRookSquare = new Vector2Int();
        
        if (hasMoved || boardState.IsAttacked(currentSquare, whiteOrBlack)) return false;
        
        Vector2Int square;
        Piece piece;
        int count = 0;
        do
        {
            square = currentSquare + (count + 1) * side * Vector2Int.right;
            if (count < 2 && boardState.IsAttacked(square, whiteOrBlack)) return false;
            count++;
            
            piece = boardState.GetPiece(square);
            if (piece == null || piece.GetPieceType() != PieceType.Rook || piece.HasMoved()) continue;
            
            castleSquare = currentSquare + side * 2 * Vector2Int.right;
            castleRookSquare = square;
            return true;
        } 
        while (boardState.IsInsideBoard(square) && piece == null && 
               (count >= 2 || !boardState.IsAttacked(square, whiteOrBlack)));
        
        return false;
    }
}