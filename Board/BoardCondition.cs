using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoardCondition
{
    public static bool IsInCheck(BoardState boardState, bool whiteOrBlack) => 
        boardState.GetAttackers(boardState.GetKingSquare(whiteOrBlack), !whiteOrBlack).Length > 0;

    public static bool IsCheckMate(BoardState boardState, bool whiteOrBlack)
    {
        Vector2Int[] squaresWithPieces = boardState.GetPiecesSquares(whiteOrBlack);

        return !squaresWithPieces.Any(t => Chess.I.GetLegalMoves(boardState, t).Length > 0);
    }

    public static bool WillLoseMaterial(BoardState boardState, bool whiteOrBlack)
    {
        foreach (Vector2Int square in boardState.GetPiecesSquares(whiteOrBlack))
        {
            int[] defendersValues = GetAttackersValues(boardState, square, whiteOrBlack);
            int[] attackersValues = GetAttackersValues(boardState, square, !whiteOrBlack);

            if (WinsExchange(defendersValues, attackersValues, 
                    boardState.GetPiece(square).GetPieceValue())) return true;
        }

        return false;
    }

    public static bool DoesNotWinMaterial(BoardState boardState, bool whiteOrBlack)
    {
        Vector2Int squareMovedTo = boardState.GetSquareMovedTo();

        Piece pieceMoved = boardState.GetPiece(squareMovedTo);
        Piece pieceTaken = boardState.GetPiece(squareMovedTo, boardState.GetPreviousBoard());
        if (pieceTaken == null) return true;
        
        int[] defendersValues = GetAttackersValues(boardState, squareMovedTo, !whiteOrBlack);
        List<int> attackersValues = GetAttackersValues(boardState, squareMovedTo, whiteOrBlack).ToList();
        attackersValues.Insert(0, pieceMoved.GetPieceValue());
        
        return !WinsExchange(defendersValues, attackersValues.ToArray(), pieceTaken.GetPieceValue());
    }

    private static int[] GetAttackersValues(BoardState boardState, Vector2Int square, bool whiteOrBlack) =>
        boardState.GetAttackers(square, whiteOrBlack)
            .Select(t => boardState.GetPiece(t).GetPieceValue())
            .OrderBy(t => t).ToArray();

    private static bool WinsExchange(int[] defendersValues, int[] attackersValues, int initialPointsToTake)
    {
        int valueToTake = initialPointsToTake;
        int pointsTotal = 0;
        for (int i = 0; i < attackersValues.Length; i++)
        {
            pointsTotal += valueToTake;
            if (i == defendersValues.Length) break;
            pointsTotal -= attackersValues[i];
            if (pointsTotal > 0) return true;
            valueToTake = defendersValues[i];
        }
        return pointsTotal > 0;
    }
}