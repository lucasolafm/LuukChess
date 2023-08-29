using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pawn : Piece
{
    private bool canEnPassant;
    private Vector2Int enPassantSquare;

    public Pawn(bool whiteOrBlack, bool hasMoved = false) : base(whiteOrBlack, hasMoved) {}
    
    public override Piece Copy() => new Pawn(whiteOrBlack, hasMoved);

    public override PieceType GetPieceType() => PieceType.Pawn;

    public override int GetPieceValue() => 1;

    public override Sprite GetSprite() => Chess.I.pawn[whiteOrBlack ? 0 : 1];

    public override Vector2Int[] GetSquares(BoardState boardState, Vector2Int currentSquare)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        
        Vector2Int squareVertical = currentSquare + GetMoveForward();
        if (!boardState.HasPiece(squareVertical))
        {
            squares.Add(squareVertical);
            Vector2Int squareVerticalDouble = currentSquare + GetMoveForward() * 2;
            if (!boardState.HasPiece(squareVerticalDouble) && !hasMoved)
            {
                squares.Add(squareVerticalDouble);
            }
        }

        Vector2Int[] squaresAttacking =
            GetSquaresAttacking(boardState, currentSquare)
                .Where(t => boardState.HasPiece(t, !whiteOrBlack) || 
                            CanEnPassant(t, boardState)).ToArray();

        return squares.Concat(squaresAttacking).ToArray();
    }

    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        new[]
        {
            currentSquare + GetMoveForward() - Vector2Int.right,
            currentSquare + GetMoveForward() + Vector2Int.right
        }.Where(boardState.IsInsideBoard).ToArray();

    public override void OnMove(Vector2Int square, BoardState boardState)
    {
        base.OnMove(square, boardState);
        
        if (IsWhiteOrBlack() && square.y == 7 || !whiteOrBlack && square.y == 0)
        {
            Queen queen = new Queen(whiteOrBlack);
            boardState.SetPiece(square, queen);
            AudioManager.I.OnPromoted();
        }

        if (square == enPassantSquare && canEnPassant)
        {
            Vector2Int pawnSquare = enPassantSquare - GetMoveForward();
            boardState.SetPiece(pawnSquare, null);
        }
    }

    private bool CanEnPassant(Vector2Int middleSquare, BoardState boardState)
    {
        Vector2Int startSquare = middleSquare + GetMoveForward();
        Vector2Int endSquare = middleSquare - GetMoveForward();
        Piece[][] previousBoard = boardState.GetPreviousBoard();
        Piece previousPieceStart = boardState.GetPiece(startSquare, previousBoard);
        Piece pieceStart = boardState.GetPiece(startSquare);
        Piece pieceEnd = boardState.GetPiece(endSquare);
        if (previousPieceStart == null || pieceStart != null ||
            pieceEnd == null || pieceEnd.GetPieceType() != PieceType.Pawn)
        {
            canEnPassant = false;
            return false;
        }

        canEnPassant = true;
        enPassantSquare = middleSquare;
        return true;
    }
    
    private Vector2Int GetMoveForward() => (whiteOrBlack ? 1 : -1) * Vector2Int.up;
}