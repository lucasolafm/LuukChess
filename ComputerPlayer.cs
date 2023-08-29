using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ComputerPlayer
{
    public static void Play(BoardState currentBoardState, bool whiteOrBlack)
    {
        Vector2Int targetSquare;
        Vector2Int[] squares = currentBoardState.GetPiecesSquares(whiteOrBlack)
            .OrderBy(_ => new System.Random().Next()).ToArray();

        List<Func<BoardState, bool, bool>> problemsPersists = new List<Func<BoardState, bool, bool>>
        {
            BoardCondition.WillLoseMaterial,
            BoardCondition.DoesNotWinMaterial
        };

        for (int i = problemsPersists.Count; i >= 0; i--)
        {
            foreach (Vector2Int square in squares)
            {
                Vector2Int[] moves = Chess.I.GetMoves(currentBoardState, whiteOrBlack, square, 
                    new List<Func<BoardState, bool, bool>>(problemsPersists));
                
                if (moves.Length == 0) continue;

                targetSquare = moves[Random.Range(0, moves.Length)];
                Chess.I.MakeAMove(square, targetSquare);
                return;
            }

            problemsPersists.RemoveAt(problemsPersists.Count - 1);
        }
    }
}